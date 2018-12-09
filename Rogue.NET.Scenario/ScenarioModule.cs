using Microsoft.Practices.ServiceLocation;
using Prism.Events;
using Prism.Mef.Modularity;
using Prism.Modularity;
using Prism.Regions;

using Rogue.NET.Common.Events.Scenario;
using Rogue.NET.Common.Events.ScenarioEditor;
using Rogue.NET.Common.Events.Splash;
using Rogue.NET.Core.Event.Scenario.Level.Event;
using Rogue.NET.Core.Logic.Processing.Enum;
using Rogue.NET.Core.Service.Interface;
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
using System.Windows;
using System.Linq;
using Rogue.NET.Scenario.Outro;

namespace Rogue.NET.Scenario
{
    [ModuleExport("ScenarioModule", typeof(ScenarioModule), InitializationMode = InitializationMode.WhenAvailable)]
    public class ScenarioModule : IModule
    {
        readonly IRegionManager _regionManager;
        readonly IEventAggregator _eventAggregator;
        readonly IScenarioController _scenarioController;
        readonly IGameController _gameController;
        readonly IScenarioResourceService _scenarioResourceService;
        readonly IScenarioFileService _scenarioFileService;

        [ImportingConstructor]
        public ScenarioModule(
            IRegionManager regionManager,
            IEventAggregator eventAggregator,
            IScenarioController scenarioController,
            IGameController gameController,
            IScenarioResourceService scenarioResourceService,
            IScenarioFileService scenarioFileService)
        {
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;
            _scenarioController = scenarioController;
            _gameController = gameController;
            _scenarioResourceService = scenarioResourceService;
            _scenarioFileService = scenarioFileService;
        }

        public void Initialize()
        {
            RegisterRegionViews();

            _gameController.Initialize();

            _eventAggregator.GetEvent<LevelLoadedEvent>().Subscribe(() =>
            {
                _regionManager.RequestNavigate("MainRegion", "GameView");
                _regionManager.RequestNavigate("GameRegion", "LevelView");
            });

            _eventAggregator.GetEvent<ScenarioUpdateEvent>().Subscribe(update =>
            {
                if (update.ScenarioUpdateType == ScenarioUpdateType.PlayerDeath)
                {
                    (_regionManager.Regions["MainRegion"]
                                   .Views
                                   .FirstOrDefault(x => x.GetType() == typeof(DeathDisplay)) as DeathDisplay)
                                   .DiedOfText = update.PlayerDeathMessage;

                    _regionManager.RequestNavigate("MainRegion", "DeathDisplay");
                }
                else if (update.ScenarioUpdateType == ScenarioUpdateType.ScenarioCompleted)
                {
                    // TODO!  Opacity needs to be reset!!!
                    (_regionManager.Regions["MainRegion"]
                                   .Views
                                   .FirstOrDefault(x => x.GetType() == typeof(OutroDisplay)) as OutroDisplay)
                                   .Opacity = 1;

                    _regionManager.RequestNavigate("MainRegion", "OutroDisplay");
                }

            });

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

            _eventAggregator.GetEvent<OutroFinishedEvent>().Subscribe(() =>
            {
                _regionManager.RequestNavigate("MainRegion", "IntroView");
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

            // Delete Scenario
            _eventAggregator.GetEvent<DeleteScenarioEvent>().Subscribe((e) =>
            {
                var result = MessageBox.Show("Are you sure you want to delete this scenario?", "Delete " + e.ScenarioName, MessageBoxButton.YesNoCancel);
                if (result == MessageBoxResult.Yes || result == MessageBoxResult.OK)
                {
                    // Delete the file
                    _scenarioFileService.DeleteScenario(e.ScenarioName);

                    // Notify listeners
                    _eventAggregator.GetEvent<ScenarioDeletedEvent>().Publish();
                }
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
            _regionManager.RegisterViewWithRegion("MainRegion", typeof(OutroDisplay));
            _regionManager.RegisterViewWithRegion("GameRegion", typeof(LevelView));
            _regionManager.RegisterViewWithRegion("GameRegion", typeof(EquipmentSelectionCtrl));
            _regionManager.RegisterViewWithRegion("GameRegion", typeof(DungeonEncyclopedia));
            _regionManager.RegisterViewWithRegion("LevelCanvasRegion", typeof(LevelCanvas));
            _regionManager.RegisterViewWithRegion("LevelCompassRegion", typeof(CompassCtrl));
            _regionManager.RegisterViewWithRegion("PlayerSubpanelRegion", typeof(PlayerSubpanel));
            _regionManager.RegisterViewWithRegion("PlayerSubpanelSkillsRegion", typeof(SkillGrid));
            _regionManager.RegisterViewWithRegion("PlayerSubpanelStatsRegion", typeof(StatsControl));
            _regionManager.RegisterViewWithRegion("PlayerSubpanelAlterationsRegion", typeof(AlterationCtrl));
            _regionManager.RegisterViewWithRegion("StatusCtrlRegion", typeof(StatusCtrl));
            _regionManager.RegisterViewWithRegion("ScenarioMessageRegion", typeof(ScenarioMessageView));
            _regionManager.RegisterViewWithRegion("PlayerStatusSmallPanelRegion", typeof(PlayerStatusSmallPanel));
        }
    }
}

