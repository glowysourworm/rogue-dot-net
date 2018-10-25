using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Model.Scenario.Content.Item;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Rogue.NET.Core.Model.Scenario.Character
{
    [Serializable]
    public abstract class Character : ScenarioObject
    {
        protected const double HP_REGEN_BASE_MULTIPLIER = 0.01;
        protected const double MP_REGEN_BASE_MULTIPLIER = 0.01;

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
        public double HpRegenBase { get { return HP_REGEN_BASE_MULTIPLIER * this.StrengthBase; } }
        public double MpRegenBase { get { return MP_REGEN_BASE_MULTIPLIER * this.IntelligenceBase; } }
        public double AuraRadiusBase { get; set; }
        public double Hp { get; set; }
        public double Mp { get; set; }

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

            // TODO: Add this to the scenario editor / generator
            this.SpeedBase = ModelConstants.MAX_SPEED;
        }
    }
}