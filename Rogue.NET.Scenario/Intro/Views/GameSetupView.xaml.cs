using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.Core.Processing.Event.Scenario;
using Rogue.NET.Core.Processing.Service.Cache;
using Rogue.NET.Intro.ViewModel;
using Rogue.NET.Scenario.Intro.Views.GameSetup;
using Rogue.NET.Scenario.Processing.Event;
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
                _eventAggregator.GetEvent<NewScenarioEvent>().Publish(new ScenarioInfo()
                {
                    RogueName = viewModel.RogueName,
                    ScenarioName = viewModel.SelectedConfiguration.Name,
                    CharacterClass = viewModel.SelectedCharacterClass == null ? string.Empty : viewModel.SelectedCharacterClass.RogueName,
                    IsObjectiveAcheived = false,
                    Seed = viewModel.Seed,
                    SurvivorMode = viewModel.SurvivorMode
                });
            }
            else
            {
                _eventAggregator.GetEvent<OpenScenarioEvent>().Publish(new ScenarioInfo()
                {
                    CurrentLevel = viewModel.SelectedGame.CurrentLevel,
                    ScenarioName = viewModel.SelectedGame.ScenarioName,
                    CharacterClass = viewModel.SelectedGame.CharacterClass,
                    SurvivorMode = viewModel.SelectedGame.SurvivorMode,
                    IsObjectiveAcheived = viewModel.SelectedGame.ObjectiveAcheived,
                    RogueName = viewModel.SelectedGame.RogueName,
                    Seed = viewModel.SelectedGame.Seed,
                    SmileyBodyColor = viewModel.SelectedGame.SmileyBodyColor,
                    SmileyExpression = viewModel.SelectedGame.SmileyExpression,
                    SmileyLineColor = viewModel.SelectedGame.SmileyLineColor
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
