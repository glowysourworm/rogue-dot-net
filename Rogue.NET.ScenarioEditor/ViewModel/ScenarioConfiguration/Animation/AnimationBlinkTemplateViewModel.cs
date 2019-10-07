using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.ViewModel.Attribute;
using Rogue.NET.ScenarioEditor.Views.Assets.SharedControl.AnimationControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Animation
{
    [UIType(DisplayName = "Blink Animation",
            Description = "Animation type that creates screen-sized timed figure that can fade in or out",
            ViewType = typeof(AnimationBlinkParameters),
            BaseType = UITypeAttributeBaseType.Animation)]
    public class AnimationBlinkTemplateViewModel : AnimationBaseTemplateViewModel
    {
        int _animationTime;
        int _repeatCount;
        bool _autoReverse;
        double _opacity1;
        double _opacity2;

        public int AnimationTime
        {
            get { return _animationTime; }
            set { this.RaiseAndSetIfChanged(ref _animationTime, value); }
        }
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
        public double Opacity1
        {
            get { return _opacity1; }
            set { this.RaiseAndSetIfChanged(ref _opacity1, value); }
        }
        public double Opacity2
        {
            get { return _opacity2; }
            set { this.RaiseAndSetIfChanged(ref _opacity2, value); }
        }

        public AnimationBlinkTemplateViewModel()
        {
            this.AnimationTime = 1000;
            this.AutoReverse = false;
            this.RepeatCount = 1;
            this.PointTargetType = AnimationPointTargetType.Source;
            this.Opacity1 = 1;
            this.Opacity2 = 0;
        }
    }
}
