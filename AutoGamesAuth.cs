using System;
using System.Threading.Tasks;
#if UNITY_WEBGL && !UNITY_EDITOR
using System.Runtime.InteropServices;
#endif
using UnityEngine;
using UnityEngine.Events;

namespace QAI.LogIn
{
    public class AutoGamesAuth : MonoBehaviour
    {
        public static AutoGamesAuth Instance;

        [HideInInspector] public UnityEvent<UserProfile> onLogin = new();
        [HideInInspector] public UnityEvent onLogout = new();

        public UserProfile UserProfile { get; private set; }

        // Используем TaskCompletionSource для превращения callback-системы в awaitable
        private TaskCompletionSource<string> _tokenRequestSource;

#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void TriggerAutoGamesLogin();
        
        [DllImport("__Internal")]
        private static extern void TriggerAutoGamesLogout();

        [DllImport("__Internal")]
        private static extern void RequestAccessTokenJS();
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

        public void OnAuthStateChanged(string payloadJson)
        {
            if (string.IsNullOrEmpty(payloadJson))
            {
                UserProfile = null;
                onLogout?.Invoke();
                return;
            }

            try
            {
                UserProfile profile = JsonUtility.FromJson<UserProfile>(payloadJson);
                UserProfile = profile;
                onLogin?.Invoke(profile);

                // Пример асинхронного получения после логина
                _ = PrintTokenAsync();
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to parse profile from JS: " + e);
            }
        }

        // Метод, который вызывает JS через SendMessage
        public void OnAccessTokenReceived(string token)
        {
            if (_tokenRequestSource != null)
            {
                _tokenRequestSource.TrySetResult(token);
                _tokenRequestSource = null;
            }
        }

        public async Task<string> GetAccessToken()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            // Если запрос уже идет, ждем его
            if (_tokenRequestSource != null) return await _tokenRequestSource.Task;

            _tokenRequestSource = new TaskCompletionSource<string>();
            RequestAccessTokenJS();
            
            return await _tokenRequestSource.Task;
#else
            return "В юньке его нет :(";
#endif
        }

        private async Task PrintTokenAsync()
        {
            string token = await GetAccessToken();
            Debug.Log(!string.IsNullOrEmpty(token) ? $"Access Token: {token}" : "Token not found");
        }

#if UNITY_EDITOR
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.H)) OnLoginButtonClicked();
            if (Input.GetKeyDown(KeyCode.O)) OnLogoutButtonClicked();
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