using Prism.Events;
using Rogue.NET.Intro.ViewModel;
using Rogue.NET.Scenario.Events;
using System;
using System.Windows.Controls;
using System.Windows.Input;

namespace Rogue.NET.Scenario.Intro.Views.GameSetup
{
    public partial class ChooseScenario : UserControl
    {
        readonly IEventAggregator _eventAggregator;

        public ChooseScenario(IEventAggregator eventAggregator)
        {
            InitializeComponent();

            _eventAggregator = eventAggregator;

            this.Loaded += (obj, e) => { this.SmallLB.SelectedIndex = 0; };
        }

        private void LeftArrowTB_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.SmallLB.SelectedIndex = Math.Max(0, this.SmallLB.SelectedIndex - 1);
        }

        private void RightArrowTB_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.SmallLB.SelectedIndex = Math.Min(this.SmallLB.Items.Count, this.SmallLB.SelectedIndex + 1);
        }

        private void BigLB_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var item = this.SmallLB.SelectedItem as GameSetupViewModel.ScenarioSelectionViewModel;
            var viewModel = this.DataContext as GameSetupViewModel;

            viewModel.ScenarioName = item.Name;
            viewModel.SmileyColor = item.SmileyColor;
            viewModel.SmileyLineColor = item.SmileyLineColor;

            _eventAggregator.GetEvent<GameSetupDisplayFinished>().Publish(new GameSetupDisplayFinishedEventArgs()
            {
                NextDisplayType = typeof(ChooseParameters)
            });
        }
    }
}
