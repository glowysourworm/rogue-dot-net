using Microsoft.Practices.Prism.Events;
using Rogue.NET.Common;
using Rogue.NET.Common.Extensions;
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

            _eventAggregator.GetEvent<GameSetupDisplayFinished>().Publish(new GameSetupDisplayFinished()
            {
                NextDisplayType = typeof(ChooseParameters)
            });
        }
    }
}
