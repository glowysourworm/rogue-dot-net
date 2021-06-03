using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Model.Scenario.Content.Item;
using Rogue.NET.Core.Model.Scenario.Dynamic.Alteration;

using System;
using System.Linq;
using System.Collections.Generic;
using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.Common.Collection;
using Rogue.NET.Common.Extension;

namespace Rogue.NET.Core.Model.Scenario.Character
{
    [Serializable]
    public abstract class CharacterBase : ScenarioObject
    {
        public SimpleDictionary<string, Equipment> Equipment { get; set; }
        public SimpleDictionary<string, Consumable> Consumables { get; set; }     

        /// <summary>
        /// Returns inventory as a single collection (avoid repeated use)
        /// </summary>
        public SimpleDictionary<string, ItemBase> Inventory
        {
            get
            {
                return this.Equipment
                           .Values
                           .Cast<ItemBase>()
                           .Union(this.Consumables.Values)
                           .ToSimpleDictionary(x => x.Id, x => x);
            }
        }

        public double HealthMax { get; set; }
        public double StaminaMax { get; set; }
        public double StrengthBase { get; set; }
        public double IntelligenceBase { get; set; }
        public double AgilityBase { get; set; }
        public double SpeedBase { get; set; }
        public double HealthRegenBase { get; set; }
        public double StaminaRegenBase { get; set; }
        public double VisionBase { get; set; }
        public double Health { get; set; }
        public double Stamina { get; set; }

        public CharacterAlteration Alteration { get; set; }

        public SimpleDictionary<string, AttackAttribute> AttackAttributes { get; set; }

        public CharacterBase()
        {
            this.Alteration = new CharacterAlteration();
            this.Equipment = new SimpleDictionary<string, Equipment>();
            this.Consumables = new SimpleDictionary<string, Consumable>();
            this.AttackAttributes = new SimpleDictionary<string, AttackAttribute>();
        }
    }
}