using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Equipment;
using System;
using System.Collections.Generic;

namespace Rogue.NET.Core.Model.Scenario.Content.Item
{
    [Serializable]
    public class Equipment : ItemBase
    {
        public EquipmentAttackAlterationTemplate AttackAlteration { get; set; }
        public EquipmentEquipAlterationTemplate EquipAlteration { get; set; }
        public EquipmentCurseAlterationTemplate CurseAlteration { get; set; }
        public bool HasAttackAlteration { get; set; }
        public bool HasEquipAlteration { get; set; }
        public bool HasCurseAlteration { get; set; }

        public bool IsEquipped { get; set; }
        public bool IsCursed { get; set; }

        public int Class { get; set; }
        public double Quality { get; set; }
        public EquipmentType Type { get; set; }
        public CharacterBaseAttribute CombatType { get; set; }

        public string AmmoName { get; set; }

        public IList<AttackAttribute> AttackAttributes { get; set; }

        public Equipment()
        {
            this.IsEquipped = false;
            this.Type = EquipmentType.Armor;
            this.CombatType = CharacterBaseAttribute.Strength;
            this.AmmoName = "";
            this.AttackAttributes = new List<AttackAttribute>();
        }
    }
}
