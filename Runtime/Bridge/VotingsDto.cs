using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace QAI.Dtos
{
    [Serializable]
    public class VotingsDto
    {
        public List<VotingItem> votings;
        public string next_cursor;
    }

    [Serializable]
    public class VotingItem
    {
        public int ID;
        public string CreatedAt;
        public string UpdatedAt;
        public int GameID;
        public int TokenID;
        public string Title;
        public string Description;
        public string Status;
        public string voting_starts_at;
        public long voting_duration; // Используем long для больших чисел таймстампа

        public MetaData MetaData; // Имя должно совпадать с JSON
        public List<VotingOption> Options;
        public List<Vote> Votes;

        // Вспомогательный метод для удобного получения Enum
        public VotingKind GetVotingKind() => VotingEnumConverter.GetKind(MetaData?.voting_kind);
        public BehaviorState GetBehaviorState() => VotingEnumConverter.GetBehaviorState(Description);
    }

    [Serializable]
    public class VotingOption
    {
        public int ID;
        public string CreatedAt;
        public string UpdatedAt;
        public int TokenVotingID;
        public string Title;
        public string Description;
        public int votedTokens;
        public int votersCount;
        public long votingPower;

        public MetaData MetaData;
    }

    [Serializable]
    public class MetaData
    {
        // Поля из разных типов голосований (все string)
        public string voting_kind;
        public string avatarId;
        public string behavior;
        public string from;
        public string to;

        // Для старых версий или действий персонажа
        public string action;
    }

    [Serializable]
    public class Vote
    {
        public int id;
        public int userTokenVotingPower;
        public string status;
        public UserData user;
    }

    [Serializable]
    public class UserData
    {
        public int id;
        public string name;
        public string walletPublicKey;
    }

    public enum VotingKind
    {
        Unknown,
        ToChangeAvatarBehavior,
        ToProposeArchEnemy,
        ToProposeEnemy,
        ToProposeAlly,
        ToNextAction 
    }    

    public static class VotingEnumConverter
    {
        public static VotingKind GetKind(string kindString)
        {
            if (string.IsNullOrEmpty(kindString)) return VotingKind.Unknown;

            switch (kindString)
            {
                case "TO_CHANGE_AVATAR_BEHAVIOR": return VotingKind.ToChangeAvatarBehavior;
                case "TO_PROPOSE_ARCH_ENEMY": return VotingKind.ToProposeArchEnemy;
                case "TO_PROPOSE_ENEMY": return VotingKind.ToProposeEnemy;
                case "TO_PROPOSE_ALLY": return VotingKind.ToProposeAlly;
                default: return VotingKind.Unknown;
            }
        }

        public static BehaviorState GetBehaviorState(string behavior)
        {
            if (string.IsNullOrEmpty(behavior)) return BehaviorState.UNKNOWN;

            string lastWord = behavior.Split(' ').Last();

            return lastWord switch
            {
                "ATTACKING" => BehaviorState.ATTACKING,
                "DEFENSIVE" => BehaviorState.DEFENSIVE,
                "EXPANDING" => BehaviorState.EXPANDING,
                "ECONOMY" => BehaviorState.ECONOMY,
                _ => BehaviorState.UNKNOWN
            };
        }
    }
}