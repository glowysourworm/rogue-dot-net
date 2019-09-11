using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration;
using System;

namespace Rogue.NET.Core.Model.Scenario.Character.Behavior
{
    [Serializable]
    public abstract class Behavior
    {
        public BehaviorCondition BehaviorCondition { get; set; }
        public BehaviorExitCondition BehaviorExitCondition { get; set; }

        /// <summary>
        /// This is the maximum number of turns that this particular behavior state can
        /// be applied - but doesn't act unless the entry / exit conditions specify.
        /// </summary>
        public int BehaviorTurnCounter { get; set; }

        public CharacterMovementType MovementType { get; set; }
        public CharacterAttackType AttackType { get; set; }

        public abstract AlterationCostTemplate SkillAlterationCost { get; }

        public Behavior()
        {
            
        }
    }
}
