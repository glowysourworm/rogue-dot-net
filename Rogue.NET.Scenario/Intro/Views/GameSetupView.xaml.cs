using Prism.Events;
using Rogue.NET.Common.Events.Scenario;
using Rogue.NET.Intro.ViewModel;
using Rogue.NET.Scenario.Events;
using Rogue.NET.Scenario.Intro.Views.GameSetup;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Rogue.NET.Intro.Views
{
    [Export]
    public partial class GameSetupView : UserControl
    {
        readonly IEventAggregator _eventAggregator;

        bool _new = true;

        [ImportingConstructor]
        public GameSetupView(
            IEventAggregator eventAggregator,
            GameSetupViewModel viewModel)
        {
            InitializeComponent();

            _eventAggregator = eventAggregator;

            this.DataContext = viewModel;
        }


        private void Start_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = this.DataContext as GameSetupViewModel;
            if (_new)
            {
                _eventAggregator.GetEvent<NewScenarioEvent>().Publish(new NewScenarioEventArgs()
                {
                    RogueName = viewModel.RogueName,
                    ScenarioName = viewModel.ScenarioName,
                    Seed = viewModel.Seed,
                    SurvivorMode = viewModel.SurvivorMode
                });
            }
            else
            {
                _eventAggregator.GetEvent<OpenScenarioEvent>().Publish(new OpenScenarioEventArgs()
                {
                    ScenarioName = viewModel.ScenarioName
                });
            }
        }

        private void Config_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                var config = e.AddedItems[0] as Rogue.NET.Intro.ViewModel.GameSetupViewModel.ScenarioSelectionViewModel;
                var viewModel = this.DataContext as GameSetupViewModel;
                viewModel.ScenarioName = config.Name;
            }
        }

        private void ScenariosListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                var scenario = e.AddedItems[0] as Rogue.NET.Intro.ViewModel.GameSetupViewModel.ScenarioSelectionViewModel;
                var viewModel = this.DataContext as GameSetupViewModel;
                viewModel.ScenarioName = scenario.Name;
            }
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);
            if (e.Key == Key.Escape)
            {
                _eventAggregator.GetEvent<GameSetupDisplayFinished>().Publish(new GameSetupDisplayFinishedEventArgs()
                {
                    NextDisplayType = typeof(NewOpenEdit)
                });
                e.Handled = true;
            }
        }
    }
}
