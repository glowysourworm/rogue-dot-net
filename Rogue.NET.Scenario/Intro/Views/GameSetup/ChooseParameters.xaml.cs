using Prism.Events;
using Rogue.NET.Common.Events.Scenario;
using Rogue.NET.Intro.ViewModel;
using Rogue.NET.Scenario.Events;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;

namespace Rogue.NET.Scenario.Intro.Views.GameSetup
{
    [Export]
    public partial class ChooseParameters : UserControl
    {
        readonly IEventAggregator _eventAggregator;

        [ImportingConstructor]
        public ChooseParameters(IEventAggregator eventAggregator)
        {
            InitializeComponent();

            _eventAggregator = eventAggregator;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            _eventAggregator.GetEvent<GameSetupDisplayFinished>().Publish(new GameSetupDisplayFinishedEventArgs()
            {
                NextDisplayType = typeof(NewOpenEdit)
            });
        }

        private void GoButton_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = this.DataContext as GameSetupViewModel;
            _eventAggregator.GetEvent<NewScenarioEvent>().Publish(new NewScenarioEventArgs()
            {
                RogueName = viewModel.RogueName,
                ScenarioName = viewModel.ScenarioName,
                Seed = viewModel.Seed,
                SurvivorMode = viewModel.SurvivorMode,
                AttributeEmphasis = viewModel.AttributeEmphasis
            });
        }
    }
}
