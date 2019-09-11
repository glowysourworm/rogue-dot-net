using Rogue.NET.Core.Model.Enums;
using System;

namespace Rogue.NET.Core.Model.Scenario.Character.Behavior
{
    [Serializable]
    public class TemporaryCharacterBehaviorDetails : BehaviorDetails
    {
        public override Behavior DefaultBehavior { get; }

        public TemporaryCharacterBehaviorDetails()
        {
            this.DefaultBehavior = new TemporaryCharacterBehavior()
            {
                AttackType = CharacterAttackType.Melee,
                BehaviorCondition = BehaviorCondition.AttackConditionsMet,
                BehaviorExitCondition = BehaviorExitCondition.BehaviorCounterExpired,
                BehaviorTurnCounter = 1,
                MovementType = CharacterMovementType.HeatSeeker
            };
        }
    }
}
