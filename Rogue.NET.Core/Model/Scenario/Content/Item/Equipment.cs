using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration;
using Rogue.NET.Core.Model.Scenario.Content.Skill;
using System;
using System.Collections.Generic;

namespace Rogue.NET.Core.Model.Scenario.Content.Item
{
    [Serializable]
    public class Equipment : ItemBase
    {
        public int Class { get; set; }
        public double Quality { get; set; }
        public EquipmentType Type { get; set; }
        public CharacterBaseAttribute CombatType { get; set; }

        public bool IsEquipped { get; set; }
        public bool IsCursed { get; set; }

        public bool HasEquipSpell { get; set; }
        public bool HasCurseSpell { get; set; }
        public Spell EquipSpell { get; set; }
        public Spell CurseSpell { get; set; }
        public string AmmoName { get; set; }

        public IList<AttackAttribute> AttackAttributes { get; set; }

        public Equipment() : base()
        {
            this.IsEquipped = false;
            this.Type = EquipmentType.Armor;
            this.CombatType = CharacterBaseAttribute.Strength;
            this.EquipSpell = new Spell();
            this.CurseSpell = new Spell();
            this.AmmoName = "";
            this.AttackAttributes = new List<AttackAttribute>();
        }
        public Equipment(EquipmentType type, string name, ImageResources icon) : base(name, icon)
        {
            this.Type = type;
            this.CombatType = CharacterBaseAttribute.Strength;
            this.IsEquipped = false;
            this.EquipSpell = new Spell();
            this.CurseSpell = new Spell();
            this.AmmoName = "";
            this.AttackAttributes = new List<AttackAttribute>();
        }
    }
}
