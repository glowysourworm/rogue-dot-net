using Rogue.NET.Core.Model.ScenarioConfiguration.Animation;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Animation;
using System.Windows;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System;

namespace Rogue.NET.ScenarioEditor.Views.Controls
{
    public class BrushPreviewCanvas : Canvas
    {
        Ellipse _gradientStart;
        Rectangle _gradientStop;

        // Element used during dragging event
        Shape _capturedElement;

        public BrushPreviewCanvas()
        {
            _gradientStart = new Ellipse();
            _gradientStart.Width = 15;
            _gradientStart.Height = 15;
            _gradientStart.Fill = Brushes.White;
            _gradientStart.Stroke = Brushes.Black;
            _gradientStart.StrokeThickness = 1;
            _gradientStart.Cursor = Cursors.Hand;
            _gradientStart.Opacity = 0.5;

            _gradientStop = new Rectangle();
            _gradientStop.Width = 15;
            _gradientStop.Height = 15;
            _gradientStop.Fill = Brushes.White;
            _gradientStop.Stroke = Brushes.Black;
            _gradientStop.StrokeThickness = 1;
            _gradientStop.Cursor = Cursors.Hand;
            _gradientStop.Opacity = 0.5;

            this.Children.Add(_gradientStart);
            this.Children.Add(_gradientStop);

            this.DataContextChanged += BrushPreviewCanvas_DataContextChanged;
        }

        private void BrushPreviewCanvas_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var template = this.DataContext as BrushTemplateViewModel;
            if (template == null)
                return;

            // Set gradient stops from the template
            var halfWidth = this.RenderSize.Width / 2.0D;
            var halfHeight = this.RenderSize.Height / 2.0D;

            // Calculate from origin ( {-1, -1} -> {1, 1} is the domain for the gradient stops)
            Canvas.SetLeft(_gradientStart, halfWidth * (1 + template.GradientStartX));
            Canvas.SetTop(_gradientStart, halfHeight * (1 + template.GradientStartY));
            Canvas.SetLeft(_gradientStop, halfWidth * (1 + template.GradientEndX));
            Canvas.SetTop(_gradientStop, halfHeight * (1 + template.GradientEndY));

            // Set Background
            this.Background = template.GenerateBrush();
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            if (e.Source == _gradientStop)
                _capturedElement = _gradientStop;

            else if (e.Source == _gradientStart)
                _capturedElement = _gradientStart;
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);

            EndMove();
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);

            EndMove();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (_capturedElement != null)
            {
                CalculateMove(Mouse.GetPosition(this));

                CalculateBackground();
            }
        }

        // Only updates the captured element location
        private void CalculateMove(Point point)
        {
            Canvas.SetLeft(_capturedElement, point.X);
            Canvas.SetTop(_capturedElement, point.Y);
        }

        private Point CalculateGradientPoint(Point point)
        {
            // Set gradient stops from the template
            var halfWidth = this.RenderSize.Width / 2.0D;
            var halfHeight = this.RenderSize.Height / 2.0D;

            // Calculate from origin ( {-1, -1} -> {1, 1} is the domain for the gradient stops)
            var x = (point.X / halfWidth) - 1;
            var y = (point.Y / halfHeight) - 1;

            return new Point(x, y);
        }

        // Re-calculates the background
        private void CalculateBackground()
        {
            var template = this.DataContext as BrushTemplateViewModel;
            if (template == null)
                return;

            // Have to build a background brush here
            switch (template.Type)
            {
                case BrushType.Linear:
                    {
                        var brush = this.Background is LinearGradientBrush ? (LinearGradientBrush)this.Background : 
                                                                             new LinearGradientBrush();

                        brush.GradientStops = new GradientStopCollection(template.GradientStops.Select(x =>
                        {
                            return new GradientStop((Color)ColorConverter.ConvertFromString(x.GradientColor), x.GradientOffset);
                        }));

                        brush.StartPoint = CalculateGradientPoint(new Point(Canvas.GetLeft(_gradientStart), Canvas.GetTop(_gradientStart)));
                        brush.EndPoint = CalculateGradientPoint(new Point(Canvas.GetLeft(_gradientStop), Canvas.GetTop(_gradientStop)));

                        this.Background = brush;
                    }
                    break;
                case BrushType.Radial:
                    {
                        var brush = this.Background is RadialGradientBrush ? (RadialGradientBrush)this.Background :
                                                                             new RadialGradientBrush();

                        brush.GradientStops = new GradientStopCollection(template.GradientStops.Select(x =>
                        {
                            return new GradientStop((Color)ColorConverter.ConvertFromString(x.GradientColor), x.GradientOffset);
                        }));

                        brush.GradientOrigin = CalculateGradientPoint(new Point(Canvas.GetLeft(_gradientStart), Canvas.GetTop(_gradientStart)));

                        var gradientStopPoint = CalculateGradientPoint(new Point(Canvas.GetLeft(_gradientStop), Canvas.GetTop(_gradientStop)));

                        brush.RadiusX = Math.Abs(gradientStopPoint.X - brush.GradientOrigin.X);
                        brush.RadiusY = Math.Abs(gradientStopPoint.Y - brush.GradientOrigin.Y);

                        this.Background = brush;
                    }
                    break;
                case BrushType.Solid:
                    {
                        this.Background = this.Background is SolidColorBrush ? 
                                            (SolidColorBrush)this.Background : 
                                            new SolidColorBrush((Color)ColorConverter.ConvertFromString(template.SolidColor));
                    }
                    break;
            }
        }

        private void EndMove()
        {
            _capturedElement = null;

            // Update Properties
            var template = this.DataContext as BrushTemplateViewModel;
            if (template == null)
                return;

            var gradientStart = CalculateGradientPoint(new Point(Canvas.GetLeft(_gradientStart), Canvas.GetTop(_gradientStart)));
            var gradientStop = CalculateGradientPoint(new Point(Canvas.GetLeft(_gradientStop), Canvas.GetTop(_gradientStop)));

            template.GradientEndX = gradientStop.X;
            template.GradientEndY = gradientStop.Y;
            template.GradientStartX = gradientStart.X;
            template.GradientStartY = gradientStart.Y;

            this.Background = template.GenerateBrush();
        }
    }
}
