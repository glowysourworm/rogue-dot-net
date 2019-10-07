using Rogue.NET.Core.Model.Enums;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Animation
{
    public class AnimationSequenceTemplateViewModel : TemplateViewModel
    {
        AlterationTargetType _targetType;
        bool _darkenBackground;

        public AlterationTargetType TargetType
        {
            get { return _targetType; }
            set { this.RaiseAndSetIfChanged(ref _targetType, value); }
        }
        public bool DarkenBackground
        {
            get { return _darkenBackground; }
            set { this.RaiseAndSetIfChanged(ref _darkenBackground, value); }
        }
        public ObservableCollection<AnimationBaseTemplateViewModel> Animations { get; set; }

        public AnimationSequenceTemplateViewModel()
        {
            this.Animations = new ObservableCollection<AnimationBaseTemplateViewModel>();
            this.TargetType = AlterationTargetType.Source;
            this.DarkenBackground = false;
        }
    }
}
