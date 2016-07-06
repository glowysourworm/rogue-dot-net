using Microsoft.Practices.Prism.Events;
using Rogue.NET.Common.Events.Scenario;
using Rogue.NET.Intro.ViewModel;
using Rogue.NET.Scenario.Events;
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

namespace Rogue.NET.Scenario.Intro.Views.GameSetup
{
    public partial class ChooseParameters : UserControl
    {
        readonly IEventAggregator _eventAggregator;

        public ChooseParameters(IEventAggregator eventAggregator)
        {
            InitializeComponent();

            _eventAggregator = eventAggregator;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            _eventAggregator.GetEvent<GameSetupDisplayFinished>().Publish(new GameSetupDisplayFinished()
            {
                NextDisplayType = typeof(NewOpenEdit)
            });
        }

        private void GoButton_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = this.DataContext as GameSetupViewModel;
            _eventAggregator.GetEvent<NewScenarioEvent>().Publish(new NewScenarioEvent()
            {
                RogueName = viewModel.RogueName,
                ScenarioName = viewModel.ScenarioName,
                Seed = viewModel.Seed,
                SurvivorMode = viewModel.SurvivorMode
            });
        }
    }
}
