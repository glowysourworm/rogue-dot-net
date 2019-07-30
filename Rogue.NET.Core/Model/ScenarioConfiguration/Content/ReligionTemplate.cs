using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using Rogue.NET.Core.Model.ScenarioConfiguration.Animation;
using System;
using System.Collections.Generic;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Content
{
    [Serializable]
    public class ReligionTemplate : DungeonObjectTemplate
    {
        bool _hasBonusAttribute;
        bool _hasBonusAttackAttributes;
        bool _allowsRenunciation;
        bool _allowsReAffiliation;
        bool _isIdentified;
        bool _canStartWith;
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
        public bool AllowsRenunciation
        {
            get { return _allowsRenunciation; }
            set
            {
                if (_allowsRenunciation != value)
                {
                    _allowsRenunciation = value;
                    OnPropertyChanged("AllowsRenunciation");
                }
            }
        }
        public bool AllowsReAffiliation
        {
            get { return _allowsReAffiliation; }
            set
            {
                if (_allowsReAffiliation != value)
                {
                    _allowsReAffiliation = value;
                    OnPropertyChanged("AllowsReAffiliation");
                }
            }
        }
        public bool IsIdentified
        {
            get { return _isIdentified; }
            set
            {
                if (_isIdentified != value)
                {
                    _isIdentified = value;
                    OnPropertyChanged("IsIdentified");
                }
            }
        }
        public bool CanStartWith
        {
            get { return _canStartWith; }
            set
            {
                if (_canStartWith != value)
                {
                    _canStartWith = value;
                    OnPropertyChanged("CanStartWith");
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
        public List<AnimationTemplate> RenunciationAnimations { get; set; }

        public ReligionTemplate()
        {
            this.BonusAttackAttributes = new List<AttackAttributeTemplate>();
            this.RenunciationAnimations = new List<AnimationTemplate>();

            this.AllowsRenunciation = true;
            this.AllowsReAffiliation = false;
            this.HasBonusAttackAttributes = false;
        }
    }
}
