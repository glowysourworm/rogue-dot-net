using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using Rogue.NET.Core.Model.ScenarioConfiguration.Animation;

using System.Collections.ObjectModel;
using System.Windows.Media;
using System.Windows;
using System;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Animation
{
    public class BrushTemplateViewModel : TemplateViewModel
    {
        public ObservableCollection<GradientStopTemplateViewModel> GradientStops { get; set; }

        private BrushType _type;
        private double _opacity;
        private string _solidColor;
        private double _gradientStartX;
        private double _gradientStartY;
        private double _gradientEndX;
        private double _gradientEndY;

        public BrushType Type
        {
            get { return _type; }
            set { this.RaiseAndSetIfChanged(ref _type, value); }
        }
        public double Opacity
        {
            get { return _opacity; }
            set { this.RaiseAndSetIfChanged(ref _opacity, value); }
        }
        public string SolidColor
        {
            get { return _solidColor; }
            set { this.RaiseAndSetIfChanged(ref _solidColor, value); }
        }
        public double GradientStartX
        {
            get { return _gradientStartX; }
            set { this.RaiseAndSetIfChanged(ref _gradientStartX, value); }
        }
        public double GradientStartY
        {
            get { return _gradientStartY; }
            set { this.RaiseAndSetIfChanged(ref _gradientStartY, value); }
        }
        public double GradientEndX
        {
            get { return _gradientEndX; }
            set { this.RaiseAndSetIfChanged(ref _gradientEndX, value); }
        }
        public double GradientEndY
        {
            get { return _gradientEndY; }
            set { this.RaiseAndSetIfChanged(ref _gradientEndY, value); }
        }

        public BrushTemplateViewModel()
        {
            this.GradientStops = new ObservableCollection<GradientStopTemplateViewModel>();
            this.Opacity = 1;
            this.SolidColor = Colors.White.ToString();
        }
        public BrushTemplateViewModel(string name)
        {
            this.GradientStops = new ObservableCollection<GradientStopTemplateViewModel>();
            this.Name = name;
            this.Opacity = 1;
            this.SolidColor = Colors.White.ToString();
        }

        // TODO: MOVE THIS
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
                        foreach (GradientStopTemplateViewModel t in this.GradientStops)
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
                        b.RadiusX = Math.Abs(x);
                        b.RadiusY = Math.Abs(y);
                        foreach (GradientStopTemplateViewModel t in this.GradientStops)
                            b.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString(t.GradientColor), t.GradientOffset));
                        return b;
                    }
            }

            return null;

        }
    }
}
