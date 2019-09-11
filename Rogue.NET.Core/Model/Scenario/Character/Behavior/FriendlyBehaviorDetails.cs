using Rogue.NET.Core.Model.Enums;
using System;

namespace Rogue.NET.Core.Model.Scenario.Character.Behavior
{
    [Serializable]
    public class FriendlyBehaviorDetails : BehaviorDetails
    {
        public override Behavior DefaultBehavior { get; }

        public FriendlyBehaviorDetails()
        {
            this.DefaultBehavior = new FriendlyBehavior()
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
