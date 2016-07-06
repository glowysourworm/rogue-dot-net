using Microsoft.Practices.Prism.Events;
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
    /// <summary>
    /// Interaction logic for ChooseSavedGame.xaml
    /// </summary>
    public partial class ChooseSavedGame : UserControl
    {
        readonly IEventAggregator _eventAggragator;

        public ChooseSavedGame(IEventAggregator eventAggregator)
        {
            InitializeComponent();

            _eventAggragator = eventAggregator;

            this.Loaded += (obj, e) => {
                var viewModel = this.DataContext as GameSetupViewModel;
                if (viewModel != null)
                    viewModel.Reinitialize();
            };
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            _eventAggragator.GetEvent<GameSetupDisplayFinished>().Publish(new GameSetupDisplayFinished()
            {
                NextDisplayType = typeof(NewOpenEdit)
            });
        }
    }
}
