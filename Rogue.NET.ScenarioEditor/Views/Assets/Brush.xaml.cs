using BrushEditor;
using Rogue.NET.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Rogue.NET.ScenarioEditor.Views.Assets
{
    public partial class Brush : UserControl
    {
        public Brush()
        {
            InitializeComponent();

            this.DataContextChanged += (obj, e) =>
            {
                var brush = e.NewValue as BrushTemplate;
                if (brush != null)
                    this.BrushEditor.BrushEditorViewModel.Brush = brush.GenerateBrush();
            };
            this.BrushEditor.BrushEditorViewModel.PropertyChanged += (obj, e) =>
            {
                var brush = this.DataContext as BrushTemplate;
                switch (this.BrushEditor.BrushEditorViewModel.BrushType)
                {
                    case BrushTypes.Linear:
                        {
                            var viewModel = this.BrushEditor.BrushEditorViewModel.Brush as LinearGradientBrush;
                            brush.GradientEndX = viewModel.EndPoint.X;
                            brush.GradientEndY = viewModel.EndPoint.Y;
                            brush.GradientStartX = viewModel.StartPoint.X;
                            brush.GradientStartY = viewModel.StartPoint.Y;
                            brush.GradientStops = viewModel.GradientStops.Select(x => new GradientStopTemplate()
                            {
                                GradientColor = x.Color.ToString(),
                                GradientOffset = x.Offset
                            }).ToList();
                            brush.Opacity = viewModel.Opacity;
                            brush.Type = BrushType.Linear;
                        }
                        break;
                    case BrushTypes.Radial:
                        {
                            var viewModel = this.BrushEditor.BrushEditorViewModel.Brush as RadialGradientBrush;
                            brush.GradientStartX = viewModel.Center.X;
                            brush.GradientStartY = viewModel.Center.Y;
                            brush.GradientStops = viewModel.GradientStops.Select(x => new GradientStopTemplate()
                            {
                                GradientColor = x.Color.ToString(),
                                GradientOffset = x.Offset
                            }).ToList();
                            brush.Opacity = viewModel.Opacity;
                            brush.Type = BrushType.Radial;
                        }
                        break;
                    case BrushTypes.Solid:
                        {
                            var viewModel = this.BrushEditor.BrushEditorViewModel.Brush as SolidColorBrush;
                            brush.Opacity = viewModel.Opacity;
                            brush.SolidColor = viewModel.Color.ToString();
                            brush.Type = BrushType.Solid;
                        }
                        break;
                }
            };
        }
    }
}
