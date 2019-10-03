using Rogue.NET.ScenarioEditor.Service.Interface;
using Rogue.NET.ScenarioEditor.ViewModel.Overview.Interface;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration;
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Common.Extension;
using Rogue.NET.ScenarioEditor.ViewModel.Overview;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Content;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.ScenarioConfiguration.Content;
using Rogue.NET.Core.Model.Scenario.Content.Item;
using Rogue.NET.Core.Model.Scenario.Character.Extension;
using Rogue.NET.Core.Model;
using Rogue.NET.Core.Processing.Model.Static;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Layout;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration;
using Rogue.NET.Core.Model.ScenarioConfiguration.Design;

namespace Rogue.NET.ScenarioEditor.Service
{
    [Export(typeof(IScenarioOverviewCalculationService))]
    public class ScenarioOverviewCalculationService : IScenarioOverviewCalculationService
    {
        readonly IScenarioCollectionProvider _scenarioCollectionProvider;
        readonly IScenarioOverviewViewModel _scenarioOverviewViewModel;
        readonly IScenarioSimulationService _scenarioSimulationService;

        [ImportingConstructor]
        public ScenarioOverviewCalculationService(
                IScenarioCollectionProvider scenarioCollectionProvider,
                IScenarioOverviewViewModel scenarioOverviewViewModel,
                IScenarioSimulationService scenarioSimulationService)
        {
            _scenarioCollectionProvider = scenarioCollectionProvider;
            _scenarioOverviewViewModel = scenarioOverviewViewModel;
            _scenarioSimulationService = scenarioSimulationService;
        }
        public void CalculateOverview(TemplateViewModel templateViewModel)
        {
            if (templateViewModel is LayoutTemplateViewModel)
            {
                // Show the projected generation quantity
                var projectionSet = _scenarioSimulationService.CalculateProjectedGeneration(_scenarioCollectionProvider.Levels, templateViewModel);

                // Set projections to the view model
                _scenarioOverviewViewModel.SetSeries(templateViewModel.Name + " Generation", projectionSet);
            }
            else if (templateViewModel is ConsumableTemplateViewModel)
            {
                // Show the projected generation quantity
                var projectionSet = _scenarioSimulationService.CalculateProjectedGeneration(_scenarioCollectionProvider.Levels, templateViewModel);

                // Set projections to the view model
                _scenarioOverviewViewModel.SetSeries(templateViewModel.Name + " Generation", projectionSet);
            }
            else if (templateViewModel is EquipmentTemplateViewModel)
            {
                // Show the projected generation quantity
                var projectionSet = _scenarioSimulationService.CalculateProjectedGeneration(_scenarioCollectionProvider.Levels, templateViewModel);

                // Set projections to the view model
                _scenarioOverviewViewModel.SetSeries(templateViewModel.Name + " Generation", projectionSet);
            }
            else if (templateViewModel is EnemyTemplateViewModel)
            {
                // Show the projected generation quantity
                var projectionSet = _scenarioSimulationService.CalculateProjectedGeneration(_scenarioCollectionProvider.Levels, templateViewModel);

                // Set projections to the view model
                _scenarioOverviewViewModel.SetSeries(templateViewModel.Name + " Generation", projectionSet);
            }
            else if (templateViewModel is FriendlyTemplateViewModel)
            {
                // Show the projected generation quantity
                var projectionSet = _scenarioSimulationService.CalculateProjectedGeneration(_scenarioCollectionProvider.Levels, templateViewModel);

                // Set projections to the view model
                _scenarioOverviewViewModel.SetSeries(templateViewModel.Name + " Generation", projectionSet);
            }
            else if (templateViewModel is DoodadTemplateViewModel)
            {
                // Show the projected generation quantity
                var projectionSet = _scenarioSimulationService.CalculateProjectedGeneration(_scenarioCollectionProvider.Levels, templateViewModel);

                // Set projections to the view model
                _scenarioOverviewViewModel.SetSeries(templateViewModel.Name + " Generation", projectionSet);
            }
            else
            {
                // Show the experience curve as the default
                var projectionSet = _scenarioSimulationService.CalculateProjectedExperience(_scenarioCollectionProvider.Levels);

                // Set projections to the view model
                _scenarioOverviewViewModel.SetSeries("Projected Experience", projectionSet);
            }
        }
    }
}
