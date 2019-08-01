using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content.Skill;
using System;

namespace Rogue.NET.Core.Model.Scenario.Character
{
    [Serializable]
    public class Behavior
    {
        private static readonly Behavior _DEFAULT = new Behavior()
        {
            AttackType = CharacterAttackType.Melee,
            MovementType = CharacterMovementType.HeatSeeker,
            BehaviorCondition = BehaviorCondition.AttackConditionsMet,
            BehaviorExitCondition = BehaviorExitCondition.BehaviorCounterExpired,
            BehaviorTurnCounter = 1
        };

        /// <summary>
        /// This is the default behavior for any Rogue.NET enemy. It can be used for when
        /// a behavior is undefined or no behavior has entry conditions met.
        /// </summary>
        public static Behavior Default
        {
            get { return _DEFAULT; }
        }

        public BehaviorCondition BehaviorCondition { get; set; }
        public BehaviorExitCondition BehaviorExitCondition { get; set; }

        /// <summary>
        /// This is the maximum number of turns that this particular behavior state can
        /// be applied - but doesn't act unless the entry / exit conditions specify.
        /// </summary>
        public int BehaviorTurnCounter { get; set; }

        public CharacterMovementType MovementType { get; set; }
        public CharacterAttackType AttackType { get; set; }
        public Spell EnemySkill { get; set; }

        public Behavior()
        {
            this.EnemySkill = new Spell();
        }
    }
}
