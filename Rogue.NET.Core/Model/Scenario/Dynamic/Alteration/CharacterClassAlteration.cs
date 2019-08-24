using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.Core.Model.Scenario.Alteration.Effect;
using Rogue.NET.Core.Model.Scenario.Content;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rogue.NET.Core.Model.Scenario.Dynamic.Alteration
{
    [Serializable]
    public class CharacterClassAlteration
    {
        public AttackAttributePassiveAlterationEffect AttackAttributeEffect { get; protected set; }
        public PassiveAlterationEffect AttributeEffect { get; protected set; }

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
            var attributeEffect = this.CharacterClass?.AttributeAlteration ?? new PassiveAlterationEffect();

            // Attribute Bonus Effect
            this.AttributeEffect = new PassiveAlterationEffect()
            {
                RogueName = (this.CharacterClass?.RogueName ?? "Character Class") + " Attribute Bonus",

                Agility = attributeEffect.Agility,
                Attack = attributeEffect.Attack,
                LightRadius = attributeEffect.LightRadius,
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
            this.AttackAttributeEffect = new AttackAttributePassiveAlterationEffect()
            {
                RogueName = (this.CharacterClass?.RogueName ?? "Character Class") + " Attack Attribute Bonus",

                AttackAttributes = attackAttributes.Select(x => new AttackAttribute()
                {
                    Attack = x.Attack,
                    Resistance = x.Resistance,
                    Weakness = x.Weakness,
                    
                    CharacterColor = x.CharacterColor,
                    CharacterSymbol = x.CharacterSymbol,
                    DisplayIcon = x.DisplayIcon,
                    Icon = x.Icon,
                    SmileyLightRadiusColor = x.SmileyLightRadiusColor,
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
