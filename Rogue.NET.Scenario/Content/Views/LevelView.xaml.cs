using Prism.Events;
using Rogue.NET.Core.Event.Scenario.Level.Event;
using Rogue.NET.Core.Logic.Processing.Enum;
using System.ComponentModel.Composition;
using System.Windows.Controls;
using System.Windows.Media;

namespace Rogue.NET.Scenario.Views
{
    [Export]
    public partial class LevelView : UserControl
    {
        [ImportingConstructor]
        public LevelView(IEventAggregator eventAggregator)
        {
            InitializeComponent();

            var brush = this.CanvasBorder.BorderBrush;
            var objectiveAcheivedBrush = new LinearGradientBrush(new GradientStopCollection(new GradientStop[] {
                new GradientStop(Colors.Red, 0),
                new GradientStop(Colors.Orange, 0.2),
                new GradientStop(Colors.Yellow, 0.4),
                new GradientStop(Colors.Green, 0.6),
                new GradientStop(Colors.Blue, 0.8),
                new GradientStop(Colors.Purple, 1)
            }));

            eventAggregator.GetEvent<ScenarioUpdateEvent>().Subscribe(update =>
            {
                if (update.ScenarioUpdateType == ScenarioUpdateType.ObjectiveAcheived)
                    this.CanvasBorder.BorderBrush = objectiveAcheivedBrush;

                else
                    this.CanvasBorder.BorderBrush = brush;
            });
        }
   }
}
