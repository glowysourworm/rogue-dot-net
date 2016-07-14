using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.PubSubEvents;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;
using Rogue.NET.Common;
using Rogue.NET.Common.Events.Splash;
using Rogue.NET.Intro.ViewModel;
using Rogue.NET.Intro.Views;
using Rogue.NET.Model;
using Rogue.NET.Scenario;
using Rogue.NET.Scenario.Content.Views;
using Rogue.NET.Scenario.Intro.Views.GameSetup;
using Rogue.NET.Scenario.Outro.Views;
using Rogue.NET.Scenario.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Rogue.NET.Scenario
{
    public class ScenarioModule : IModule
    {
        readonly IRegionManager _regionManager;
        readonly IUnityContainer _unityContainer;
        readonly IEventAggregator _eventAggregator;

        public ScenarioModule(
            IRegionManager regionManager,
            IUnityContainer unityContainer,
            IEventAggregator eventAggregator)
        {
            _regionManager = regionManager;
            _unityContainer = unityContainer;
            _eventAggregator = eventAggregator;
        }

        public void Initialize()
        {
            _eventAggregator.GetEvent<SplashUpdateEvent>().Publish(new SplashUpdateEvent()
            {
                Message = "Loading Scenario Module...",
                Progress = 50
            });


            _unityContainer.RegisterType<GameSetupViewModel, GameSetupViewModel>(new ContainerControlledLifetimeManager());
            _unityContainer.RegisterType<Consumables, Consumables>();
            _unityContainer.RegisterType<EquipmentCtrl, EquipmentCtrl>();
            _unityContainer.RegisterType<ItemGrid, ItemGrid>();
            _unityContainer.RegisterType<LevelView, LevelView>(new ContainerControlledLifetimeManager());

            // singletons
            _unityContainer.RegisterInstance(typeof(ScenarioController), new ScenarioController(_unityContainer, _eventAggregator, _regionManager));
            _unityContainer.RegisterInstance(typeof(GameView), new GameView(_regionManager, _unityContainer, _eventAggregator));
            //_unityContainer.RegisterInstance(typeof(LevelView), new LevelView());
            _unityContainer.RegisterInstance(typeof(EquipmentSelectionCtrl), new EquipmentSelectionCtrl());
            _unityContainer.RegisterInstance(typeof(ShopCtrl), new ShopCtrl());
            _unityContainer.RegisterInstance(typeof(DungeonEncyclopedia), new DungeonEncyclopedia());
            _unityContainer.RegisterInstance(typeof(DeathDisplay), new DeathDisplay(_eventAggregator));
            _unityContainer.RegisterInstance(typeof(GameInfoView), new GameInfoView(_eventAggregator));

            _regionManager.RegisterViewWithRegion("MainRegion", typeof(IntroView));
            _regionManager.RegisterViewWithRegion("MainRegion", typeof(GameSetupView));
            _regionManager.RegisterViewWithRegion("GameSetupRegion", typeof(NewOpenEdit));
            _regionManager.RegisterViewWithRegion("GameSetupRegion", typeof(ChooseParameters));
            _regionManager.RegisterViewWithRegion("GameSetupRegion", typeof(ChooseSavedGame));
            _regionManager.RegisterViewWithRegion("GameSetupRegion", typeof(ChooseScenario));
            _regionManager.RegisterViewWithRegion("MainRegion", () => _unityContainer.Resolve<DeathDisplay>());
            _regionManager.RegisterViewWithRegion("MainRegion", () => _unityContainer.Resolve<GameView>());
            //_regionManager.RegisterViewWithRegion("GameRegion", () => _unityContainer.Resolve<LevelView>());
            _regionManager.RegisterViewWithRegion("GameRegion", typeof(LevelView));
            _regionManager.RegisterViewWithRegion("GameRegion", () => _unityContainer.Resolve<EquipmentSelectionCtrl>());
            _regionManager.RegisterViewWithRegion("GameRegion", () => _unityContainer.Resolve<ShopCtrl>());
            _regionManager.RegisterViewWithRegion("GameRegion", () => _unityContainer.Resolve<DungeonEncyclopedia>());
            _regionManager.RegisterViewWithRegion("GameInfoRegion", () => _unityContainer.Resolve<GameInfoView>());

            _regionManager.RegisterViewWithRegion("ConsumablesRegion", () => 
            {
                var view = _unityContainer.Resolve<ItemGrid>();
                view.TheGrid.Height = 300; // scroll bars...
                view.Mode = ItemGridModes.Consumable;
                return view;
            });

            _regionManager.RegisterViewWithRegion("EquipmentRegion", () =>
            {
                var view = _unityContainer.Resolve<ItemGrid>();
                view.Mode = ItemGridModes.Equipment;
                return view;
            });

            _regionManager.RegisterViewWithRegion("InventoryRegion", () =>
            {
                var view = _unityContainer.Resolve<ItemGrid>();
                view.TheGrid.Height = 300; // scroll bars...
                view.Mode = ItemGridModes.Inventory;
                return view;
            });
        }
    }
}

