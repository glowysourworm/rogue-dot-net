using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.ViewModel.Attribute;
using Rogue.NET.ScenarioEditor.Views.Assets.SharedControl.AnimationControl;
using System;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Animation
{
    [UIType(DisplayName = "Bubbles Animation",
            Description = "Animation type that creates a flurry of particles centered around a Character",
            ViewType = typeof(AnimationBubblesParameters),
            BaseType = UITypeAttributeBaseType.Animation)]
    public class AnimationBubblesTemplateViewModel : AnimationEllipseBaseTemplateViewModel
    {
        private int _animationTime;
        private int _childCount;
        private int _erradicity;
        private int _radius;
        private int _roamRadius;

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
        public int RoamRadius
        {
            get { return _roamRadius; }
            set { this.RaiseAndSetIfChanged(ref _roamRadius, value); }
        }

        public AnimationBubblesTemplateViewModel()
        {
            this.AnimationTime = 1000;
            this.ChildCount = 5;
            this.Erradicity = 1;
            this.Radius = 20;
            this.RoamRadius = 20;
        }
    }
}
