using Rogue.NET.Core.Model.Enums;
using Rogue.NET.ScenarioEditor.ViewModel.Attribute;
using Rogue.NET.ScenarioEditor.Views;
using Rogue.NET.ScenarioEditor.Views.Assets;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Content
{
    [UIType(DisplayName = "Enemy",
            Description = "Character that is opposed to the player - which the player can attack",
            ViewType = typeof(Enemy))]
    public class EnemyTemplateViewModel : NonPlayerCharacterTemplateViewModel
    {
        private bool _generateOnStep;
        private RangeViewModel<double> _experienceGiven;

        public bool GenerateOnStep
        {
            get { return _generateOnStep; }
            set { this.RaiseAndSetIfChanged(ref _generateOnStep, value); }
        }
        public RangeViewModel<double> ExperienceGiven
        {
            get { return _experienceGiven; }
            set { this.RaiseAndSetIfChanged(ref _experienceGiven, value); }
        }

        public EnemyTemplateViewModel() : base()
        {
            this.GenerateOnStep = true;
            this.ExperienceGiven = new RangeViewModel<double>(0, 100);
            this.AlignmentType = CharacterAlignmentType.EnemyAligned;
        }
    }
}
