using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Animation
{
    [Serializable]
    public class AnimationBlinkTemplate : AnimationBaseTemplate
    {
        int _animationTime;
        int _repeatCount;
        bool _autoReverse;
        double _opacity1;
        double _opacity2;

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
        public double Opacity1
        {
            get { return _opacity1; }
            set
            {
                if (_opacity1 != value)
                {
                    _opacity1 = value;
                    OnPropertyChanged("Opacity1");
                }
            }
        }
        public double Opacity2
        {
            get { return _opacity2; }
            set
            {
                if (_opacity2 != value)
                {
                    _opacity2 = value;
                    OnPropertyChanged("Opacity2");
                }
            }
        }

        public AnimationBlinkTemplate()
        {
            this.AnimationTime = 1000;
            this.RepeatCount = 1;
            this.AutoReverse = false;
            this.Opacity1 = 1;
            this.Opacity2 = 0;
        }
    }
}
