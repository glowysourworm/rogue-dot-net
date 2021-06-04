using Rogue.NET.Core.Media.SymbolEffect.Utility;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Animation
{
    public enum BrushType
    {
        Solid,
        Linear,
        Radial
    }

    [Serializable]
    public class BrushTemplate : Template
    {
        private BrushType _type;
        private double _opacity;
        private string _solidColor;
        private double _gradientStartX;
        private double _gradientStartY;
        private double _gradientEndX;
        private double _gradientEndY;

        public List<GradientStopTemplate> GradientStops { get; set; }
        public BrushType Type
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
        public double Opacity
        {
            get { return _opacity; }
            set
            {
                if (_opacity != value)
                {
                    _opacity = value;
                    OnPropertyChanged("Opacity");
                }
            }
        }
        public string SolidColor
        {
            get { return _solidColor; }
            set
            {
                if (_solidColor != value)
                {
                    _solidColor = value;
                    OnPropertyChanged("SolidColor");
                }
            }
        }
        public double GradientStartX
        {
            get { return _gradientStartX; }
            set
            {
                if (_gradientStartX != value)
                {
                    _gradientStartX = value;
                    OnPropertyChanged("GradientStartX");
                }
            }
        }
        public double GradientStartY
        {
            get { return _gradientStartY; }
            set
            {
                if (_gradientStartY != value)
                {
                    _gradientStartY = value;
                    OnPropertyChanged("GradientStartY");
                }
            }
        }
        public double GradientEndX
        {
            get { return _gradientEndX; }
            set
            {
                if (_gradientEndX != value)
                {
                    _gradientEndX = value;
                    OnPropertyChanged("GradientEndX");
                }
            }
        }
        public double GradientEndY
        {
            get { return _gradientEndY; }
            set
            {
                if (_gradientEndY != value)
                {
                    _gradientEndY = value;
                    OnPropertyChanged("GradientEndY");
                }
            }
        }

        public BrushTemplate()
        {
            this.GradientStops = new List<GradientStopTemplate>();
            this.Opacity = 1;
            this.SolidColor = ColorOperations.ConvertBack(Colors.White);
        }
        public BrushTemplate(string name)
        {
            this.GradientStops = new List<GradientStopTemplate>();
            this.Name = name;
            this.Opacity = 1;
            this.SolidColor = ColorOperations.ConvertBack(Colors.White);
        }

        public Brush GenerateBrush()
        {
            switch (this.Type)
            {
                case BrushType.Solid:
                    {
                        SolidColorBrush b = new SolidColorBrush((Color)ColorConverter.ConvertFromString(this.SolidColor));
                        //b.Opacity = this.Opacity;
                        return b;
                    }
                case BrushType.Linear:
                    {
                        LinearGradientBrush b = new LinearGradientBrush();
                        //b.Opacity = this.Opacity;
                        b.StartPoint = new Point(this.GradientStartX, this.GradientStartY);
                        b.EndPoint = new Point(this.GradientEndX, this.GradientEndY);
                        foreach (GradientStopTemplate t in this.GradientStops)
                            b.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString(t.GradientColor), t.GradientOffset));

                        return b;
                    }
                case BrushType.Radial:
                    {
                        RadialGradientBrush b = new RadialGradientBrush();
                        //b.Opacity = this.Opacity;
                        b.GradientOrigin = new Point(this.GradientStartX, this.GradientStartY);
                        double x = this.GradientEndX - this.GradientStartX;
                        double y = this.GradientEndY - this.GradientStartY;
                        b.RadiusX = System.Math.Abs(x);
                        b.RadiusY = System.Math.Abs(y);
                        foreach (GradientStopTemplate t in this.GradientStops)
                            b.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString(t.GradientColor), t.GradientOffset));
                        return b;
                    }
            }

            return null;

        }
    }
}
