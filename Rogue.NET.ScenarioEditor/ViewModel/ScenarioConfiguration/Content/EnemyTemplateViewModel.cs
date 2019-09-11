﻿using System.Collections.ObjectModel;

using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Animation;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Content
{
    public class EnemyTemplateViewModel : CharacterTemplateViewModel
    {
        public ObservableCollection<AnimationTemplateViewModel> DeathAnimations { get; set; }

        private bool _generateOnStep;
        private RangeViewModel<double> _experienceGiven;
        private BehaviorDetailsTemplateViewModel _behaviorDetails;
        private AnimationGroupTemplateViewModel _deathAnimationGroup;

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
        public BehaviorDetailsTemplateViewModel BehaviorDetails
        {
            get { return _behaviorDetails; }
            set { this.RaiseAndSetIfChanged(ref _behaviorDetails, value); }
        }
        public AnimationGroupTemplateViewModel DeathAnimationGroup
        {
            get { return _deathAnimationGroup; }
            set { this.RaiseAndSetIfChanged(ref _deathAnimationGroup, value); }
        }

        public EnemyTemplateViewModel()
        {
            this.GenerateOnStep = true;
            this.ExperienceGiven = new RangeViewModel<double>(0, 0, 100, 100000);
            this.BehaviorDetails = new BehaviorDetailsTemplateViewModel();
            this.DeathAnimations = new ObservableCollection<AnimationTemplateViewModel>();
            this.DeathAnimationGroup = new AnimationGroupTemplateViewModel();
        }
    }
}
