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
        private bool _hasReligion;
        private RangeViewModel<double> _experienceGiven;
        private RangeViewModel<double> _religiousAffiliationLevel;
        private BehaviorDetailsTemplateViewModel _behaviorDetails;
        private ReligionTemplateViewModel _religion;

        public bool HasReligion
        {
            get { return _hasReligion; }
            set { this.RaiseAndSetIfChanged(ref _hasReligion, value); }
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
        public RangeViewModel<double> ReligiousAffiliationLevel
        {
            get { return _religiousAffiliationLevel; }
            set { this.RaiseAndSetIfChanged(ref _religiousAffiliationLevel, value); }
        }
        public BehaviorDetailsTemplateViewModel BehaviorDetails
        {
            get { return _behaviorDetails; }
            set { this.RaiseAndSetIfChanged(ref _behaviorDetails, value); }
        }
        public ReligionTemplateViewModel Religion
        {
            get { return _religion; }
            set { this.RaiseAndSetIfChanged(ref _religion, value); }
        }

        public EnemyTemplateViewModel()
        {
            this.HasReligion = false;
            this.GenerateOnStep = true;
            this.ExperienceGiven = new RangeViewModel<double>(0, 0, 100, 100000);
            this.ReligiousAffiliationLevel = new RangeViewModel<double>(0.01, 0.03, 0.05, 1);
            this.BehaviorDetails = new BehaviorDetailsTemplateViewModel();
            this.AttackAttributes = new ObservableCollection<AttackAttributeTemplateViewModel>();
            this.DeathAnimations = new ObservableCollection<AnimationTemplateViewModel>();
            this.Religion = new ReligionTemplateViewModel();
        }
        public EnemyTemplateViewModel(DungeonObjectTemplateViewModel template) : base(template)
        {
            this.HasReligion = false;
            this.GenerateOnStep = true;
            this.ExperienceGiven = new RangeViewModel<double>(0, 0, 100, 100000);
            this.ReligiousAffiliationLevel = new RangeViewModel<double>(0.01, 0.03, 0.05, 1);
            this.BehaviorDetails = new BehaviorDetailsTemplateViewModel();
            this.AttackAttributes = new ObservableCollection<AttackAttributeTemplateViewModel>();
            this.DeathAnimations = new ObservableCollection<AnimationTemplateViewModel>();
            this.Religion = new ReligionTemplateViewModel();
        }
    }
}
