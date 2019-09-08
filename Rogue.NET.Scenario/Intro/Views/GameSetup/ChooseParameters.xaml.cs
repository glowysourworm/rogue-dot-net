using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.Core.Event.Scenario;
using Rogue.NET.Intro.ViewModel;
using Rogue.NET.Scenario.Events;
using Rogue.NET.Scenario.Intro.Views.GameSetup.Parameters;
using System;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;

namespace Rogue.NET.Scenario.Intro.Views.GameSetup
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export]
    public partial class ChooseParameters : UserControl
    {
        readonly IRogueEventAggregator _eventAggregator;

        Type _currentParametersDisplayType;

        [ImportingConstructor]
        public ChooseParameters(IRogueEventAggregator eventAggregator)
        {
            InitializeComponent();

            _eventAggregator = eventAggregator;

            this.Loaded += (sender, e) =>
            {
                var viewModel = this.DataContext as GameSetupViewModel;

                // Reset View Model Parameters before loading first parameters view
                if (viewModel != null)
                {
                    viewModel.RogueName = "";
                    viewModel.Seed = 1;
                    viewModel.SurvivorMode = false;
                    viewModel.SelectedCharacterClass = null;
                }

                // Load the first parameters display
                eventAggregator.GetEvent<RequestNaviateParametersDisplayEvent>().Publish(typeof(ChooseName));

                _currentParametersDisplayType = typeof(ChooseName);
            };
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = this.DataContext as GameSetupViewModel;
            if (viewModel == null)
                return;

            if (_currentParametersDisplayType == typeof(ChooseName))
            {
                _eventAggregator.GetEvent<GameSetupDisplayFinished>().Publish(new GameSetupDisplayFinishedEventArgs()
                {
                    NextDisplayType = typeof(NewOpenEdit)
                });
            }
            else if (_currentParametersDisplayType == typeof(ChooseCharacterClass))
            {
                // Load the first parameters display
                _eventAggregator.GetEvent<RequestNaviateParametersDisplayEvent>().Publish(typeof(ChooseName));

                _currentParametersDisplayType = typeof(ChooseName);
            }
            else
            {
                // If there are character classes in the scenario then allow user to re-select character class
                if (viewModel.SelectedConfiguration.CharacterClasses.Count > 0)
                {
                    // Load the first parameters display
                    _eventAggregator.GetEvent<RequestNaviateParametersDisplayEvent>().Publish(typeof(ChooseCharacterClass));

                    _currentParametersDisplayType = typeof(ChooseCharacterClass);
                }
                else
                {
                    // Load the first parameters display
                    _eventAggregator.GetEvent<RequestNaviateParametersDisplayEvent>().Publish(typeof(ChooseName));

                    _currentParametersDisplayType = typeof(ChooseName);
                }
            }
        }

        private void GoButton_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = this.DataContext as GameSetupViewModel;
            if (viewModel == null)
                return;

            // If there are character classes in the scenario then allow user to select character class
            if (_currentParametersDisplayType == typeof(ChooseName) &&
                viewModel.SelectedConfiguration.CharacterClasses.Count > 0)
            {
                _eventAggregator.GetEvent<RequestNaviateParametersDisplayEvent>().Publish(typeof(ChooseCharacterClass));

                _currentParametersDisplayType = typeof(ChooseCharacterClass);
            }
            else
            {
                _eventAggregator.GetEvent<NewScenarioEvent>().Publish(new NewScenarioEventArgs()
                {
                    RogueName = viewModel.RogueName,
                    ScenarioName = viewModel.SelectedConfiguration.Name,
                    CharacterClassName = viewModel.SelectedCharacterClass != null ? viewModel.SelectedCharacterClass.RogueName : string.Empty,
                    Seed = viewModel.Seed,
                    SurvivorMode = viewModel.SurvivorMode
                });
            }
        }
    }
}
