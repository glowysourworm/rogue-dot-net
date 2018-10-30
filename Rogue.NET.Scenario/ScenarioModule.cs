using Prism.Events;
using Prism.Mef.Modularity;
using Prism.Modularity;
using Prism.Regions;
using Rogue.NET.Common.Events.Splash;
using Rogue.NET.Intro.Views;
using Rogue.NET.Scenario.Content.Views;
using Rogue.NET.Scenario.Intro.Views.GameSetup;
using Rogue.NET.Scenario.Outro.Views;
using Rogue.NET.Scenario.Views;
using System.ComponentModel.Composition;

namespace Rogue.NET.Scenario
{
    [ModuleExport("Scenario", typeof(ScenarioModule))]
    public class ScenarioModule : IModule
    {
        readonly IRegionManager _regionManager;
        readonly IEventAggregator _eventAggregator;

        [ImportingConstructor]
        public ScenarioModule(
            IRegionManager regionManager,
            IEventAggregator eventAggregator)
        {
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;
        }

        public void Initialize()
        {
            _eventAggregator.GetEvent<SplashUpdateEvent>().Publish(new SplashUpdateEventArgs()
            {
                Message = "Loading Scenario Module...",
                Progress = 50
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

