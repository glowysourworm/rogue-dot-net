using System;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Animation
{
    [Serializable]
    public class AnimationAuraTemplate : AnimationEllipseBaseTemplate
    {
        private int _repeatCount;
        private int _animationTime;
        private bool _autoReverse;

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

        public AnimationAuraTemplate()
        {
            this.AnimationTime = 1000;
            this.AutoReverse = false;
            this.RepeatCount = 1;
        }
    }
}
