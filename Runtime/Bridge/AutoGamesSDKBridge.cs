using System;
using System.Runtime.InteropServices;
using QAI.Dtos;
using UnityEngine;
using UnityEngine.Events;

namespace QAI
{
    public class AutoGamesSDKBridge : MonoBehaviour
    {
        public static UnityEvent<UserProfileAssetDto> onProfileAssetReceive = new();
        public static bool IsMobileOrTablet { get; private set; }
        public static AutoGamesSDKBridge Instance;

        public VotingsDto VotingsResult { get; private set; }

        [DllImport("__Internal")]
        private static extern void FetchVotingsFromJS(int tokenId);

        [DllImport("__Internal")]
        private static extern void VoteFromJS(int voteId, int optionId);

        [DllImport("__Internal")]
        private static extern void FetchUserAssetsFromJS(string profileId);

        public UnityEvent<VotingsDto> onVotingsReceive;
        public UnityEvent onVoteSuccess;

        private void Awake()
        {
            Instance = this;
        }

        public void SetPlatform(string platformType)
        {
            IsMobileOrTablet = platformType != "Desktop";
        }

        public void RequestVotings(int tokenId = -1)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            FetchVotingsFromJS(tokenId);
#else
            Debug.Log($"[Editor] RequestVotings called for token: {(tokenId == -1 ? "ALL" : tokenId.ToString())}");
#endif
        }

        public void OnVotingsLoaded(string json)
        {
            try
            {
                VotingsResult = JsonUtility.FromJson<VotingsDto>(json);
                onVotingsReceive?.Invoke(VotingsResult);
                if (VotingsResult?.votings != null)
                {
                    foreach (var voting in VotingsResult.votings)
                    {
                        Debug.Log($"[SDK] Voting: {voting.Title}, Kind: {voting.GetVotingKind()}");
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Votings parsing error: " + e.Message);
            }
        }

        public void SendVote(int voteId, int optionId)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            VoteFromJS(voteId, optionId);
#else
            Debug.Log($"[Editor] Vote {voteId}, Option {optionId}");
#endif
        }

        public void OnVoteSuccess(string resultJson)
        {
            Debug.Log($"Vote success! {resultJson}");
            onVoteSuccess?.Invoke();
        }

        public void OnVoteError(string errorMessage)
        {
            Debug.LogError("Voting error: " + errorMessage);
        }

        public void OnAssetsLoaded(string profileAssets)
        {
            var userProfileAssets = JsonUtility.FromJson<UserProfileAssetDto>(profileAssets);
            onProfileAssetReceive?.Invoke(userProfileAssets);
        }
    }
}
