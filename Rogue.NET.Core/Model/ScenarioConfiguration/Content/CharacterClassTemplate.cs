using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using Rogue.NET.Core.Model.ScenarioConfiguration.Animation;
using System;
using System.Collections.Generic;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Content
{
    [Serializable]
    public class CharacterClassTemplate : DungeonObjectTemplate
    {
        bool _hasBonusAttribute;
        bool _hasBonusAttackAttributes;
        double _bonusAttributeValue;
        CharacterAttribute _bonusAttribute;

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
        public bool HasBonusAttackAttributes
        {
            get { return _hasBonusAttackAttributes; }
            set
            {
                if (_hasBonusAttackAttributes != value)
                {
                    _hasBonusAttackAttributes = value;
                    OnPropertyChanged("HasBonusAttackAttribute");
                }
            }
        }
        public double BonusAttributeValue
        {
            get { return _bonusAttributeValue; }
            set
            {
                if (_bonusAttributeValue != value)
                {
                    _bonusAttributeValue = value;
                    OnPropertyChanged("BonusAttributeValue");
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

        public List<AttackAttributeTemplate> BonusAttackAttributes { get; set; }

        public CharacterClassTemplate()
        {
            this.BonusAttackAttributes = new List<AttackAttributeTemplate>();
            this.HasBonusAttackAttributes = false;
        }
    }
}
