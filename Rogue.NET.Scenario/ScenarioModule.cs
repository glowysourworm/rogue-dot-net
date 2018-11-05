using Prism.Events;
using Prism.Mef.Modularity;
using Prism.Modularity;
using Prism.Regions;

using Rogue.NET.Common.Events.Scenario;
using Rogue.NET.Common.Events.ScenarioEditor;
using Rogue.NET.Common.Events.Splash;
using Rogue.NET.Model.Events;
using Rogue.NET.Scenario.Controller.Interface;
using Rogue.NET.Scenario.Events;

using System.ComponentModel.Composition;

namespace Rogue.NET.Scenario
{
    [ModuleExport("Scenario", typeof(ScenarioModule))]
    public class ScenarioModule : IModule
    {
        readonly IRegionManager _regionManager;
        readonly IEventAggregator _eventAggregator;
        readonly IScenarioController _scenarioController;
        readonly IGameController _gameController;

        [ImportingConstructor]
        public ScenarioModule(
            IRegionManager regionManager,
            IEventAggregator eventAggregator,
            IScenarioController scenarioController,
            IGameController gameController)
        {
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;
            _scenarioController = scenarioController;
            _gameController = gameController;
        }

        public void Initialize()
        {
            _gameController.Initialize();

            _eventAggregator.GetEvent<SplashUpdateEvent>().Publish(new SplashUpdateEventArgs()
            {
                Message = "Loading Scenario Module...",
                Progress = 50
            });
            _eventAggregator.GetEvent<LevelInitializedEvent>().Subscribe(() =>
            {
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

