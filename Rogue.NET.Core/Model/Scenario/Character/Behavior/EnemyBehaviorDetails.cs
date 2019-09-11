using Rogue.NET.Core.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Model.Scenario.Character.Behavior
{
    [Serializable]
    public class EnemyBehaviorDetails : BehaviorDetails
    {
        public override Behavior DefaultBehavior { get; }

        public EnemyBehaviorDetails()
        {
            this.DefaultBehavior = new EnemyBehavior()
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
