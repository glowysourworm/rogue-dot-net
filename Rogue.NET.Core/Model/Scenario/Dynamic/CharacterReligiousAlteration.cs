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

        public AlterationEffect AttackAttributeEffect
        {
            get { return (_religion != null && _religion.HasBonusAttackAttributes) ? _religion.AttackAttributeAlteration : null; }
        }

        public AlterationEffect AttributeEffect
        {
            get { return (_religion != null && _religion.HasAttributeBonus) ? _religion.AttributeAlteration : null; }
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
