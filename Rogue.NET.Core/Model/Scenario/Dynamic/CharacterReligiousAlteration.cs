using Rogue.NET.Core.Model.Scenario.Alteration;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Model.Scenario.Content.Religion;
using Rogue.NET.Core.Model.ScenarioConfiguration.Animation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Model.Scenario.Dynamic
{
    [Serializable]
    public class CharacterReligiousAlteration
    {
        public AlterationEffect AttackAttributeEffect { get; protected set; }
        public AlterationEffect AttributeEffect { get; protected set; }

        protected Religion Religion { get; set; }
        protected double ReligiousAffiliationLevel { get; set; }

        protected IEnumerable<AttackAttribute> ScenarioAttackAttributes { get; set; }

        public bool IsAffiliated()
        {
            return this.Religion != null && this.ReligiousAffiliationLevel > 0;
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

        public void Affiliate(Religion religion, double affiliationLevel)
        {
            if (this.Religion != null)
                throw new Exception("Trying to affiliate religion without renouncing old religion");

            this.Religion = religion;
            this.ReligiousAffiliationLevel = affiliationLevel;

            CreateEffects();
        }

        public void SetAffiliationLevel(double affiliationLevel)
        {
            if (this.Religion == null)
                throw new Exception("Not affiliated with any religion");

            this.ReligiousAffiliationLevel = affiliationLevel;

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

        public ReligiousAffiliationAttackParameters GetParameters(string enemyReligionName)
        {
            return this.Religion?.AttackParameters.First(x => x.EnemyReligionName == enemyReligionName);
        }

        public bool HasAttributeEffect
        {
            get { return this.Religion?.HasAttributeBonus ?? false; }
        }

        public bool HasAttackAttributeEffect
        {
            get { return this.Religion?.HasBonusAttackAttributes ?? false; }
        }

        public double Affiliation
        {
            get { return this.ReligiousAffiliationLevel; }
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

                Agility = attributeEffect.Agility * this.ReligiousAffiliationLevel,
                Attack = attributeEffect.Attack * this.ReligiousAffiliationLevel,
                AuraRadius = attributeEffect.AuraRadius * this.ReligiousAffiliationLevel,
                CriticalHit = attributeEffect.CriticalHit * this.ReligiousAffiliationLevel,
                Defense = attributeEffect.Defense * this.ReligiousAffiliationLevel,
                DodgeProbability = attributeEffect.DodgeProbability * this.ReligiousAffiliationLevel,
                FoodUsagePerTurn = attributeEffect.FoodUsagePerTurn * this.ReligiousAffiliationLevel,
                HpPerStep = attributeEffect.HpPerStep * this.ReligiousAffiliationLevel,
                Intelligence = attributeEffect.Intelligence * this.ReligiousAffiliationLevel,
                MagicBlockProbability = attributeEffect.MagicBlockProbability * this.ReligiousAffiliationLevel,
                MpPerStep = attributeEffect.MpPerStep * this.ReligiousAffiliationLevel,
                Speed = attributeEffect.Speed * this.ReligiousAffiliationLevel,
                Strength = attributeEffect.Strength * this.ReligiousAffiliationLevel
            };

            var attackAttributes = this.Religion == null ? this.ScenarioAttackAttributes : 
                                                           this.Religion.AttackAttributeAlteration.AttackAttributes;

            // Attack Attribute Bonus Effect
            this.AttackAttributeEffect = new AlterationEffect()
            {
                DisplayName = (this.Religion?.RogueName ?? "Zero") + " Attack Attribute Bonus",

                AttackAttributes = attackAttributes.Select(x => new AttackAttribute()
                {
                    Attack = x.Attack * this.ReligiousAffiliationLevel,
                    Resistance = x.Resistance * this.ReligiousAffiliationLevel,
                    Weakness = (int)Math.Ceiling(x.Weakness * this.ReligiousAffiliationLevel),

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
