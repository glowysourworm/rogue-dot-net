using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.ViewModel.Attribute;
using Rogue.NET.ScenarioEditor.Views.Assets.SharedControl.AnimationControl;
using System;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Animation
{
    [UIType(DisplayName = "Projectile Animation",
            Description = "Animation type that creates a single particle projectile towards affected Characters",
            ViewType = typeof(AnimationProjectileParameters),
            BaseType = UITypeAttributeBaseType.Animation)]
    public class AnimationProjectileTemplateViewModel : AnimationEllipseBaseTemplateViewModel
    {
        private int _repeatCount;
        private int _animationTime;
        private bool _autoReverse;
        private bool _reverse;
        private int _erradicity;

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
        public bool Reverse
        {
            get { return _reverse; }
            set { this.RaiseAndSetIfChanged(ref _reverse, value); }
        }
        public int Erradicity
        {
            get { return _erradicity; }
            set { this.RaiseAndSetIfChanged(ref _erradicity, value); }
        }

        public AnimationProjectileTemplateViewModel()
        {
            this.AnimationTime = 1000;
            this.AutoReverse = false;
            this.Reverse = false;
            this.Erradicity = 1;
            this.RepeatCount = 1;
        }
    }
}
