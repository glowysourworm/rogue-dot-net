using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration;
using System;

namespace Rogue.NET.Core.Model.Scenario.Character.Behavior
{
    [Serializable]
    public class Behavior
    {
        /// <summary>
        /// This is the default behavior for any Rogue.NET character. It can be used for when
        /// a behavior is undefined or no behavior has entry conditions met. (Must be overridden in inherited class)
        /// </summary>
        public static Behavior Default = new Behavior()
        {
            AttackType = CharacterAttackType.Melee,
            BehaviorCondition = BehaviorCondition.AttackConditionsMet,
            BehaviorExitCondition = BehaviorExitCondition.BehaviorCounterExpired,
            BehaviorTurnCounter = 1,
            MovementType = CharacterMovementType.HeatSeeker            
        };

        public BehaviorCondition BehaviorCondition { get; set; }
        public BehaviorExitCondition BehaviorExitCondition { get; set; }

        /// <summary>
        /// This is the maximum number of turns that this particular behavior state can
        /// be applied - but doesn't act unless the entry / exit conditions specify.
        /// </summary>
        public int BehaviorTurnCounter { get; set; }

        public CharacterMovementType MovementType { get; set; }
        public CharacterAttackType AttackType { get; set; }

        public AlterationTemplate SkillAlteration { get; set; }

        public Behavior()
        {
            
        }
    }
}
