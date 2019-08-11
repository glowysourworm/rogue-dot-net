using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Model.Scenario.Content.Item;
using Rogue.NET.Core.Model.Scenario.Dynamic.Alteration;

using System;
using System.Linq;
using System.Collections.Generic;

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
        public double HpRegenBase { get { return ModelConstants.HpRegenBaseMultiplier * this.StrengthBase; } }
        public double MpRegenBase { get { return ModelConstants.MpRegenBaseMultiplier * this.IntelligenceBase; } }
        public double LightRadiusBase { get; set; }
        public double Hp { get; set; }
        public double Mp { get; set; }

        public CharacterAlteration Alteration { get; set; }

        public Character() : base() 
        {
            Initialize();
        }
        public Character(string name, string symbol)
            : base(name, symbol, "#FFFFFFFF")
        {
            Initialize();
        }
        private void Initialize()
        {
            this.Alteration = new CharacterAlteration();
            this.Equipment = new Dictionary<string, Equipment>();
            this.Consumables = new Dictionary<string, Consumable>();
        }
    }
}