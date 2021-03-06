﻿using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;

using System;
using System.Windows.Media;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Animation
{
    public class GradientStopTemplateViewModel : TemplateViewModel
    {
        private string _gradientColor;
        private double _gradientOffset;

        public string GradientColor
        {
            get { return _gradientColor; }
            set { this.RaiseAndSetIfChanged(ref _gradientColor, value); }
        }
        public double GradientOffset
        {
            get { return _gradientOffset; }
            set { this.RaiseAndSetIfChanged(ref _gradientOffset, value); }
        }
        public GradientStopTemplateViewModel()
        {
            this.GradientColor = Colors.Transparent.ToString();
            this.GradientOffset = 0;
        }
        public GradientStopTemplateViewModel(double offset, Color c)
        {
            this.GradientColor = c.ToString();
            this.GradientOffset = offset;
        }
        public GradientStopTemplateViewModel(double offset, string color)
        {
            this.GradientColor = color;
            this.GradientOffset = offset;
        }
    }
}
