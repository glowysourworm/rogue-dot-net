using Rogue.NET.Core.Model.Enums;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Content;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration
{
    public class SkillTemplateViewModel : TemplateViewModel
    {
        int _levelRequirement;
        int _pointRequirement;
        bool _hasReligionRequirement;
        bool _hasAttributeRequirement;
        double _attributeLevelRequirement;
        CharacterAttribute _attributeRequirement;
        SpellTemplateViewModel _alteration;

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
        public bool HasReligionRequirement
        {
            get { return _hasReligionRequirement; }
            set { this.RaiseAndSetIfChanged(ref _hasReligionRequirement, value); }
        }
        public bool HasAttributeRequirement
        {
            get { return _hasAttributeRequirement; }
            set { this.RaiseAndSetIfChanged(ref _hasAttributeRequirement, value); }
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

        public SkillTemplateViewModel()
        {
            this.Alteration = new SpellTemplateViewModel();
            this.AttributeRequirement = CharacterAttribute.Agility;
        }
    }
}
