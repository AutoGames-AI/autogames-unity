using System;
#if UNITY_WEBGL && !UNITY_EDITOR
using System.Runtime.InteropServices;
#endif
using UnityEngine;
using UnityEngine.Events;

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
        [DllImport("__Internal")]
        private static extern string GetAccessTokenJS();
#endif

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

        public string GetAccessToken()
        {
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
            Debug.Log(token != null ? $"Токен: {token}" : "Токена нэт");
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

