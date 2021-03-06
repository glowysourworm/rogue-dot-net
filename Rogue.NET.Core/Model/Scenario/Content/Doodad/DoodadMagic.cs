﻿using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content.Skill;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Doodad;
using System;

namespace Rogue.NET.Core.Model.Scenario.Content.Doodad
{
    [Serializable]
    public class DoodadMagic : DoodadBase
    {
        public bool IsAutomatic { get; set; }
        public bool IsInvoked { get; set; }

        public DoodadAlterationTemplate AutomaticAlteration { get; set; }
        public DoodadAlterationTemplate InvokedAlteration { get; set; }

        public bool HasCharacterClassRequirement { get; set; }
        public string CharacterClass { get; set; }

        public DoodadMagic() : base()
        {
            this.Type = DoodadType.Magic;
        }
    }
}
