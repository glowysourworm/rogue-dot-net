using Rogue.NET.Model.Events;
using Rogue.NET.Scenario.Views;
using Rogue.NET.Scenario.Outro.Views;
using Rogue.NET.Common.Events.Scenario;
using Rogue.NET.Common.Events.ScenarioEditor;
using Rogue.NET.Scenario.Events;

using Prism.Events;
using Prism.Regions;
using System.ComponentModel.Composition;

namespace Rogue.NET.Scenario
{
    [Export]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class ScenarioController
    {
        readonly IEventAggregator _eventAggregator;
        readonly IRegionManager _regionManager;

        public ScenarioController(
            IEventAggregator eventAggregator,
            IRegionManager regionManager)
        {
            _eventAggregator = eventAggregator;
            _regionManager = regionManager;

            Initialize();
        }
        private void Initialize()
        {
            _eventAggregator.GetEvent<LevelInitializedEvent>().Subscribe((e) =>
            {
                var view = _regionManager.Regions["GameView"].Context = e;

                _regionManager.RequestNavigate("MainRegion", "GameView");
                _regionManager.RequestNavigate("GameRegion", "LevelView");
                _regionManager.RequestNavigate("GameInfoRegion", "GameInfoView");

            }, true);

            _eventAggregator.GetEvent<PlayerDiedEvent>().Subscribe((e) =>
            {
                _regionManager.Regions["DeathDisplay"].Context = e;

                _regionManager.RequestNavigate("MainRegion", "DeathDisplay");
            }, true);

            _eventAggregator.GetEvent<ExitScenarioEvent>().Subscribe(() =>
            {
                _regionManager.RequestNavigate("MainRegion", "GameSetupView");
                _regionManager.RequestNavigate("GameSetupRegion", "NewOpenEdit");
            });

            _eventAggregator.GetEvent<ExitScenarioEditorEvent>().Subscribe(() =>
            {
                _regionManager.RequestNavigate("MainRegion", "GameSetupView");
                _regionManager.RequestNavigate("GameSetupRegion", "NewOpenEdit");
            });

            _eventAggregator.GetEvent<IntroFinishedEvent>().Subscribe(() =>
            {
                _regionManager.RequestNavigate("MainRegion", "GameSetupView");
                _regionManager.RequestNavigate("GameSetupRegion", "NewOpenEdit");
            });

            _eventAggregator.GetEvent<GameSetupDisplayFinished>().Subscribe((e) =>
            {
                _regionManager.RequestNavigate("GameSetupRegion", e.NextDisplayType.Name);
            });
        }
    }
}
