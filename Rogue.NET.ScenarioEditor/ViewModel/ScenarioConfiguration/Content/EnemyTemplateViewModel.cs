using System.Collections.ObjectModel;

using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Animation;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Content
{
    public class EnemyTemplateViewModel : CharacterTemplateViewModel
    {
        public ObservableCollection<AttackAttributeTemplateViewModel> AttackAttributes { get; set; }
        public ObservableCollection<AnimationTemplateViewModel> DeathAnimations { get; set; }

        private bool _generateOnStep;
        private bool _isInvisible;
        private bool _hasReligiousAffiliation;
        private RangeViewModel<double> _experienceGiven;
        private BehaviorDetailsTemplateViewModel _behaviorDetails;
        private ReligiousAffiliationTemplateViewModel _religiousAffiliation;

        public bool HasReligiousAffiliation
        {
            get { return _hasReligiousAffiliation; }
            set { this.RaiseAndSetIfChanged(ref _hasReligiousAffiliation, value); }
        }
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
        public ReligiousAffiliationTemplateViewModel ReligiousAffiliation
        {
            get { return _religiousAffiliation; }
            set { this.RaiseAndSetIfChanged(ref _religiousAffiliation, value); }
        }

        public EnemyTemplateViewModel()
        {
            this.HasReligiousAffiliation = false;
            this.GenerateOnStep = true;
            this.ExperienceGiven = new RangeViewModel<double>(0, 0, 100, 100000);
            this.BehaviorDetails = new BehaviorDetailsTemplateViewModel();
            this.AttackAttributes = new ObservableCollection<AttackAttributeTemplateViewModel>();
            this.DeathAnimations = new ObservableCollection<AnimationTemplateViewModel>();
            this.ReligiousAffiliation = new ReligiousAffiliationTemplateViewModel();
        }
        public EnemyTemplateViewModel(DungeonObjectTemplateViewModel template) : base(template)
        {
            this.HasReligiousAffiliation = false;
            this.GenerateOnStep = true;
            this.ExperienceGiven = new RangeViewModel<double>(0, 0, 100, 100000);
            this.BehaviorDetails = new BehaviorDetailsTemplateViewModel();
            this.AttackAttributes = new ObservableCollection<AttackAttributeTemplateViewModel>();
            this.DeathAnimations = new ObservableCollection<AnimationTemplateViewModel>();
            this.ReligiousAffiliation = new ReligiousAffiliationTemplateViewModel();
        }
    }
}
