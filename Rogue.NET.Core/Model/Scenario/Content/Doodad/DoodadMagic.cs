using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content.Skill;
using System;

namespace Rogue.NET.Core.Model.Scenario.Content.Doodad
{
    [Serializable]
    public class DoodadMagic : DoodadBase
    {
        public Spell AutomaticSpell { get; set; }
        public Spell InvokedSpell { get; set; }
        public bool IsAutomatic { get; set; }
        public bool IsInvoked { get; set; }

        public bool HasReligionRequirement { get; set; }
        public string ReligionName { get; set; }

        public DoodadMagic() : base()
        {
            this.Type = DoodadType.Magic;
            this.AutomaticSpell = new Spell();
            this.InvokedSpell = new Spell();
            this.ReligionName = null;
        }
        public DoodadMagic(string name, Spell autoSpell, Spell invokeSpell, bool isauto, bool isinvoke, ImageResources icon)
            : base(name, icon)
        {
            this.Type = DoodadType.Magic;
            this.AutomaticSpell = autoSpell;
            this.InvokedSpell = invokeSpell;
            this.IsAutomatic = isauto;
            this.IsInvoked = isinvoke;
            this.ReligionName = null;
        }
    }
}
