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
        bool _hasAttributeRequirement;
        bool _hasCharacterClassRequirement;
        double _attributeLevelRequirement;
        CharacterAttribute _attributeRequirement;
        SpellTemplate _alteration;
        CharacterClassTemplate _characterClass;

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
        public bool HasCharacterClassRequirement
        {
            get { return _hasCharacterClassRequirement; }
            set
            {
                if (_hasCharacterClassRequirement != value)
                {
                    _hasCharacterClassRequirement = value;
                    OnPropertyChanged("HasCharacterClassRequirement");
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
        public CharacterClassTemplate CharacterClass
        {
            get { return _characterClass; }
            set
            {
                if (_characterClass != value)
                {
                    _characterClass = value;
                    OnPropertyChanged("CharacterClass");
                }
            }
        }

        public SkillTemplate()
        {
            this.Alteration = new SpellTemplate();
            this.AttributeRequirement = CharacterAttribute.Agility;
            this.HasCharacterClassRequirement = false;
            this.CharacterClass = new CharacterClassTemplate();
        }
    }
}
