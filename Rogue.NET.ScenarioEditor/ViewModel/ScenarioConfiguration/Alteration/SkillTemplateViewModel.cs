using Rogue.NET.Core.Model.Enums;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Content;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration
{
    public class SkillTemplateViewModel : TemplateViewModel
    {
        int _levelRequirement;
        int _pointRequirement;
        bool _hasAttributeRequirement;
        bool _hasCharacterClassRequirement;
        double _attributeLevelRequirement;
        CharacterAttribute _attributeRequirement;
        SpellTemplateViewModel _alteration;
        CharacterClassTemplateViewModel _characterClass;

        public int LevelRequirement
        {
            get { return _levelRequirement; }
            set { this.RaiseAndSetIfChanged(ref _levelRequirement, value); }
        }
        public int PointRequirement
        {
            get { return _pointRequirement; }
            set { this.RaiseAndSetIfChanged(ref _pointRequirement, value); }
        }
        public bool HasAttributeRequirement
        {
            get { return _hasAttributeRequirement; }
            set { this.RaiseAndSetIfChanged(ref _hasAttributeRequirement, value); }
        }
        public bool HasCharacterClassRequirement
        {
            get { return _hasCharacterClassRequirement; }
            set { this.RaiseAndSetIfChanged(ref _hasCharacterClassRequirement, value); }
        }
        public double AttributeLevelRequirement
        {
            get { return _attributeLevelRequirement; }
            set { this.RaiseAndSetIfChanged(ref _attributeLevelRequirement, value); }
        }
        public CharacterAttribute AttributeRequirement
        {
            get { return _attributeRequirement; }
            set { this.RaiseAndSetIfChanged(ref _attributeRequirement, value); }
        }
        public SpellTemplateViewModel Alteration
        {
            get { return _alteration; }
            set { this.RaiseAndSetIfChanged(ref _alteration, value); }
        }
        public CharacterClassTemplateViewModel CharacterClass
        {
            get { return _characterClass; }
            set { this.RaiseAndSetIfChanged(ref _characterClass, value); }
        }

        public SkillTemplateViewModel()
        {
            this.Alteration = new SpellTemplateViewModel();
            this.AttributeRequirement = CharacterAttribute.Agility;
            this.HasCharacterClassRequirement = false;
            this.CharacterClass = new CharacterClassTemplateViewModel();
        }
    }
}
