using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Character.Behavior;
using System;

namespace Rogue.NET.Core.Model.Scenario.Character
{
    /// <summary>
    /// Special type of unit that only lasts for a set number of turns. Temporary Characters are removed when
    /// Level is unloaded - THEY ARE NOT PERSISTED IN PLAYER PARTY.
    /// </summary>
    [Serializable]
    public class TemporaryCharacter : NonPlayerCharacter
    {
        /// <summary>
        /// Counter that specifies when Summoned Unit will be removed from the level content
        /// </summary>
        public int LifetimeCounter { get; set; }
        public override BehaviorDetails BehaviorDetails { get; set; }

        public TemporaryCharacter() : base()
        {
            this.BehaviorDetails = new TemporaryCharacterBehaviorDetails();
        }
    }
}
