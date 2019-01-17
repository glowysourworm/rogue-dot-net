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

        public bool IsAffiliated()
        {
            return _religion != null && _religiousAffiliationLevel > 0;
        }

        public IEnumerable<AnimationTemplate> Renounce()
        {
            var renunciationAnimations = _religion.RenunciationAnimations;

            _religion = null;

            return renunciationAnimations;
        }

        public void Affiliate(Religion religion, double affiliationLevel)
        {
            if (_religion != null)
                throw new Exception("Trying to affiliate religion without renouncing old religion");

            _religion = religion;
            _religiousAffiliationLevel = affiliationLevel;
        }

        public void SetAffiliationLevel(double affiliationLevel)
        {
            if (_religion == null)
                throw new Exception("Not affiliated with any religion");

            _religiousAffiliationLevel = affiliationLevel;
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

        /// <summary>
        /// Treated as an attack attribute passive effect
        /// </summary>
        public AlterationEffect AttackAttributeEffect
        {
            get
            {
                if (_religion == null || !_religion.HasBonusAttackAttributes)
                    return null;

                // Return effect scaled by affiliation
                return new AlterationEffect()
                {
                    AttackAttributes = _religion.AttackAttributeAlteration.AttackAttributes.Select(x => new AttackAttribute()
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

        /// <summary>
        /// Treated as a passive effect
        /// </summary>
        public AlterationEffect AttributeEffect
        {
            get
            {
                if (_religion == null || !_religion.HasAttributeBonus)
                    return null;

                return new AlterationEffect()
                {
                    Agility = _religion.AttributeAlteration.Agility * _religiousAffiliationLevel,
                    Attack = _religion.AttributeAlteration.Attack * _religiousAffiliationLevel,
                    AuraRadius = _religion.AttributeAlteration.AuraRadius * _religiousAffiliationLevel,
                    CriticalHit = _religion.AttributeAlteration.CriticalHit * _religiousAffiliationLevel,
                    Defense = _religion.AttributeAlteration.Defense * _religiousAffiliationLevel,
                    DodgeProbability = _religion.AttributeAlteration.DodgeProbability * _religiousAffiliationLevel,
                    FoodUsagePerTurn = _religion.AttributeAlteration.FoodUsagePerTurn * _religiousAffiliationLevel,
                    HpPerStep = _religion.AttributeAlteration.HpPerStep * _religiousAffiliationLevel,
                    Intelligence = _religion.AttributeAlteration.Intelligence * _religiousAffiliationLevel,
                    MagicBlockProbability = _religion.AttributeAlteration.MagicBlockProbability * _religiousAffiliationLevel,
                    MpPerStep = _religion.AttributeAlteration.MpPerStep * _religiousAffiliationLevel,
                    Speed = _religion.AttributeAlteration.Speed * _religiousAffiliationLevel,
                    Strength = _religion.AttributeAlteration.Strength * _religiousAffiliationLevel
                };
            }
        }

        public double Affiliation
        {
            get { return _religiousAffiliationLevel; }
        }

        public CharacterReligiousAlteration()
        {

        }
    }
}
