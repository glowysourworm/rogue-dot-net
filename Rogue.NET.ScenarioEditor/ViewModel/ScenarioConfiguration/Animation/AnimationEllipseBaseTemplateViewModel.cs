using Rogue.NET.Core.Model.Enums;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Animation
{
    public class AnimationEllipseBaseTemplateViewModel : AnimationBaseTemplateViewModel
    {
        private AnimationEasingType _easingType;
        private double _easingAmount;
        private double _opacity1;
        private double _opacity2;
        private int _height1;
        private int _height2;
        private int _width1;
        private int _width2;

        public AnimationEasingType EasingType
        {
            get { return _easingType; }
            set { this.RaiseAndSetIfChanged(ref _easingType, value); }
        }
        public double EasingAmount
        {
            get { return _easingAmount; }
            set { this.RaiseAndSetIfChanged(ref _easingAmount, value); }
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
        public int Height1
        {
            get { return _height1; }
            set { this.RaiseAndSetIfChanged(ref _height1, value); }
        }
        public int Height2
        {
            get { return _height2; }
            set { this.RaiseAndSetIfChanged(ref _height2, value); }
        }
        public int Width1
        {
            get { return _width1; }
            set { this.RaiseAndSetIfChanged(ref _width1, value); }
        }
        public int Width2
        {
            get { return _width2; }
            set { this.RaiseAndSetIfChanged(ref _width2, value); }
        }

        public AnimationEllipseBaseTemplateViewModel()
        {
            this.EasingType = AnimationEasingType.None;
            this.EasingAmount = 0.0;
            this.Height1 = 4;
            this.Height2 = 4;
            this.Opacity1 = 1;
            this.Opacity2 = 1;
            this.Width1 = 4;
            this.Width2 = 4;
        }
    }
}
