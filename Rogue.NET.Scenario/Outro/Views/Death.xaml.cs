using Prism.Events;
using Rogue.NET.Common.Events.Scenario;
using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.Scenario.Content.ViewModel.Content;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;

namespace Rogue.NET.Scenario.Outro.Views
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export]
    public partial class DeathDisplay : UserControl
    {
        readonly IRogueEventAggregator _eventAggregator;

        public string DiedOfText
        {
            get { return this.DiedOfTextBlock?.Text ?? ""; }
            set
            {
                this.DiedOfTextBlock.Text = value;
            }
        }

        [ImportingConstructor]
        public DeathDisplay(IRogueEventAggregator eventAggregator, PlayerViewModel playerViewModel)
        {
            this.DataContext = playerViewModel;

            InitializeComponent();

            _eventAggregator = eventAggregator;
        }
        private void ContinueButton_Click(object sender, RoutedEventArgs e)
        {
            _eventAggregator.GetEvent<ContinueScenarioEvent>().Publish();
        }
        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            _eventAggregator.GetEvent<ExitScenarioEvent>().Publish();
        }
    }
}
