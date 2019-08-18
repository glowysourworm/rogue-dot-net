using Prism.Mef.Modularity;
using Prism.Modularity;

using Rogue.NET.Common.Events.Scenario;
using Rogue.NET.Common.Events.ScenarioEditor;
using Rogue.NET.Core.Event.Scenario.Level.Event;
using Rogue.NET.Core.Logic.Processing.Enum;
using Rogue.NET.Core.Service.Interface;
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
using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.Common.Extension.Prism.RegionManager.Interface;
using Rogue.NET.Scenario.Constant;
using Rogue.NET.Scenario.Intro.Views;

namespace Rogue.NET.Scenario
{
    [ModuleExport("ScenarioModule", typeof(ScenarioModule), InitializationMode = InitializationMode.WhenAvailable)]
    public class ScenarioModule : IModule
    {
        readonly IRogueRegionManager _regionManager;
        readonly IRogueEventAggregator _eventAggregator;
        readonly IScenarioController _scenarioController;
        readonly IGameController _gameController;
        readonly IScenarioResourceService _scenarioResourceService;
        readonly IScenarioFileService _scenarioFileService;

        [ImportingConstructor]
        public ScenarioModule(
            IRogueRegionManager regionManager,
            IRogueEventAggregator eventAggregator,
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
            _gameController.Initialize();

            _eventAggregator.GetEvent<LevelLoadedEvent>().Subscribe(() =>
            {
                _regionManager.LoadSingleInstance(RegionName.MainRegion, typeof(GameView));
                _regionManager.LoadSingleInstance(RegionName.GameRegion, typeof(LevelView));
            });

            _eventAggregator.GetEvent<ScenarioUpdateEvent>().Subscribe(update =>
            {
                if (update.ScenarioUpdateType == ScenarioUpdateType.PlayerDeath)
                {
                    var view = _regionManager.LoadSingleInstance(RegionName.MainRegion, typeof(DeathDisplay));

                    (view as DeathDisplay).DiedOfText = update.PlayerDeathMessage;
                }
                else if (update.ScenarioUpdateType == ScenarioUpdateType.ScenarioCompleted)
                {
                    // TODO:REGIONMANAGER  Opacity needs to be reset!!!
                    //(_regionManager.Regions["MainRegion"]
                    //               .Views
                    //               .FirstOrDefault(x => x.GetType() == typeof(OutroDisplay)) as OutroDisplay)
                    //               .Opacity = 1;

                    var view = _regionManager.LoadSingleInstance(RegionName.MainRegion, typeof(OutroDisplay));
                }

            });

            _eventAggregator.GetEvent<ExitScenarioEvent>().Subscribe(() =>
            {
                _regionManager.LoadSingleInstance(RegionName.MainRegion, typeof(GameSetupView));
                _regionManager.LoadSingleInstance(RegionName.GameSetupRegion, typeof(NewOpenEdit));
            });

            _eventAggregator.GetEvent<ExitScenarioEditorEvent>().Subscribe(() =>
            {
                _regionManager.LoadSingleInstance(RegionName.MainRegion, typeof(GameSetupView));
                _regionManager.LoadSingleInstance(RegionName.GameSetupRegion, typeof(NewOpenEdit));
            });

            _eventAggregator.GetEvent<IntroFinishedEvent>().Subscribe(() =>
            {
                _regionManager.LoadSingleInstance(RegionName.MainRegion, typeof(GameSetupView));
                _regionManager.LoadSingleInstance(RegionName.GameSetupRegion, typeof(NewOpenEdit));
            });

            _eventAggregator.GetEvent<GameSetupDisplayFinished>().Subscribe((e) =>
            {
                _regionManager.LoadSingleInstance(RegionName.GameSetupRegion, e.NextDisplayType);
            });

            _eventAggregator.GetEvent<RequestNaviateParametersDisplayEvent>().Subscribe((type) =>
            {
                _regionManager.LoadSingleInstance(RegionName.ChooseParametersRegion, type);
            });

            _eventAggregator.GetEvent<OutroFinishedEvent>().Subscribe(() =>
            {
                _regionManager.LoadSingleInstance(RegionName.MainRegion, typeof(IntroView));
            });

            _eventAggregator.GetEvent<RequestNavigateToLevelViewEvent>().Subscribe(() =>
            {
                _regionManager.LoadSingleInstance(RegionName.GameRegion, typeof(LevelView));
            });
            _eventAggregator.GetEvent<RequestNavigateToEquipmentSelectionEvent>().Subscribe(() =>
            {
                _regionManager.LoadSingleInstance(RegionName.GameRegion, typeof(EquipmentSelectionCtrl));
            });
            _eventAggregator.GetEvent<RequestNavigateToEncyclopediaEvent>().Subscribe(() =>
            {
                _regionManager.LoadSingleInstance(RegionName.GameRegion, typeof(DungeonEncyclopedia));
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

            RegisterViews();
        }

        private void RegisterViews()
        {
            // RogueRegionManager does not require that views are pre-registered. There are several
            // ways to load a view into a region; but those will instantiate the view when they're called.
            //
            // This can cause issues because many event aggregator subscriptions are handled in the 
            // constructors. 
            //
            // The Prism region manager handles this using pre-registration; but the design is limited to
            // singleton regions. Sharing view instances led to problems in the scenario editor - so I
            // created a region manager that uses region instances to register views.
            //
            // Leaving these (below) views un-registered caused problems when loading scenario view models
            // because most of the event-aggregator events had already been called to initialize the view.

            // I'm thinking that loading the primary game view may be enough to get all the child-views 
            // instantiated.. but will have to see how it works.
            _regionManager.PreRegisterView(RegionName.MainRegion, typeof(GameView));
            _regionManager.PreRegisterView(RegionName.GameRegion, typeof(LevelView));
            _regionManager.PreRegisterView(RegionName.GameRegion, typeof(EquipmentSelectionCtrl));
            _regionManager.PreRegisterView(RegionName.GameRegion, typeof(DungeonEncyclopedia));
        }
    }
}

