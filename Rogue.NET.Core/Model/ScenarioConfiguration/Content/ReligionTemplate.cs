using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using System;
using System.Collections.Generic;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Content
{
    [Serializable]
    public class ReligionTemplate : DungeonObjectTemplate
    {
        bool _hasBonusAttribute;
        bool _hasBonusAttackAttribute;
        CharacterAttribute _bonusAttribute;
        AttackAttributeTemplate _bonusAttackAttribute;

        public bool HasAttributeBonus
        {
            get { return _hasBonusAttribute; }
            set
            {
                if (_hasBonusAttribute != value)
                {
                    _hasBonusAttribute = value;
                    OnPropertyChanged("HasAttributeBonus");
                }
            }
        }
        public bool HasBonusAttackAttribute
        {
            get { return _hasBonusAttackAttribute; }
            set
            {
                if (_hasBonusAttackAttribute != value)
                {
                    _hasBonusAttackAttribute = value;
                    OnPropertyChanged("HasBonusAttackAttribute");
                }
            }
        }
        public CharacterAttribute BonusAttribute
        {
            get { return _bonusAttribute; }
            set
            {
                if (_bonusAttribute != value)
                {
                    _bonusAttribute = value;
                    OnPropertyChanged("BonusAttribute");
                }
            }
        }
        public AttackAttributeTemplate BonusAttackAttribute
        {
            get { return _bonusAttackAttribute; }
            set
            {
                if (_bonusAttackAttribute != value)
                {
                    _bonusAttackAttribute = value;
                    OnPropertyChanged("BonusAttackAttribute");
                }
            }
        }

        public List<ReligiousAffiliationAttackParametersTemplate> AttackParameters { get; set; }

        public ReligionTemplate()
        {
            this.AttackParameters = new List<ReligiousAffiliationAttackParametersTemplate>();
        }
    }
}
