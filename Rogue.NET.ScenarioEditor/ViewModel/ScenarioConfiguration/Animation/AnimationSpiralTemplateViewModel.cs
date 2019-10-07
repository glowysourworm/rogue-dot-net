using Rogue.NET.ScenarioEditor.ViewModel.Attribute;
using Rogue.NET.ScenarioEditor.Views.Assets.SharedControl.AnimationControl;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Animation
{
    [UIType(DisplayName = "Spiral Animation",
            Description = "Animation type that creates a spiral of particles focused around affected Characters",
            ViewType = typeof(AnimationSpiralParameters),
            BaseType = UITypeAttributeBaseType.Animation)]
    public class AnimationSpiralTemplateViewModel : AnimationEllipseBaseTemplateViewModel
    {
        bool _clockwise;
        private int _animationTime;
        private int _childCount;
        private int _erradicity;
        private int _radius;
        private int _radiusChangeRate;
        private double _rotationRate;

        public bool Clockwise
        {
            get { return _clockwise; }
            set { this.RaiseAndSetIfChanged(ref _clockwise, value); }
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
        public int RadiusChangeRate
        {
            get { return _radiusChangeRate; }
            set { this.RaiseAndSetIfChanged(ref _radiusChangeRate, value); }
        }
        public double RotationRate
        {
            get { return _rotationRate; }
            set { this.RaiseAndSetIfChanged(ref _rotationRate, value); }
        }

        public AnimationSpiralTemplateViewModel()
        {
            this.Clockwise = false;
            this.AnimationTime = 1000;
            this.ChildCount = 5;
            this.Erradicity = 1;
            this.Radius = 20;
            this.RadiusChangeRate = 0;
            this.RotationRate = 1.0;
        }
    }
}
