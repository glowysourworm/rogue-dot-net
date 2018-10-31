using Prism.Events;
using Rogue.NET.Common.Events.Scenario;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;

namespace Rogue.NET.Scenario.Outro.Views
{
    [Export]
    public partial class DeathDisplay : UserControl
    {
        readonly IEventAggregator _eventAggregator;

        [ImportingConstructor]
        public DeathDisplay(IEventAggregator eventAggregator)
        {
            InitializeComponent();

            _eventAggregator = eventAggregator;
        }
        private void ContinueButton_Click(object sender, RoutedEventArgs e)
        {
            _eventAggregator.GetEvent<ContinueScenarioEvent>().Publish();
        }
        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            _eventAggregator.GetEvent<ExitScenarioEvent>().Publish();
        }
    }
}
