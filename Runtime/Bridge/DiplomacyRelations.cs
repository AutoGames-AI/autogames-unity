// ReSharper disable InconsistentNaming

using System.Collections.Generic;

namespace QAI.Dtos
{
    public record DiplomacyRelations
    {
        public int id;
        public int archEnemyId;
        public List<int> enemyIds;
        public List<int> alliesIds;
        public BehaviorState relationState;
    }

    public enum BehaviorState
    {
        ATTACKING,
        DEFENSIVE,
        EXPANDING,
        ECONOMY,
        UNKNOWN
    }
}
