using Microsoft.Practices.Prism.Events;
using Rogue.NET.Common;
using Rogue.NET.Common.Events.Scenario;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Rogue.NET.Scenario.Outro.Views
{
    /// <summary>
    /// Interaction logic for DeathDisplay.xaml
    /// </summary>
    public partial class DeathDisplay : UserControl
    {
        readonly IEventAggregator _eventAggregator;

        public DeathDisplay(IEventAggregator eventAggregator)
        {
            InitializeComponent();

            _eventAggregator = eventAggregator;
        }
        private void ContinueButton_Click(object sender, RoutedEventArgs e)
        {
            _eventAggregator.GetEvent<ContinueScenarioEvent>().Publish(new ContinueScenarioEvent());
        }
        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            _eventAggregator.GetEvent<ExitScenarioEvent>().Publish(new ExitScenarioEvent());
        }
    }
}
