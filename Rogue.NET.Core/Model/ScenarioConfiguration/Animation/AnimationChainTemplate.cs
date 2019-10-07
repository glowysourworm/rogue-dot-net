using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using System;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Animation
{
    [Serializable]
    public class AnimationChainTemplate : AnimationEllipseBaseTemplate
    {
        private int _repeatCount;
        private int _animationTime;
        private bool _autoReverse;
        private bool _reverse;
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

        public AnimationChainTemplate()
        {
            this.AnimationTime = 1000;
            this.AutoReverse = false;
            this.Reverse = false;
            this.Erradicity = 1;
            this.RepeatCount = 1;
        }
    }
}
