using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Animation;
using Rogue.NET.Core.Model.Scenario.Character.Behavior;
using System;

namespace Rogue.NET.Core.Model.Scenario.Character
{
    /// <summary>
    /// Base class for all characters that aren't the Player - adds BehaviorDetails that are common to all other
    /// characters.
    /// </summary>
    [Serializable]
    public class NonPlayerCharacter : Character
    {
        public CharacterAlignmentType AlignmentType { get; set; }
        public BehaviorDetails BehaviorDetails { get; set; }
        public AnimationSequence DeathAnimation { get; set; }
        public double TurnCounter { get; set; }

        /// <summary>
        /// Flag to alert character to enemies if they're invisible or transmorgified
        /// </summary>
        public bool IsAlerted { get; set; }

        public NonPlayerCharacter() : base()
        {
            this.BehaviorDetails = new BehaviorDetails();
            this.DeathAnimation = new AnimationSequence();
            this.TurnCounter = 0;
        }
    }
}
