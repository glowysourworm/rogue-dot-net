using Prism.Events;
using Rogue.NET.Common.Events.Scenario;
using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.Intro.ViewModel;
using Rogue.NET.Scenario.Events;
using Rogue.NET.Scenario.Intro.Views.GameSetup;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Rogue.NET.Scenario.Intro.Views
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export]
    public partial class GameSetupView : UserControl
    {
        readonly IRogueEventAggregator _eventAggregator;

        bool _new = true;

        [ImportingConstructor]
        public GameSetupView(
            IRogueEventAggregator eventAggregator,
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
                    ScenarioName = viewModel.SelectedConfiguration.Name,
                    CharacterClassName = viewModel.SelectedCharacterClass == null ? string.Empty : viewModel.SelectedCharacterClass.RogueName,
                    Seed = viewModel.Seed,
                    SurvivorMode = viewModel.SurvivorMode
                });
            }
            else
            {
                _eventAggregator.GetEvent<OpenScenarioEvent>().Publish(new OpenScenarioEventArgs()
                {
                    ScenarioName = viewModel.SelectedGame.Name
                });
            }
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);
            if (e.Key == Key.Escape)
            {
                var viewModel = this.DataContext as GameSetupViewModel;

                _eventAggregator.GetEvent<GameSetupDisplayFinished>().Publish(new GameSetupDisplayFinishedEventArgs()
                {
                    NextDisplayType = typeof(NewOpenEdit)
                });
                e.Handled = true;
            }
        }
    }
}
