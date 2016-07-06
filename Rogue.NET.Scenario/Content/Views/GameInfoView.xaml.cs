using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.PubSubEvents;
using Microsoft.Practices.Unity;
using Rogue.NET.Model.Events;
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

namespace Rogue.NET.Scenario.Content.Views
{
    public partial class GameInfoView : UserControl
    {
        public GameInfoView()
        {
        }

        [InjectionConstructor]
        public GameInfoView(IEventAggregator eventAggregator)
        {
            InitializeComponent();

            eventAggregator.GetEvent<ScenarioInfoUpdatedEvent>().Subscribe((e) =>
            {
                string leftText = "{1} Scenario (#{2}) / Level {4}";
                string rightText = "Ticks: {0}";

                if (e.SurvivorMode)
                    this.SurvivorModeTB.Visibility = System.Windows.Visibility.Visible;

                if (e.ObjectiveAcheived)
                    this.ObjectiveAcheivedTB.Visibility = System.Windows.Visibility.Visible;

                leftText = string.Format(leftText, e.PlayerName, e.ScenarioName, e.Seed, e.StartTime.ToShortDateString(), e.CurrentLevel);
                rightText = string.Format(rightText, e.Ticks.ToString());

                this.GeneralInfoTB.Text = leftText;
                this.TicksTB.Text = rightText;
            });
        }
    }
}
