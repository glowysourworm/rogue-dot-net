using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Model.Scenario.Content.Item;
using System;
using System.Linq;
using System.Collections.Generic;
using Rogue.NET.Core.Model.Scenario.Dynamic;

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
        public double AuraRadiusBase { get; set; }
        public double Hp { get; set; }
        public double Mp { get; set; }

        public abstract CharacterAlteration Alteration { get; set; }
        public virtual CharacterClassAlteration CharacterClassAlteration { get; set; }

        public Character() : base() 
        {
            Initialize();
        }
        public Character(string name, string symbol)
            : base(name, symbol, "#FFFFFFFF")
        {
            Initialize();
        }
        public Character(string name, SmileyMoods mood, string bodyColor, string lineColor, string auraColor)
            : base(name, mood, bodyColor, lineColor, auraColor)
        {
            Initialize();
        }

        private void Initialize()
        {
            this.Equipment = new Dictionary<string, Equipment>();
            this.Consumables = new Dictionary<string, Consumable>();
            this.CharacterClassAlteration = new CharacterClassAlteration();
        }
    }
}