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
        Religion _religion;
        double _religiousAffiliationLevel;

        // Keep these instantiated for pass-through usage.
        AlterationEffect _attributeEffect = new AlterationEffect();
        AlterationEffect _attackAttributeEffect = new AlterationEffect();

        public bool IsAffiliated()
        {
            return _religion != null && _religiousAffiliationLevel > 0;
        }

        public IEnumerable<AnimationTemplate> Renounce(IEnumerable<AttackAttribute> scenarioAttributes)
        {
            var renunciationAnimations = _religion.RenunciationAnimations;

            _religion = null;

            CreateEffects(scenarioAttributes);

            return renunciationAnimations;
        }

        public void Affiliate(Religion religion, double affiliationLevel, IEnumerable<AttackAttribute> scenarioAttributes)
        {
            if (_religion != null)
                throw new Exception("Trying to affiliate religion without renouncing old religion");

            _religion = religion;
            _religiousAffiliationLevel = affiliationLevel;

            CreateEffects(scenarioAttributes);
        }

        public void SetAffiliationLevel(double affiliationLevel, IEnumerable<AttackAttribute> scenarioAttributes)
        {
            if (_religion == null)
                throw new Exception("Not affiliated with any religion");

            _religiousAffiliationLevel = affiliationLevel;

            CreateEffects(scenarioAttributes);
        }

        public string ReligionName
        {
            get { return _religion == null ? string.Empty : _religion.RogueName; }
        }

        public ScenarioImage Symbol
        {
            get { return _religion; }
        }

        public ReligiousAffiliationAttackParameters GetParameters(string enemyReligionName)
        {
            return _religion == null ? null :
                   _religion.AttackParameters.First(x => x.EnemyReligionName == enemyReligionName);
        }

        public AlterationEffect AttackAttributeEffect
        {
            get { return _attackAttributeEffect; }
        }

        public AlterationEffect AttributeEffect
        {
            get { return _attributeEffect; }
        }

        public bool HasAttributeEffect
        {
            get { return _religion?.HasAttributeBonus ?? false; }
        }

        public bool HasAttackAttributeEffect
        {
            get { return _religion?.HasBonusAttackAttributes ?? false; }
        }

        public double Affiliation
        {
            get { return _religiousAffiliationLevel; }
        }

        public CharacterReligiousAlteration()
        {

        }

        protected void CreateEffects(IEnumerable<AttackAttribute> scenarioAttributes)
        {
            var attributeEffect = _religion?.AttributeAlteration ?? new AlterationEffect();

            // Attribute Bonus Effect
            _attributeEffect = new AlterationEffect()
            {
                DisplayName = (_religion?.RogueName ?? "Zero") + " Attribute Bonus",

                Agility = attributeEffect.Agility * _religiousAffiliationLevel,
                Attack = attributeEffect.Attack * _religiousAffiliationLevel,
                AuraRadius = attributeEffect.AuraRadius * _religiousAffiliationLevel,
                CriticalHit = attributeEffect.CriticalHit * _religiousAffiliationLevel,
                Defense = attributeEffect.Defense * _religiousAffiliationLevel,
                DodgeProbability = attributeEffect.DodgeProbability * _religiousAffiliationLevel,
                FoodUsagePerTurn = attributeEffect.FoodUsagePerTurn * _religiousAffiliationLevel,
                HpPerStep = attributeEffect.HpPerStep * _religiousAffiliationLevel,
                Intelligence = attributeEffect.Intelligence * _religiousAffiliationLevel,
                MagicBlockProbability = attributeEffect.MagicBlockProbability * _religiousAffiliationLevel,
                MpPerStep = attributeEffect.MpPerStep * _religiousAffiliationLevel,
                Speed = attributeEffect.Speed * _religiousAffiliationLevel,
                Strength = attributeEffect.Strength * _religiousAffiliationLevel
            };

            var attackAttributes = _religion == null ? scenarioAttributes : _religion.AttackAttributeAlteration.AttackAttributes;

            // Attack Attribute Bonus Effect
            _attackAttributeEffect = new AlterationEffect()
            {
                DisplayName = (_religion?.RogueName ?? "Zero") + " Attack Attribute Bonus",

                AttackAttributes = attackAttributes.Select(x => new AttackAttribute()
                {
                    Attack = x.Attack * _religiousAffiliationLevel,
                    Resistance = x.Resistance * _religiousAffiliationLevel,
                    Weakness = x.Weakness * _religiousAffiliationLevel,

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
