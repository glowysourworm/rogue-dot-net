using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using System;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Animation
{
    [Serializable]
    public class AnimationBarrageTemplate : AnimationEllipseBaseTemplate
    {
        private bool _reverse;
        private int _animationTime;
        private int _childCount;
        private int _erradicity;
        private int _radius;

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

        public AnimationBarrageTemplate()
        {
            this.Reverse = false;
            this.AnimationTime = 1000;
            this.ChildCount = 5;
            this.Erradicity = 1;
            this.Radius = 20;
        }
    }
}
