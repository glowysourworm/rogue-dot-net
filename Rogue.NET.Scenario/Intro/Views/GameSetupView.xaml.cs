using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.PubSubEvents;
using Rogue.NET.Common.Events.Scenario;
using Rogue.NET.Intro.ViewModel;
using Rogue.NET.Scenario.Events;
using Rogue.NET.Scenario.Intro.Views.GameSetup;
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

namespace Rogue.NET.Intro.Views
{
    public partial class GameSetupView : UserControl
    {
        readonly IEventAggregator _eventAggregator;

        bool _new = true;

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
                _eventAggregator.GetEvent<NewScenarioEvent>().Publish(new NewScenarioEvent()
                {
                    RogueName = viewModel.RogueName,
                    ScenarioName = viewModel.ScenarioName,
                    Seed = viewModel.Seed,
                    SurvivorMode = viewModel.SurvivorMode
                });
            }
            else
            {
                _eventAggregator.GetEvent<OpenScenarioEvent>().Publish(new OpenScenarioEvent()
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
                _eventAggregator.GetEvent<GameSetupDisplayFinished>().Publish(new GameSetupDisplayFinished()
                {
                    NextDisplayType = typeof(NewOpenEdit)
                });
                e.Handled = true;
            }
        }
    }
}
