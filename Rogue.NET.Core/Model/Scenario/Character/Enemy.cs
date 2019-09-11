using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Character.Behavior;
using System;

namespace Rogue.NET.Core.Model.Scenario.Character
{
    [Serializable]
    public class Enemy : NonPlayerCharacter
    {
        public override BehaviorDetails BehaviorDetails { get; set; }
        public double ExperienceGiven { get; set; }

        public Enemy() : base()
        {
            this.AlignmentType = CharacterAlignmentType.EnemyAligned;
            this.BehaviorDetails = new EnemyBehaviorDetails();
        }
    }
}
