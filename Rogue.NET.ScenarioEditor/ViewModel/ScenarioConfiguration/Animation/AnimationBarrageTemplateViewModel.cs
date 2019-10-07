using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.ViewModel.Attribute;
using Rogue.NET.ScenarioEditor.Views.Assets.SharedControl.AnimationControl;
using System;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Animation
{
    [UIType(DisplayName = "Barrage Animation",
            Description = "Animation type that creates a storm of particles focused towards a Character",
            ViewType = typeof(AnimationBarrageParameters),
            BaseType = UITypeAttributeBaseType.Animation)]
    public class AnimationBarrageTemplateViewModel : AnimationEllipseBaseTemplateViewModel
    {
        private bool _reverse;
        private int _animationTime;
        private int _childCount;
        private int _erradicity;
        private int _radius;

        public bool Reverse
        {
            get { return _reverse; }
            set { this.RaiseAndSetIfChanged(ref _reverse, value); }
        }
        public int AnimationTime
        {
            get { return _animationTime; }
            set { this.RaiseAndSetIfChanged(ref _animationTime, value); }
        }
        public int ChildCount
        {
            get { return _childCount; }
            set { this.RaiseAndSetIfChanged(ref _childCount, value); }
        }
        public int Erradicity
        {
            get { return _erradicity; }
            set { this.RaiseAndSetIfChanged(ref _erradicity, value); }
        }
        public int Radius
        {
            get { return _radius; }
            set { this.RaiseAndSetIfChanged(ref _radius, value); }
        }

        public AnimationBarrageTemplateViewModel()
        {
            this.Reverse = false;
            this.AnimationTime = 1000;
            this.ChildCount = 5;
            this.Erradicity = 1;
            this.Radius = 20;
        }
    }
}
