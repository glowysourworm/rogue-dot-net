using Rogue.NET.Core.Model.Scenario.Alteration;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Model.ScenarioConfiguration.Animation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rogue.NET.Core.Model.Scenario.Dynamic
{
    [Serializable]
    public class CharacterReligiousAlteration
    {
        public AlterationEffect AttackAttributeEffect { get; protected set; }
        public AlterationEffect AttributeEffect { get; protected set; }

        protected Religion Religion { get; set; }

        protected IEnumerable<AttackAttribute> ScenarioAttackAttributes { get; set; }

        public bool IsAffiliated()
        {
            return this.Religion != null;
        }

        public bool CanRenounce()
        {
            return this.Religion?.AllowsRenunciation ?? false;
        }

        public IEnumerable<AnimationTemplate> Renounce()
        {
            var renunciationAnimations = this.Religion.RenunciationAnimations;

            this.Religion = null;

            CreateEffects();

            return renunciationAnimations;
        }

        public void Affiliate(Religion religion)
        {
            if (this.Religion != null)
                throw new Exception("Trying to affiliate religion without renouncing old religion");

            this.Religion = religion;

            CreateEffects();
        }

        public void SetAffiliationLevel(double affiliationLevel)
        {
            if (this.Religion == null)
                throw new Exception("Not affiliated with any religion");

            CreateEffects();
        }

        public string ReligionName
        {
            get { return this.Religion?.RogueName ?? string.Empty; }
        }

        public ScenarioImage Symbol
        {
            get { return this.Religion; }
        }

        public bool HasAttributeEffect
        {
            get { return this.Religion?.HasAttributeBonus ?? false; }
        }

        public bool HasAttackAttributeEffect
        {
            get { return this.Religion?.HasBonusAttackAttributes ?? false; }
        }

        public CharacterReligiousAlteration()
        {

        }

        /// <summary>
        /// Must be called immediately after instantiating.
        /// </summary>
        public void Initialize(IEnumerable<AttackAttribute> scenarioAttributes)
        {
            this.ScenarioAttackAttributes = scenarioAttributes;

            CreateEffects();
        }

        protected void CreateEffects()
        {
            var attributeEffect = this.Religion?.AttributeAlteration ?? new AlterationEffect();

            // Attribute Bonus Effect
            this.AttributeEffect = new AlterationEffect()
            {
                DisplayName = (this.Religion?.RogueName ?? "Zero") + " Attribute Bonus",

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

            var attackAttributes = this.Religion == null ? this.ScenarioAttackAttributes : 
                                                           this.Religion.AttackAttributeAlteration.AttackAttributes;

            // Attack Attribute Bonus Effect
            this.AttackAttributeEffect = new AlterationEffect()
            {
                DisplayName = (this.Religion?.RogueName ?? "Zero") + " Attack Attribute Bonus",

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
