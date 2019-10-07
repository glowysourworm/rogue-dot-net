using Rogue.NET.ScenarioEditor.ViewModel.Attribute;
using Rogue.NET.ScenarioEditor.Views.Assets.SharedControl.AnimationControl;
using System;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Animation
{
    [UIType(DisplayName = "Aura Animation",
            Description = "Animation type that modulates an ellipse around a Character",
            ViewType = typeof(AnimationAuraParameters),
            BaseType = UITypeAttributeBaseType.Animation)]
    public class AnimationAuraTemplateViewModel : AnimationEllipseBaseTemplateViewModel
    {
        private int _repeatCount;
        private int _animationTime;
        private bool _autoReverse;

        public int RepeatCount
        {
            get { return _repeatCount; }
            set { this.RaiseAndSetIfChanged(ref _repeatCount, value); }
        }
        public int AnimationTime
        {
            get { return _animationTime; }
            set { this.RaiseAndSetIfChanged(ref _animationTime, value); }
        }
        public bool AutoReverse
        {
            get { return _autoReverse; }
            set { this.RaiseAndSetIfChanged(ref _autoReverse, value); }
        }

        public AnimationAuraTemplateViewModel()
        {
            this.AnimationTime = 1000;
            this.AutoReverse = false;
            this.RepeatCount = 1;
        }
    }
}
