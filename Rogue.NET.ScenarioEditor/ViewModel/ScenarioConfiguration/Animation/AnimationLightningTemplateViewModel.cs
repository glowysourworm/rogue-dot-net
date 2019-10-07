using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.ViewModel.Attribute;
using Rogue.NET.ScenarioEditor.Views.Assets.SharedControl.AnimationControl;
using System;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Animation
{
    [UIType(DisplayName = "Lightning Animation",
            Description = "Animation type that creates a lightning bolt towards the affected Characters",
            ViewType = typeof(AnimationLightningParameters),
            BaseType = UITypeAttributeBaseType.Animation)]
    public class AnimationLightningTemplateViewModel : AnimationBaseTemplateViewModel
    {
        private int _animationTime;
        private int _incrementHeightLimit;
        private int _incrementWidthLimit;
        private int _holdEndTime;

        public int AnimationTime
        {
            get { return _animationTime; }
            set { this.RaiseAndSetIfChanged(ref _animationTime, value); }
        }
        public int IncrementHeightLimit
        {
            get { return _incrementHeightLimit; }
            set { this.RaiseAndSetIfChanged(ref _incrementHeightLimit, value); }
        }
        public int IncrementWidthLimit
        {
            get { return _incrementWidthLimit; }
            set { this.RaiseAndSetIfChanged(ref _incrementWidthLimit, value); }
        }
        public int HoldEndTime
        {
            get { return _holdEndTime; }
            set { this.RaiseAndSetIfChanged(ref _holdEndTime, value); }
        }

        public AnimationLightningTemplateViewModel()
        {
            this.AnimationTime = 1000;
            this.IncrementHeightLimit = 10;
            this.IncrementWidthLimit = 10;
            this.HoldEndTime = 750;
        }
    }
}
