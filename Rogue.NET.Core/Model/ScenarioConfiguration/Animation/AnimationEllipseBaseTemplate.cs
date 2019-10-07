using Rogue.NET.Core.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Animation
{
    [Serializable]
    public class AnimationEllipseBaseTemplate : AnimationBaseTemplate
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
            set
            {
                if (_easingType != value)
                {
                    _easingType = value;
                    OnPropertyChanged("EasingType");
                }
            }
        }
        public double EasingAmount
        {
            get { return _easingAmount; }
            set
            {
                if (_easingAmount != value)
                {
                    _easingAmount = value;
                    OnPropertyChanged("EasingAmount");
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
        public int Height1
        {
            get { return _height1; }
            set
            {
                if (_height1 != value)
                {
                    _height1 = value;
                    OnPropertyChanged("Height1");
                }
            }
        }
        public int Height2
        {
            get { return _height2; }
            set
            {
                if (_height2 != value)
                {
                    _height2 = value;
                    OnPropertyChanged("Height2");
                }
            }
        }
        public int Width1
        {
            get { return _width1; }
            set
            {
                if (_width1 != value)
                {
                    _width1 = value;
                    OnPropertyChanged("Width1");
                }
            }
        }
        public int Width2
        {
            get { return _width2; }
            set
            {
                if (_width2 != value)
                {
                    _width2 = value;
                    OnPropertyChanged("Width2");
                }
            }
        }

        public AnimationEllipseBaseTemplate()
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
