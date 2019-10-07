using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using System;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Animation
{
    [Serializable]
    public class AnimationProjectileConstantVelocityTemplate : AnimationEllipseBaseTemplate
    {
        private int _repeatCount;
        private bool _autoReverse;
        private bool _reverse;
        private int _velocity;
        private int _erradicity;

        public int RepeatCount
        {
            get { return _repeatCount; }
            set
            {
                if (_repeatCount != value)
                {
                    _repeatCount = value;
                    OnPropertyChanged("RepeatCount");
                }
            }
        }
        public bool AutoReverse
        {
            get { return _autoReverse; }
            set
            {
                if (_autoReverse != value)
                {
                    _autoReverse = value;
                    OnPropertyChanged("AutoReverse");
                }
            }
        }
        public bool Reverse
        {
            get { return _reverse; }
            set
            {
                if (_reverse != value)
                {
                    _reverse = value;
                    OnPropertyChanged("Reverse");
                }
            }
        }
        public int Velocity
        {
            get { return _velocity; }
            set
            {
                if (_velocity != value)
                {
                    _velocity = value;
                    OnPropertyChanged("Velocity");
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

        public AnimationProjectileConstantVelocityTemplate()
        {
            this.FillTemplate = new BrushTemplate();
            this.AutoReverse = false;
            this.Erradicity = 1;
            this.RepeatCount = 1;
            this.Velocity = 50;
        }
    }
}
