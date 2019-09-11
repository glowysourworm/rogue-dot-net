using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Character.Behavior;
using System;

namespace Rogue.NET.Core.Model.Scenario.Character
{
    [Serializable]
    public class Friendly : NonPlayerCharacter
    {
        /// <summary>
        /// If set to true friendly should move / attack with Player; and be moved between levels
        /// </summary>
        public bool InPlayerParty { get; set; }

        public Friendly() : base()
        {
            this.AlignmentType = CharacterAlignmentType.PlayerAligned;
        }
    }
}
