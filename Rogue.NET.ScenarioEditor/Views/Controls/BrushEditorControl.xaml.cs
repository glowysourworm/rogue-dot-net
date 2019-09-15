using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.ScenarioEditor.Events;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Animation;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Rogue.NET.ScenarioEditor.Views.Controls
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export]
    public partial class BrushEditorControl : UserControl
    {
        [ImportingConstructor]
        public BrushEditorControl(IRogueEventAggregator eventAggregator)
        {
            InitializeComponent();

            // Copy the traits of the supplied brush
            eventAggregator.GetEvent<BrushCopyEvent>()
                           .Subscribe(brush =>
                           {
                               var viewModel = this.DataContext as BrushTemplateViewModel;
                               if (viewModel != null)
                               {
                                   viewModel.GradientEndX = brush.GradientEndX;
                                   viewModel.GradientEndY = brush.GradientEndY;
                                   viewModel.GradientStartX = brush.GradientStartX;
                                   viewModel.GradientStartY = brush.GradientStartY;

                                   // Clear() not supported in the undo service (TODO)
                                   for (int i = viewModel.GradientStops.Count - 1; i >= 0; i--)
                                       viewModel.GradientStops.RemoveAt(i);

                                   foreach (var gradientStop in brush.GradientStops)
                                       viewModel.GradientStops.Add(new GradientStopTemplateViewModel(gradientStop.GradientOffset, gradientStop.GradientColor));

                                   viewModel.Opacity = brush.Opacity;
                                   viewModel.SolidColor = brush.SolidColor;
                                   viewModel.Type = brush.Type;
                               }
                           });
        }

        private void AddGradientStopButton_Click(object sender, RoutedEventArgs e)
        {
            var template = this.DataContext as BrushTemplateViewModel;
            if (template == null)
                return;

            template.GradientStops.Add(new GradientStopTemplateViewModel(0, Color.FromArgb(0x00, 0x00, 0x00, 0x00)));
        }
        private void RemoveGradientStopButton_Click(object sender, RoutedEventArgs e)
        {
            var template = this.DataContext as BrushTemplateViewModel;
            if (template == null)
                return;

            for (int i = this.GradientStopListBox.SelectedItems.Count - 1;i >= 0;i--)
            {
                var item = this.GradientStopListBox.SelectedItems[i] as GradientStopTemplateViewModel;

                template.GradientStops.Remove(item);
            }
        }
    }
}
