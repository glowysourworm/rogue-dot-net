using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using System;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Animation
{
    [Serializable]
    public class AnimationLightningTemplate : AnimationBaseTemplate
    {
        private int _animationTime;
        private int _incrementHeightLimit;
        private int _incrementWidthLimit;
        private int _holdEndTime;

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
        public int IncrementHeightLimit
        {
            get { return _incrementHeightLimit; }
            set
            {
                if (_incrementHeightLimit != value)
                {
                    _incrementHeightLimit = value;
                    OnPropertyChanged("IncrementHeightLimit");
                }
            }
        }
        public int IncrementWidthLimit
        {
            get { return _incrementWidthLimit; }
            set
            {
                if (_incrementWidthLimit != value)
                {
                    _incrementWidthLimit = value;
                    OnPropertyChanged("IncrementWidthLimit");
                }
            }
        }
        public int HoldEndTime
        {
            get { return _holdEndTime; }
            set
            {
                if (_holdEndTime != value)
                {
                    _holdEndTime = value;
                    OnPropertyChanged("HoldEndTime");
                }
            }
        }

        public AnimationLightningTemplate()
        {
            this.AnimationTime = 1000;
            this.IncrementHeightLimit = 10;
            this.IncrementWidthLimit = 10;
            this.HoldEndTime = 750;
        }
    }
}
