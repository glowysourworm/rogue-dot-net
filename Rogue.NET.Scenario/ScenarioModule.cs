using Microsoft.Practices.ServiceLocation;
using Prism.Events;
using Prism.Mef.Modularity;
using Prism.Modularity;
using Prism.Regions;

using Rogue.NET.Common.Events.Scenario;
using Rogue.NET.Common.Events.ScenarioEditor;
using Rogue.NET.Common.Events.Splash;
using Rogue.NET.Intro.Views;
using Rogue.NET.Model.Events;
using Rogue.NET.Scenario.Content.Views;
using Rogue.NET.Scenario.Controller.Interface;
using Rogue.NET.Scenario.Events;
using Rogue.NET.Scenario.Events.Content;
using Rogue.NET.Scenario.Intro.Views.GameSetup;
using Rogue.NET.Scenario.Outro.Views;
using Rogue.NET.Scenario.Views;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Windows;

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

            // Halt back end threads on application exit
            Application.Current.Exit += (sender, e) => { _scenarioController.Stop(); };
        }

        public void Initialize()
        {
            RegisterRegionViews();

            _scenarioController.Initialize();
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

            _eventAggregator.GetEvent<RequestNavigateToLevelViewEvent>().Subscribe(() =>
            {
                _regionManager.RequestNavigate("GameRegion", "LevelView");
            });
            _eventAggregator.GetEvent<RequestNavigateToEquipmentSelectionEvent>().Subscribe(() =>
            {
                _regionManager.RequestNavigate("GameRegion", "EquipmentSelectionCtrl");
            });
            _eventAggregator.GetEvent<RequestNavigateToEncyclopediaEvent>().Subscribe(() =>
            {
                _regionManager.RequestNavigate("GameRegion", "DungeonEncyclopedia");
            });
        }

        private void RegisterRegionViews()
        {
            _regionManager.RegisterViewWithRegion("MainRegion", typeof(IntroView));
            _regionManager.RegisterViewWithRegion("MainRegion", typeof(GameSetupView));
            _regionManager.RegisterViewWithRegion("GameSetupRegion", typeof(NewOpenEdit));
            _regionManager.RegisterViewWithRegion("GameSetupRegion", typeof(ChooseParameters));
            _regionManager.RegisterViewWithRegion("GameSetupRegion", typeof(ChooseSavedGame));
            _regionManager.RegisterViewWithRegion("GameSetupRegion", typeof(ChooseScenario));
            _regionManager.RegisterViewWithRegion("MainRegion", typeof(DeathDisplay));
            _regionManager.RegisterViewWithRegion("MainRegion", typeof(GameView));
            _regionManager.RegisterViewWithRegion("GameRegion", typeof(LevelView));
            _regionManager.RegisterViewWithRegion("GameRegion", typeof(EquipmentSelectionCtrl));
            _regionManager.RegisterViewWithRegion("GameRegion", typeof(DungeonEncyclopedia));
            _regionManager.RegisterViewWithRegion("GameInfoRegion", typeof(GameInfoView));
            _regionManager.RegisterViewWithRegion("LevelCanvasRegion", typeof(LevelCanvas));
            _regionManager.RegisterViewWithRegion("PlayerSubpanelRegion", typeof(PlayerSubpanel));
            _regionManager.RegisterViewWithRegion("PlayerSubpanelSkillsRegion", typeof(SkillGrid));
            _regionManager.RegisterViewWithRegion("PlayerSubpanelStatsRegion", typeof(StatsControl));
            _regionManager.RegisterViewWithRegion("StatusCtrlRegion", typeof(StatusCtrl));
        }


    }
}

