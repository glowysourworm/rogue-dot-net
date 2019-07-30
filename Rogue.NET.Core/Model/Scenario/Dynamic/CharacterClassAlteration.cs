using Rogue.NET.Core.Model.Scenario.Alteration;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Model.ScenarioConfiguration.Animation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rogue.NET.Core.Model.Scenario.Dynamic
{
    [Serializable]
    public class CharacterClassAlteration
    {
        public AlterationEffect AttackAttributeEffect { get; protected set; }
        public AlterationEffect AttributeEffect { get; protected set; }

        public CharacterClass CharacterClass { get; protected set; }

        protected IEnumerable<AttackAttribute> ScenarioAttackAttributes { get; set; }

        public ScenarioImage Symbol
        {
            get { return this.CharacterClass; }
        }

        public bool HasAttributeEffect
        {
            get { return this.CharacterClass?.HasAttributeBonus ?? false; }
        }

        public bool HasAttackAttributeEffect
        {
            get { return this.CharacterClass?.HasBonusAttackAttributes ?? false; }
        }

        public bool HasCharacterClass()
        {
            return this.CharacterClass != null;
        }

        public CharacterClassAlteration()
        {
            this.CharacterClass = null;
            this.ScenarioAttackAttributes = null;
        }

        public void Initialize(CharacterClass characterClass, IEnumerable<AttackAttribute> scenarioAttributes)
        {
            this.CharacterClass = characterClass;
            this.ScenarioAttackAttributes = scenarioAttributes;

            CreateEffects();
        }

        protected void CreateEffects()
        {
            var attributeEffect = this.CharacterClass?.AttributeAlteration ?? new AlterationEffect();

            // Attribute Bonus Effect
            this.AttributeEffect = new AlterationEffect()
            {
                DisplayName = (this.CharacterClass?.RogueName ?? "Zero") + " Attribute Bonus",

                Agility = attributeEffect.Agility,
                Attack = attributeEffect.Attack,
                AuraRadius = attributeEffect.AuraRadius,
                CriticalHit = attributeEffect.CriticalHit,
                Defense = attributeEffect.Defense,
                DodgeProbability = attributeEffect.DodgeProbability,
                FoodUsagePerTurn = attributeEffect.FoodUsagePerTurn,
                HpPerStep = attributeEffect.HpPerStep,
                Intelligence = attributeEffect.Intelligence,
                MagicBlockProbability = attributeEffect.MagicBlockProbability,
                MpPerStep = attributeEffect.MpPerStep,
                Speed = attributeEffect.Speed,
                Strength = attributeEffect.Strength
            };

            var attackAttributes = this.CharacterClass == null ? this.ScenarioAttackAttributes : 
                                                           this.CharacterClass.AttackAttributeAlteration.AttackAttributes;

            // Attack Attribute Bonus Effect
            this.AttackAttributeEffect = new AlterationEffect()
            {
                DisplayName = (this.CharacterClass?.RogueName ?? "Zero") + " Attack Attribute Bonus",

                AttackAttributes = attackAttributes.Select(x => new AttackAttribute()
                {
                    Attack = x.Attack,
                    Resistance = x.Resistance,
                    Weakness = x.Weakness,

                    CharacterColor = x.CharacterColor,
                    CharacterSymbol = x.CharacterSymbol,
                    DisplayIcon = x.DisplayIcon,
                    Icon = x.Icon,
                    SmileyAuraColor = x.SmileyAuraColor,
                    SmileyBodyColor = x.SmileyBodyColor,
                    SmileyLineColor = x.SmileyLineColor,
                    SmileyMood = x.SmileyMood,
                    SymbolType = x.SymbolType,
                    RogueName = x.RogueName

                }).ToList()
            };
        }
    }
}
