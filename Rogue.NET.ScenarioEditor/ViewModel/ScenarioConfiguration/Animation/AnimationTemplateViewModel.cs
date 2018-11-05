using Rogue.NET.Core.Model.Enums;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using System;

using ReactiveUI;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Animation
{
    public class AnimationTemplateViewModel : TemplateViewModel
    {
        private int _repeatCount;
        private int _animationTime;
        private bool _autoReverse;
        private bool _constantVelocity;
        private double _accelerationRatio;
        private AnimationType _type;
        private BrushTemplateViewModel _fillTemplate;
        private BrushTemplateViewModel _strokeTemplate;
        private double _strokeThickness;
        private double _opacity1;
        private double _opacity2;
        private double _height1;
        private double _height2;
        private double _width1;
        private double _width2;
        private int _velocity;
        private int _childCount;
        private int _erradicity;
        private double _radiusFromFocus;
        private double _spiralRate;
        private double _roamRadius;

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
        public bool ConstantVelocity
        {
            get { return _constantVelocity; }
            set { this.RaiseAndSetIfChanged(ref _constantVelocity, value); }
        }
        public double AccelerationRatio
        {
            get { return _accelerationRatio; }
            set { this.RaiseAndSetIfChanged(ref _accelerationRatio, value); }
        }
        public AnimationType Type
        {
            get { return _type; }
            set { this.RaiseAndSetIfChanged(ref _type, value); }
        }
        public BrushTemplateViewModel FillTemplate
        {
            get { return _fillTemplate; }
            set { this.RaiseAndSetIfChanged(ref _fillTemplate, value); }
        }
        public BrushTemplateViewModel StrokeTemplate
        {
            get { return _strokeTemplate; }
            set { this.RaiseAndSetIfChanged(ref _strokeTemplate, value); }
        }
        public double StrokeThickness
        {
            get { return _strokeThickness; }
            set { this.RaiseAndSetIfChanged(ref _strokeThickness, value); }
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
        public double Height1
        {
            get { return _height1; }
            set { this.RaiseAndSetIfChanged(ref _height1, value); }
        }
        public double Height2
        {
            get { return _height2; }
            set { this.RaiseAndSetIfChanged(ref _height2, value); }
        }
        public double Width1
        {
            get { return _width1; }
            set { this.RaiseAndSetIfChanged(ref _width1, value); }
        }
        public double Width2
        {
            get { return _width2; }
            set { this.RaiseAndSetIfChanged(ref _width2, value); }
        }
        public int Velocity
        {
            get { return _velocity; }
            set { this.RaiseAndSetIfChanged(ref _velocity, value); }
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
        public double RadiusFromFocus
        {
            get { return _radiusFromFocus; }
            set { this.RaiseAndSetIfChanged(ref _radiusFromFocus, value); }
        }
        public double SpiralRate
        {
            get { return _spiralRate; }
            set { this.RaiseAndSetIfChanged(ref _spiralRate, value); }
        }
        public double RoamRadius
        {
            get { return _roamRadius; }
            set { this.RaiseAndSetIfChanged(ref _roamRadius, value); }
        }

        //Constructors
        public AnimationTemplateViewModel()
        {
            this.StrokeTemplate = new BrushTemplateViewModel();
            this.FillTemplate = new BrushTemplateViewModel();
            this.AccelerationRatio = 1;
            this.AnimationTime = 1000;
            this.AutoReverse = false;
            this.ChildCount = 5;
            this.ConstantVelocity = false;
            this.Erradicity = 1;
            this.Height1 = 4;
            this.Height2 = 4;
            this.Opacity1 = 1;
            this.Opacity2 = 1;
            this.RadiusFromFocus = 20;
            this.RepeatCount = 1;
            this.RoamRadius = 20;
            this.SpiralRate = 10;
            this.StrokeThickness = 1;
            this.Type = AnimationType.ProjectileSelfToTarget;
            this.Velocity = 50;
            this.Width1 = 4;
            this.Width2 = 4;
        }
    }
}
