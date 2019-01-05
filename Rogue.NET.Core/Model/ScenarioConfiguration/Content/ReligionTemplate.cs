using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using System;
using System.Collections.Generic;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Content
{
    [Serializable]
    public class ReligionTemplate : DungeonObjectTemplate
    {
        string _followerName;
        bool _hasBonusAttribute;
        bool _hasBonusAttackAttribute;
        bool _allowsRenunciation;
        bool _allowsReAffiliation;
        double _bonusAttributeValue;
        CharacterAttribute _bonusAttribute;
        AttackAttributeTemplate _bonusAttackAttribute;

        public string FollowerName
        {
            get { return _followerName; }
            set
            {
                if (_followerName != value)
                {
                    _followerName = value;
                    OnPropertyChanged("FollowerName");
                }
            }
        }
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
            this.FollowerName = "Christian";

            this.AttackParameters = new List<ReligiousAffiliationAttackParametersTemplate>();

            this.AllowsRenunciation = true;
            this.AllowsReAffiliation = false;
        }
    }
}
