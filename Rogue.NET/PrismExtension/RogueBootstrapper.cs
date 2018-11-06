using Rogue.NET.Scenario;
using Rogue.NET.View;
using Rogue.NET.ScenarioEditor;
using Rogue.NET.Core;
using Rogue.NET.Common.ViewModel;

using Prism.Mef;
using Prism.Regions;

using System.Windows;
using System.Windows.Controls;
using System.Reflection;
using System.ComponentModel.Composition.Hosting;

using Microsoft.Practices.ServiceLocation;

using Rogue.NET.Scenario.Views;
using Rogue.NET.Intro.Views;
using Rogue.NET.Scenario.Intro.Views.GameSetup;
using Rogue.NET.Scenario.Outro.Views;
using Rogue.NET.Scenario.Content.Views;
using Rogue.NET.ScenarioEditor.Views;
using Rogue.NET.ScenarioEditor.Views.Assets.Animation;
using Rogue.NET.ScenarioEditor.Views.Assets.Consumable;
using Rogue.NET.ScenarioEditor.Views.Assets.Enemy;
using Rogue.NET.ScenarioEditor.Views.Assets;
using Rogue.NET.ScenarioEditor.Views.Assets.Equipment;
using Rogue.NET.ScenarioEditor.Views.Controls;
using Rogue.NET.ScenarioEditor.ViewModel;
using Rogue.NET.ScenarioEditor.Views.Assets.Spell;
using Rogue.NET.ScenarioEditor.Views.Construction;
using Rogue.NET.Common.Extension.Prism;
using System.Windows.Media;

namespace Rogue.NET.PrismExtension
{
    public class RogueBootstrapper : MefBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            return ServiceLocator.Current.GetInstance<Shell>();
        }

        protected override void InitializeModules()
        {
            RegisterRegionViews();

            base.InitializeModules();

            Application.Current.MainWindow = this.Shell as Shell;
            Application.Current.MainWindow.Show();

            // Request Navigate
            var regionManager = this.Container.GetExport<IRegionManager>().Value;

            regionManager.RequestNavigate("MainRegion", "IntroView");
        }

        protected override AggregateCatalog CreateAggregateCatalog()
        {
            var catalogs = new AssemblyCatalog[]
            {
                new AssemblyCatalog(Assembly.GetAssembly(typeof(RogueModule))),
                new AssemblyCatalog(Assembly.GetAssembly(typeof(ScenarioEditorModule))),
                new AssemblyCatalog(Assembly.GetAssembly(typeof(ScenarioModule))),
                new AssemblyCatalog(Assembly.GetAssembly(typeof(CoreModule))),
                new AssemblyCatalog(Assembly.GetAssembly(typeof(NotifyViewModel))) // Common
            };
            return new AggregateCatalog(catalogs);
        }

        protected override RegionAdapterMappings ConfigureRegionAdapterMappings()
        {
            var mappings = base.ConfigureRegionAdapterMappings();

            mappings.RegisterMapping(typeof(Border), this.Container.GetExportedValue<BorderRegionAdapter>());
            mappings.RegisterMapping(typeof(TransitionPresenter), this.Container.GetExportedValue<TransitionPresenterRegionAdapater>());

            return mappings;
        }

        private void RegisterRegionViews()
        {
            var regionManager = this.Container.GetExport<IRegionManager>().Value;

            // Put this here because was having some MAJOR issues getting MEF to work
            // with the version 7 PrismLibrary abstractions to inject IContainerProvider into
            // the IModule instances. So, I moved them all here to use this.Container

            // Item Grid Regions
            regionManager.RegisterViewWithRegion("UncurseItemGridRegion", () =>
            {
                var itemGrid = this.Container.GetExport<ItemGrid>().Value;
                itemGrid.Mode = ItemGridModes.Uncurse;
                return itemGrid;
            });
            regionManager.RegisterViewWithRegion("ImbueItemGridRegion", () =>
            {
                var itemGrid = this.Container.GetExport<ItemGrid>().Value;
                itemGrid.Mode = ItemGridModes.Imbue;
                return itemGrid;
            });
            regionManager.RegisterViewWithRegion("IdentifyItemGridRegion", () =>
            {
                var itemGrid = this.Container.GetExport<ItemGrid>().Value;
                itemGrid.Mode = ItemGridModes.Identify;
                return itemGrid;
            });
            regionManager.RegisterViewWithRegion("EnchantItemGridRegion", () =>
            {
                var itemGrid = this.Container.GetExport<ItemGrid>().Value;
                itemGrid.Mode = ItemGridModes.Identify;
                return itemGrid;
            });

            regionManager.RegisterViewWithRegion("MainRegion", typeof(IntroView));
            regionManager.RegisterViewWithRegion("MainRegion", typeof(GameSetupView));
            regionManager.RegisterViewWithRegion("GameSetupRegion", typeof(NewOpenEdit));
            regionManager.RegisterViewWithRegion("GameSetupRegion", typeof(ChooseParameters));
            regionManager.RegisterViewWithRegion("GameSetupRegion", typeof(ChooseSavedGame));
            regionManager.RegisterViewWithRegion("GameSetupRegion", typeof(ChooseScenario));
            regionManager.RegisterViewWithRegion("MainRegion", typeof(DeathDisplay));
            regionManager.RegisterViewWithRegion("MainRegion", typeof(GameView));
            regionManager.RegisterViewWithRegion("GameRegion", typeof(LevelView));
            regionManager.RegisterViewWithRegion("GameRegion", typeof(EquipmentSelectionCtrl));
            regionManager.RegisterViewWithRegion("GameRegion", typeof(DungeonEncyclopedia));
            regionManager.RegisterViewWithRegion("GameInfoRegion", typeof(GameInfoView));
            regionManager.RegisterViewWithRegion("LevelCanvasRegion", typeof(LevelCanvas));
            regionManager.RegisterViewWithRegion("PlayerSubpanelRegion", typeof(PlayerSubpanel));

            regionManager.RegisterViewWithRegion("PlayerSubpanelEquipmentRegion", () =>
            {
                var view = this.Container.GetExport<ItemGrid>().Value;
                view.Mode = ItemGridModes.Equipment;
                return view;
            });

            regionManager.RegisterViewWithRegion("EquipmentSelectionRegion", () =>
            {
                var view = this.Container.GetExport<ItemGrid>().Value;
                view.Mode = ItemGridModes.Equipment;
                return view;
            });

            regionManager.RegisterViewWithRegion("PlayerSubpanelConsumablesRegion", () =>
            {
                var view = this.Container.GetExport<ItemGrid>().Value;
                view.Mode = ItemGridModes.Consumable;
                return view;
            });

            regionManager.RegisterViewWithRegion("PlayerSubpanelInventoryRegion", () =>
            {
                var view = this.Container.GetExport<ItemGrid>().Value;
                view.Mode = ItemGridModes.Inventory;
                return view;
            });
        }
    }
}