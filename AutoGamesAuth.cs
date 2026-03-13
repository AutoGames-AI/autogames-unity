using System;
using System.Collections;
using System.Text;
//#if UNITY_WEBGL && !UNITY_EDITOR
using System.Runtime.InteropServices;
//#endif
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace QAI.LogIn
{
    // The GameObject name must be "AuthManager"; if you rename it, update index.html accordingly.
    public class AutoGamesAuth : MonoBehaviour
    {
        public static AutoGamesAuth Instance;

        [HideInInspector] public UnityEvent<UserProfile> onLogin;
        [HideInInspector] public UnityEvent onLogout;


        public UserProfile UserProfile { get; private set; }

#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void TriggerAutoGamesLogin();
        [DllImport("__Internal")]
        private static extern void TriggerAutoGamesLogout();
        
#endif

        [DllImport("__Internal")]
        private static extern string GetAccessTokenJS();

        private void Awake()
        {
            Instance = this;
        }

        public void OnLoginButtonClicked()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            TriggerAutoGamesLogin();
#else
            Debug.Log("Login available only in WebGL build. Simulating...");
            // You can simulate a test profile in the editor
            OnAuthStateChanged("{\"ID\":123,\"Email\":\"test@test.local\",\"Name\":\"Dev\"}");
#endif
        }

        public void OnLogoutButtonClicked()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            TriggerAutoGamesLogout();
#else
            Debug.Log("Logout (editor)");
            OnAuthStateChanged("");
#endif
        }

        // This method is called from JS: SendMessage('AuthManager','OnAuthStateChanged', payload);
        // payload = JSON string of the profile or an empty string on logout
        public void OnAuthStateChanged(string payloadJson)
        {
            if (string.IsNullOrEmpty(payloadJson))
            {
                Debug.Log("User logged out");
                // clear local session
                HandleLoggedOut();
                return;
            }

            try
            {
                // For simplicity, we use JsonUtility. The SDK's JSON may contain fields not supported by JsonUtility.
                UserProfile profile = JsonUtility.FromJson<UserProfile>(payloadJson);
                Debug.Log($"User logged in: {profile.Email} (ID {profile.ID})");
                HandleLoggedIn(profile);
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to parse profile from JS: " + e);
            }
        }

        private async void HandleLoggedIn(UserProfile profile)
        {
            await Awaitable.EndOfFrameAsync();

            UserProfile = profile;

            onLogin?.Invoke(profile);

            PrintToken();
        }

        private void HandleLoggedOut()
        {
            onLogout?.Invoke();
        }

        public async Awaitable<string> GetAccessToken()
        {
            await RefreshTokenAsync(GetAccessTokenJS());

#if UNITY_WEBGL && !UNITY_EDITOR
            return GetAccessTokenJS();
#else
            return "В юньке его нет :(";
#endif
        }


        public async void PrintToken()
        {
            await Awaitable.WaitForSecondsAsync(1f);

#if UNITY_WEBGL && !UNITY_EDITOR
            string token = GetAccessTokenJS();
            Debug.Log(token != null ? $"Access Token: {token}" : "Token not foundoken");
#endif
        }

#if UNITY_EDITOR
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.H))
            {
                OnLoginButtonClicked();
            }
            if (Input.GetKeyDown(KeyCode.O))
            {
                OnLogoutButtonClicked();
            }
        }
#endif

        private const string RefreshUrl = "https://api.dev.autogames.app/sso/refresh-token";

        // Класс для структуры запроса (если нужен JSON в теле)
        [System.Serializable]
        public class RefreshRequest
        {
            //public string refresh_token;
        }

        public async Awaitable RefreshTokenAsync(string oldToken)
        {
            var data = new RefreshRequest { };
            string json = JsonUtility.ToJson(data);

            using (UnityWebRequest request = new UnityWebRequest(RefreshUrl, "POST"))
            {
                byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();

                request.SetRequestHeader("Content-Type", "application/json");
                //request.SetRequestHeader("Authorization", "Bearer " + oldToken);

                // В Unity можно дождаться окончания UnityWebRequestAsyncOperation с помощью await
                var operation = request.SendWebRequest();

                while (!operation.isDone)
                {
                    await Awaitable.NextFrameAsync(); // Ждем следующего кадра, чтобы не блокировать основной поток
                }

                if (request.result == UnityWebRequest.Result.Success)
                {
                    Debug.Log("Токен успешно обновлен: " + request.downloadHandler.text);
                    // Тут парсишь новый токен и сохраняешь его
                }
                else
                {
                    Debug.LogError("Token refresh error: " + request.error);
                }
            }
        }

    }

    [Serializable]
    public class UserProfile
    {
        public int ID;
        public string Email;
        public string Name;
        public string SolanaAddress;
    }
}

