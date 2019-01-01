using ProtoBuf;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using System;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Animation
{
    [Serializable]
    [ProtoContract(AsReferenceDefault = true)]
    public class AnimationTemplate : Template
    {
        private int _repeatCount;
        private int _animationTime;
        private bool _autoReverse;
        private bool _constantVelocity;
        private double _accelerationRatio;
        private AnimationType _type;
        private BrushTemplate _fillTemplate;
        private BrushTemplate _strokeTemplate;
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

        [ProtoMember(1)]
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
        [ProtoMember(2)]
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
        [ProtoMember(3)]
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
        [ProtoMember(4)]
        public bool ConstantVelocity
        {
            get { return _constantVelocity; }
            set
            {
                if (_constantVelocity != value)
                {
                    _constantVelocity = value;
                    OnPropertyChanged("ConstantVelocity");
                }
            }
        }
        [ProtoMember(5)]
        public double AccelerationRatio
        {
            get { return _accelerationRatio; }
            set
            {
                if (_accelerationRatio != value)
                {
                    _accelerationRatio = value;
                    OnPropertyChanged("AccelerationRatio");
                }
            }
        }
        [ProtoMember(6)]
        public AnimationType Type
        {
            get { return _type; }
            set
            {
                if (_type != value)
                {
                    _type = value;
                    OnPropertyChanged("Type");
                }
            }
        }
        [ProtoMember(7, AsReference = true)]
        public BrushTemplate FillTemplate
        {
            get { return _fillTemplate; }
            set
            {
                if (_fillTemplate != value)
                {
                    _fillTemplate = value;
                    OnPropertyChanged("FillTemplate");
                }
            }
        }
        [ProtoMember(8, AsReference = true)]
        public BrushTemplate StrokeTemplate
        {
            get { return _strokeTemplate; }
            set
            {
                if (_strokeTemplate != value)
                {
                    _strokeTemplate = value;
                    OnPropertyChanged("StrokeTemplate");
                }
            }
        }
        [ProtoMember(9)]
        public double StrokeThickness
        {
            get { return _strokeThickness; }
            set
            {
                if (_strokeThickness != value)
                {
                    _strokeThickness = value;
                    OnPropertyChanged("StrokeThickness");
                }
            }
        }
        [ProtoMember(10)]
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
        [ProtoMember(11)]
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
        [ProtoMember(12)]
        public double Height1
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
        [ProtoMember(13)]
        public double Height2
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
        [ProtoMember(14)]
        public double Width1
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
        [ProtoMember(15)]
        public double Width2
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
        [ProtoMember(16)]
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
        [ProtoMember(17)]
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
        [ProtoMember(18)]
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
        [ProtoMember(19)]
        public double RadiusFromFocus
        {
            get { return _radiusFromFocus; }
            set
            {
                if (_radiusFromFocus != value)
                {
                    _radiusFromFocus = value;
                    OnPropertyChanged("RadiusFromFocus");
                }
            }
        }
        [ProtoMember(20)]
        public double SpiralRate
        {
            get { return _spiralRate; }
            set
            {
                if (_spiralRate != value)
                {
                    _spiralRate = value;
                    OnPropertyChanged("SpiralRate");
                }
            }
        }
        [ProtoMember(21)]
        public double RoamRadius
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

        //Constructors
        public AnimationTemplate()
        {
            this.StrokeTemplate = new BrushTemplate();
            this.FillTemplate = new BrushTemplate();
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
