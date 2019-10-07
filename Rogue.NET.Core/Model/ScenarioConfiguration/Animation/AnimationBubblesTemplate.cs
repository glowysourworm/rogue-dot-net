using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using System;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Animation
{
    [Serializable]
    public class AnimationBubblesTemplate : AnimationEllipseBaseTemplate
    {
        private int _animationTime;
        private int _childCount;
        private int _erradicity;
        private int _radius;
        private int _roamRadius;

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
        public int RoamRadius
        {
            get { return _roamRadius; }
            set
            {
                if (_roamRadius != value)
                {
                    _roamRadius = value;
                    OnPropertyChanged("RoamRadius");
                }
            }
        }

        public AnimationBubblesTemplate()
        {
            this.AnimationTime = 1000;
            this.ChildCount = 5;
            this.Erradicity = 1;
            this.Radius = 20;
            this.RoamRadius = 20;
        }
    }
}
