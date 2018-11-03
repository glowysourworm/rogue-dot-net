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
        readonly IModelController _modelController;

        [ImportingConstructor]
        public ScenarioModule(
            IRegionManager regionManager,
            IEventAggregator eventAggregator,
            IScenarioController scenarioController,
            IModelController modelController)
        {
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;
            _scenarioController = scenarioController;
            _modelController = modelController;
        }

        public void Initialize()
        {
            _modelController.Initialize();

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
        //public void OnInitialized(IContainerProvider containerProvider)
        //{
        //    _regionManager.RegisterViewWithRegion("MainRegion", typeof(IntroView));
        //    _regionManager.RegisterViewWithRegion("MainRegion", typeof(GameSetupView));
        //    _regionManager.RegisterViewWithRegion("GameSetupRegion", typeof(NewOpenEdit));
        //    _regionManager.RegisterViewWithRegion("GameSetupRegion", typeof(ChooseParameters));
        //    _regionManager.RegisterViewWithRegion("GameSetupRegion", typeof(ChooseSavedGame));
        //    _regionManager.RegisterViewWithRegion("GameSetupRegion", typeof(ChooseScenario));
        //    _regionManager.RegisterViewWithRegion("MainRegion", () => containerProvider.Resolve<DeathDisplay>());
        //    _regionManager.RegisterViewWithRegion("MainRegion", () => containerProvider.Resolve<GameView>());
        //    _regionManager.RegisterViewWithRegion("GameRegion", () => containerProvider.Resolve<LevelView>());
        //    _regionManager.RegisterViewWithRegion("GameRegion", () => containerProvider.Resolve<EquipmentSelectionCtrl>());
        //    _regionManager.RegisterViewWithRegion("GameRegion", () => containerProvider.Resolve<DungeonEncyclopedia>());
        //    _regionManager.RegisterViewWithRegion("GameInfoRegion", () => containerProvider.Resolve<GameInfoView>());

        //    _regionManager.RegisterViewWithRegion("PlayerSubpanelEquipmentRegion", () =>
        //    {
        //        var view = containerProvider.Resolve<ItemGrid>();
        //        view.Mode = ItemGridModes.Equipment;
        //        return view;
        //    });

        //    _regionManager.RegisterViewWithRegion("EquipmentSelectionRegion", () =>
        //    {
        //        var view = containerProvider.Resolve<ItemGrid>();
        //        view.Mode = ItemGridModes.Equipment;
        //        return view;
        //    });

        //    _regionManager.RegisterViewWithRegion("PlayerSubpanelConsumablesRegion", () =>
        //    {
        //        var view = containerProvider.Resolve<ItemGrid>();
        //        view.Mode = ItemGridModes.Consumable;
        //        return view;
        //    });

        //    _regionManager.RegisterViewWithRegion("PlayerSubpanelInventoryRegion", () =>
        //    {
        //        var view = containerProvider.Resolve<ItemGrid>();
        //        view.Mode = ItemGridModes.Inventory;
        //        return view;
        //    });
        //}

        //public void RegisterTypes(IContainerRegistry containerRegistry)
        //{

        //}
    }
}

