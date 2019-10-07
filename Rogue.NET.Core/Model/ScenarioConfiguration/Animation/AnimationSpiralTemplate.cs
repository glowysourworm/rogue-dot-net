using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using System;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Animation
{
    [Serializable]
    public class AnimationSpiralTemplate : AnimationEllipseBaseTemplate
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
            set
            {
                if (_clockwise != value)
                {
                    _clockwise = value;
                    OnPropertyChanged("Clockwise");
                }
            }
        }
        public int AnimationTime
        {
            get { return _animationTime; }
            set
            {
                if (_animationTime != value)
                {
                    _animationTime = value;
                    OnPropertyChanged("AnimationTime");
                }
            }
        }
        public int ChildCount
        {
            get { return _childCount; }
            set
            {
                if (_childCount != value)
                {
                    _childCount = value;
                    OnPropertyChanged("ChildCount");
                }
            }
        }
        public int Erradicity
        {
            get { return _erradicity; }
            set
            {
                if (_erradicity != value)
                {
                    _erradicity = value;
                    OnPropertyChanged("Erradicity");
                }
            }
        }
        public int Radius
        {
            get { return _radius; }
            set
            {
                if (_radius != value)
                {
                    _radius = value;
                    OnPropertyChanged("Radius");
                }
            }
        }
        public int RadiusChangeRate
        {
            get { return _radiusChangeRate; }
            set
            {
                if (_radiusChangeRate != value)
                {
                    _radiusChangeRate = value;
                    OnPropertyChanged("Radius");
                }
            }
        }
        public double RotationRate
        {
            get { return _rotationRate; }
            set
            {
                if (_rotationRate != value)
                {
                    _rotationRate = value;
                    OnPropertyChanged("RotationRate");
                }
            }
        }

        public AnimationSpiralTemplate()
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
