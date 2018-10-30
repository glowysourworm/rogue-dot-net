using Prism.Events;
using Rogue.NET.Model.Events;
using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace Rogue.NET.Scenario.Content.Views
{
    [Export]
    public partial class GameInfoView : UserControl
    {
        [ImportingConstructor]
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
