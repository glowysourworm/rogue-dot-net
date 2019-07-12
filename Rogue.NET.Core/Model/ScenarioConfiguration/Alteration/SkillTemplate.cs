using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using Rogue.NET.Core.Model.ScenarioConfiguration.Content;
using System;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Alteration
{
    [Serializable]
    public class SkillTemplate : Template
    {
        int _levelRequirement;
        int _pointRequirement;
        bool _hasReligionRequirement;
        bool _hasAttributeRequirement;
        double _attributeLevelRequirement;
        CharacterAttribute _attributeRequirement;
        ReligionTemplate _religion;
        SpellTemplate _alteration;

        public int LevelRequirement
        {
            get { return _levelRequirement; }
            set
            {
                if (_levelRequirement != value)
                {
                    _levelRequirement = value;
                    OnPropertyChanged("LevelRequirement");
                }
            }
        }
        public int PointRequirement
        {
            get { return _pointRequirement; }
            set
            {
                if (_pointRequirement != value)
                {
                    _pointRequirement = value;
                    OnPropertyChanged("PointRequirement");
                }
            }
        }
        public bool HasReligionRequirement
        {
            get { return _hasReligionRequirement; }
            set
            {
                if (_hasReligionRequirement != value)
                {
                    _hasReligionRequirement = value;
                    OnPropertyChanged("HasReligionRequirement");
                }
            }
        }
        public bool HasAttributeRequirement
        {
            get { return _hasAttributeRequirement; }
            set
            {
                if (_hasAttributeRequirement != value)
                {
                    _hasAttributeRequirement = value;
                    OnPropertyChanged("HasAttributeRequirement");
                }
            }
        }
        public double AttributeLevelRequirement
        {
            get { return _attributeLevelRequirement; }
            set
            {
                if (_attributeLevelRequirement != value)
                {
                    _attributeLevelRequirement = value;
                    OnPropertyChanged("AttributeLevelRequirement");
                }
            }
        }
        public CharacterAttribute AttributeRequirement
        {
            get { return _attributeRequirement; }
            set
            {
                if (_attributeRequirement != value)
                {
                    _attributeRequirement = value;
                    OnPropertyChanged("AttributeRequirement");
                }
            }
        }
        public ReligionTemplate Religion
        {
            get { return _religion; }
            set
            {
                if (_religion != value)
                {
                    _religion = value;
                    OnPropertyChanged("Religion");
                }
            }
        }
        public SpellTemplate Alteration
        {
            get { return _alteration; }
            set
            {
                if (_alteration != value)
                {
                    _alteration = value;
                    OnPropertyChanged("Alteration");
                }
            }
        }

        public SkillTemplate()
        {
            this.Alteration = new SpellTemplate();
            this.AttributeRequirement = CharacterAttribute.Agility;
            this.Religion = new ReligionTemplate();
        }
    }
}
