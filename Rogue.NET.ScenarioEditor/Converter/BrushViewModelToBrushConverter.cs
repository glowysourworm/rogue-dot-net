using Rogue.NET.Core.Model.ScenarioConfiguration.Animation;
using Rogue.NET.Core.Utility;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Animation;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Rogue.NET.ScenarioEditor.Converter
{
    public class BrushViewModelToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var viewModel = value as BrushTemplateViewModel;

            if (viewModel == null)
                return Binding.DoNothing;

            // NOTE** Opacity Not Used
            switch (viewModel.Type)
            {
                case BrushType.Solid:
                    return new SolidColorBrush(ColorUtility.Convert(viewModel.SolidColor));
                case BrushType.Linear:
                    {
                        var brush = new LinearGradientBrush();
                        brush.StartPoint = new Point(viewModel.GradientStartX, viewModel.GradientStartY);
                        brush.EndPoint = new Point(viewModel.GradientEndX, viewModel.GradientEndY);

                        foreach (var gradientStop in viewModel.GradientStops)
                            brush.GradientStops.Add(new GradientStop(ColorUtility.Convert(gradientStop.GradientColor), 
                                                                     gradientStop.GradientOffset));

                        return brush;
                    }
                case BrushType.Radial:
                    {
                        var brush = new RadialGradientBrush();
                        brush.GradientOrigin = new Point(viewModel.GradientStartX, viewModel.GradientStartY);
                        var radiusX = viewModel.GradientEndX - viewModel.GradientStartX;
                        var radiusY = viewModel.GradientEndY - viewModel.GradientStartY;

                        brush.RadiusX = Math.Abs(radiusX);
                        brush.RadiusY = Math.Abs(radiusY);

                        foreach (var gradientStop in viewModel.GradientStops)
                            brush.GradientStops.Add(new GradientStop(ColorUtility.Convert(gradientStop.GradientColor),
                                                                     gradientStop.GradientOffset));

                        return brush;
                    }
            }

            return Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
