using Prism.Events;
using Rogue.NET.Common.Events.Scenario;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Intro.ViewModel;
using Rogue.NET.Scenario.Events;
using Rogue.NET.Scenario.Intro.Views.GameSetup.Parameters;
using System;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;

namespace Rogue.NET.Scenario.Intro.Views.GameSetup
{
    [Export]
    public partial class ChooseParameters : UserControl
    {
        readonly IEventAggregator _eventAggregator;

        Type _currentParametersDisplayType;

        [ImportingConstructor]
        public ChooseParameters(IEventAggregator eventAggregator)
        {
            InitializeComponent();

            _eventAggregator = eventAggregator;

            this.Loaded += (sender, e) =>
            {
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
            else if (_currentParametersDisplayType == typeof(ChooseReligion))
            {
                // Load the first parameters display
                _eventAggregator.GetEvent<RequestNaviateParametersDisplayEvent>().Publish(typeof(ChooseName));

                _currentParametersDisplayType = typeof(ChooseName);
            }
            else
            {
                // If there are religions in the scenario then allow user to re-select relegion
                if (viewModel.SelectedConfiguration.Religions.Count > 0)
                {
                    // Load the first parameters display
                    _eventAggregator.GetEvent<RequestNaviateParametersDisplayEvent>().Publish(typeof(ChooseReligion));

                    _currentParametersDisplayType = typeof(ChooseReligion);
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

            if (_currentParametersDisplayType == typeof(ChooseName))
            {
                // If there are religions in the scenario then allow user to select relegion
                if (viewModel.SelectedConfiguration.Religions.Count > 0)
                {
                    _eventAggregator.GetEvent<RequestNaviateParametersDisplayEvent>().Publish(typeof(ChooseReligion));

                    _currentParametersDisplayType = typeof(ChooseReligion);
                }
                else
                {
                    _eventAggregator.GetEvent<RequestNaviateParametersDisplayEvent>().Publish(typeof(ChooseAttribute));

                    _currentParametersDisplayType = typeof(ChooseAttribute);
                }
            }
            else if (_currentParametersDisplayType == typeof(ChooseReligion))
            {
                // Load the first parameters display
                _eventAggregator.GetEvent<RequestNaviateParametersDisplayEvent>().Publish(typeof(ChooseAttribute));

                _currentParametersDisplayType = typeof(ChooseAttribute);
            }
            else
            {
                _eventAggregator.GetEvent<NewScenarioEvent>().Publish(new NewScenarioEventArgs()
                {
                    RogueName = viewModel.RogueName,
                    ScenarioName = viewModel.SelectedConfiguration.Name,
                    ReligionName = viewModel.SelectedReligion != null ? viewModel.SelectedReligion.Name : string.Empty,
                    Seed = viewModel.Seed,
                    SurvivorMode = viewModel.SurvivorMode,
                    AttributeEmphasis = viewModel.AttributeEmphasis
                });
            }
        }
    }
}
