using ProtoBuf;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using System;
using System.Windows.Media;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Animation
{
    [Serializable]
    [ProtoContract(AsReferenceDefault = true, SkipConstructor = true)]
    public class GradientStopTemplate : Template
    {
        private string _gradientColor;
        private double _gradientOffset;

        [ProtoMember(1)]
        public string GradientColor
        {
            get { return _gradientColor; }
            set
            {
                if (_gradientColor != value)
                {
                    _gradientColor = value;
                    OnPropertyChanged("GradientColor");
                }
            }
        }
        [ProtoMember(2)]
        public double GradientOffset
        {
            get { return _gradientOffset; }
            set
            {
                if (_gradientOffset != value)
                {
                    _gradientOffset = value;
                    OnPropertyChanged("GradientOffset");
                }
            }
        }
        public GradientStopTemplate() { }
        public GradientStopTemplate(double offset, Color c)
        {
            this.GradientColor = c.ToString();
            this.GradientOffset = offset;
        }
    }
}
