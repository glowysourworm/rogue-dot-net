using System.Collections.ObjectModel;

using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Content
{
    public class EnemyTemplateViewModel : CharacterTemplateViewModel
    {
        public ObservableCollection<AttackAttributeTemplateViewModel> AttackAttributes { get; set; }

        private bool _generateOnStep;
        private bool _isInvisible;
        private RangeViewModel<double> _experienceGiven;
        private BehaviorDetailsTemplateViewModel _behaviorDetails;

        public bool GenerateOnStep
        {
            get { return _generateOnStep; }
            set { this.RaiseAndSetIfChanged(ref _generateOnStep, value); }
        }
        public bool IsInvisible
        {
            get { return _isInvisible; }
            set { this.RaiseAndSetIfChanged(ref _isInvisible, value); }
        }
        public RangeViewModel<double> ExperienceGiven
        {
            get { return _experienceGiven; }
            set { this.RaiseAndSetIfChanged(ref _experienceGiven, value); }
        }
        public BehaviorDetailsTemplateViewModel BehaviorDetails
        {
            get { return _behaviorDetails; }
            set { this.RaiseAndSetIfChanged(ref _behaviorDetails, value); }
        }

        public EnemyTemplateViewModel()
        {
            this.GenerateOnStep = true;
            this.ExperienceGiven = new RangeViewModel<double>(0, 0, 100, 100000);
            this.BehaviorDetails = new BehaviorDetailsTemplateViewModel();
            this.AttackAttributes = new ObservableCollection<AttackAttributeTemplateViewModel>();
        }
        public EnemyTemplateViewModel(DungeonObjectTemplateViewModel template) : base(template)
        {
            this.GenerateOnStep = true;
            this.ExperienceGiven = new RangeViewModel<double>(0, 0, 100, 100000);
            this.BehaviorDetails = new BehaviorDetailsTemplateViewModel();
            this.AttackAttributes = new ObservableCollection<AttackAttributeTemplateViewModel>();
        }
    }
}
