using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.ViewModel.Attribute;
using Rogue.NET.ScenarioEditor.Views.Assets.SharedControl.AnimationControl;
using System;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Animation
{
    [UIType(DisplayName = "Projectile Animation (constant velocity)",
            Description = "Animation type that creates a single particle projectile towards affected Characters",
            ViewType = typeof(AnimationProjectileConstantVelocityParameters),
            BaseType = UITypeAttributeBaseType.Animation)]
    public class AnimationProjectileConstantVelocityTemplateViewModel : AnimationEllipseBaseTemplateViewModel
    {
        private int _repeatCount;
        private bool _autoReverse;
        private bool _reverse;
        private int _velocity;
        private int _erradicity;

        public int RepeatCount
        {
            get { return _repeatCount; }
            set { this.RaiseAndSetIfChanged(ref _repeatCount, value); }
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
        public int Velocity
        {
            get { return _velocity; }
            set { this.RaiseAndSetIfChanged(ref _velocity, value); }
        }
        public int Erradicity
        {
            get { return _erradicity; }
            set { this.RaiseAndSetIfChanged(ref _erradicity, value); }
        }

        public AnimationProjectileConstantVelocityTemplateViewModel()
        {
            this.AutoReverse = false;
            this.Reverse = false;
            this.Erradicity = 1;
            this.RepeatCount = 1;
            this.Velocity = 50;
        }
    }
}
