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
using Rogue.NET.Common.Extension.Prism;
using Rogue.NET.Scenario.Content.ViewModel.ItemGrid;
using Rogue.NET.Scenario.ViewModel.ItemGrid;

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
            RegisterViewsUsingContainer();

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

        // Couldn't figure out an easy way to do this with injection.. or to call the container in the code
        private void RegisterViewsUsingContainer()
        {
            var regionManager = ServiceLocator.Current.GetInstance<IRegionManager>();

            // Have to use composition container to be  able to resolve the view here. The 
            // other way to do this is to inject a service or view model.
            regionManager.RegisterViewWithRegion("PlayerSubpanelEquipmentRegion", () =>
            {
                var view = this.Container.GetExport<ItemGrid>().Value;
                view.Mode = ItemGridModes.Equipment;
                view.IntendedAction = ItemGridActions.Equip;
                view.DataContext = this.Container.GetExport<ItemGridViewModel>().Value.Equipment;
                return view;
            });

            regionManager.RegisterViewWithRegion("EquipmentSelectionRegion", () =>
            {
                var view = this.Container.GetExport<ItemGrid>().Value;
                view.Mode = ItemGridModes.Equipment;
                view.IntendedAction = ItemGridActions.Equip;
                view.DataContext = this.Container.GetExport<ItemGridViewModel>().Value.Equipment;
                return view;
            });

            regionManager.RegisterViewWithRegion("PlayerSubpanelConsumablesRegion", () =>
            {
                var view = this.Container.GetExport<ItemGrid>().Value;
                view.Mode = ItemGridModes.Consumable;
                view.IntendedAction = ItemGridActions.Consume;
                view.DataContext = this.Container.GetExport<ItemGridViewModel>().Value.Consumables;
                return view;
            });

            regionManager.RegisterViewWithRegion("PlayerSubpanelInventoryRegion", () =>
            {
                var view = this.Container.GetExport<ItemGrid>().Value;
                view.Mode = ItemGridModes.Inventory;
                view.IntendedAction = ItemGridActions.Drop;
                view.DataContext = this.Container.GetExport<ItemGridViewModel>().Value.Inventory;
                return view;
            });

            regionManager.RegisterViewWithRegion("UncurseItemGridRegion", () =>
            {
                var view = this.Container.GetExport<ItemGrid>().Value;
                view.Mode = ItemGridModes.Uncurse;
                view.IntendedAction = ItemGridActions.Uncurse;
                view.DataContext = this.Container.GetExport<ItemGridViewModel>().Value.UncurseEquipment;
                return view;
            });
            regionManager.RegisterViewWithRegion("ImbueItemGridRegion", () =>
            {
                var view = this.Container.GetExport<ItemGrid>().Value;
                view.Mode = ItemGridModes.Imbue;
                view.IntendedAction = ItemGridActions.Imbue;
                view.DataContext = this.Container.GetExport<ItemGridViewModel>().Value.ImbueEquipment;
                return view;
            });
            regionManager.RegisterViewWithRegion("IdentifyItemGridRegion", () =>
            {
                var view = this.Container.GetExport<ItemGrid>().Value;
                view.Mode = ItemGridModes.Identify;
                view.IntendedAction = ItemGridActions.Identify;
                view.DataContext = this.Container.GetExport<ItemGridViewModel>().Value.IdentifyInventory;
                return view;
            });
            regionManager.RegisterViewWithRegion("EnchantItemGridRegion", () =>
            {
                var view = this.Container.GetExport<ItemGrid>().Value;
                view.Mode = ItemGridModes.Enchant;
                view.IntendedAction = ItemGridActions.Enchant;
                view.DataContext = this.Container.GetExport<ItemGridViewModel>().Value.EnchantEquipment;
                return view;
            });
        }
    }
}