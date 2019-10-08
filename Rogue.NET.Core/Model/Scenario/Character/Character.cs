using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Model.Scenario.Content.Item;
using Rogue.NET.Core.Model.Scenario.Dynamic.Alteration;

using System;
using System.Linq;
using System.Collections.Generic;
using Rogue.NET.Core.Model.Scenario.Alteration.Common;

namespace Rogue.NET.Core.Model.Scenario.Character
{
    [Serializable]
    public abstract class Character : ScenarioObject
    {
        public Dictionary<string, Equipment> Equipment { get; set; }
        public Dictionary<string, Consumable> Consumables { get; set; }     

        /// <summary>
        /// Returns inventory as a single collection (avoid repeated use)
        /// </summary>
        public Dictionary<string, ItemBase> Inventory
        {
            get
            {
                return this.Equipment
                           .Values
                           .Cast<ItemBase>()
                           .Union(this.Consumables.Values)
                           .ToDictionary(x => x.Id);
            }
        }

        public double HpMax { get; set; }
        public double MpMax { get; set; }
        public double StrengthBase { get; set; }
        public double IntelligenceBase { get; set; }
        public double AgilityBase { get; set; }
        public double SpeedBase { get; set; }
        public double HpRegenBase { get; set; }
        public double MpRegenBase { get; set; }
        public double LightRadiusBase { get; set; }
        public double Hp { get; set; }
        public double Mp { get; set; }

        public CharacterAlteration Alteration { get; set; }

        public Dictionary<string, AttackAttribute> AttackAttributes { get; set; }

        public Character()
        {
            this.Alteration = new CharacterAlteration();
            this.Equipment = new Dictionary<string, Equipment>();
            this.Consumables = new Dictionary<string, Consumable>();
            this.AttackAttributes = new Dictionary<string, AttackAttribute>();
        }
    }
}