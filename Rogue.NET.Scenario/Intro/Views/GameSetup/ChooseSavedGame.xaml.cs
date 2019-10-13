using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.Core.Processing.Event.Scenario;
using Rogue.NET.Intro.ViewModel;
using Rogue.NET.Scenario.Intro.ViewModel;
using Rogue.NET.Scenario.Processing.Event;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;

namespace Rogue.NET.Scenario.Intro.Views.GameSetup
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export]
    public partial class ChooseSavedGame : UserControl
    {
        readonly IRogueEventAggregator _eventAggregator;

        [ImportingConstructor]
        public ChooseSavedGame(IRogueEventAggregator eventAggregator)
        {
            InitializeComponent();

            _eventAggregator = eventAggregator;

            this.Loaded += (obj, e) => 
            {
                var viewModel = this.DataContext as GameSetupViewModel;
                if (viewModel != null)
                    viewModel.Reinitialize();
            };
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = (sender as Button).DataContext as SavedGameViewModel;

            // Delete
            _eventAggregator.GetEvent<DeleteScenarioEvent>()
                            .Publish(new DeleteScenarioEventData()
                            {
                                ScenarioName = viewModel.Name
                            });

            // Back
            _eventAggregator.GetEvent<GameSetupDisplayFinished>()
                            .Publish(new GameSetupDisplayFinishedEventArgs()
            {
                NextDisplayType = typeof(NewOpenEdit)
            });
        }

        private void GoButton_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = (sender as Button).DataContext as SavedGameViewModel;

            _eventAggregator.GetEvent<OpenScenarioEvent>()
                            .Publish(new OpenScenarioEventArgs()
                            {
                                ScenarioName = viewModel.Name
                            });
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            _eventAggregator.GetEvent<GameSetupDisplayFinished>()
                            .Publish(new GameSetupDisplayFinishedEventArgs()
                            {
                                NextDisplayType = typeof(NewOpenEdit)
                            });
        }
    }
}
