using Rogue.NET.Core.Model.Enums;
using Rogue.NET.ScenarioEditor.ViewModel.Attribute;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration;
using Rogue.NET.ScenarioEditor.Views.Assets;
using System.Collections.ObjectModel;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Content
{
    [UIType(DisplayName = "Player Class",
            Description = "Type of character that your player will be - each has its own special attributes, items, and skills",
            ViewType = typeof(Player))]
    public class PlayerTemplateViewModel : CharacterTemplateViewModel
    {
        private string _class;
        private RangeViewModel<double> _foodUsage;
        private ObservableCollection<SkillSetTemplateViewModel> _skills;

        public string Class
        {
            get { return _class; }
            set { this.RaiseAndSetIfChanged(ref _class, value); }
        }
        public RangeViewModel<double> FoodUsage
        {
            get { return _foodUsage; }
            set { this.RaiseAndSetIfChanged(ref _foodUsage, value); }
        }
        public ObservableCollection<SkillSetTemplateViewModel> Skills
        {
            get { return _skills; }
            set { this.RaiseAndSetIfChanged(ref _skills, value); }
        }

        public PlayerTemplateViewModel()
        {
            this.Skills = new ObservableCollection<SkillSetTemplateViewModel>();
            this.FoodUsage = new RangeViewModel<double>(0.005, 0.01);
            this.Class = "Fighter";
            this.SymbolDetails.SymbolType = SymbolType.Smiley;
        }
    }
}
