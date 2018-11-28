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
using Rogue.NET.Common.Extension.Prism;
using Rogue.NET.Scenario.Content.ViewModel.ItemGrid;
using Rogue.NET.Scenario.ViewModel.ItemGrid;
using System.Windows.Threading;
using System;
using Prism.Modularity;
using System.Collections.Generic;

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

            var moduleManager = this.Container.GetExport<IModuleManager>().Value;

            moduleManager.LoadModuleCompleted += (obj, args) =>
            {

            };

            base.InitializeModules();

            Application.Current.MainWindow = this.Shell as Shell;
            Application.Current.MainWindow.Show();

            // Request Navigate
            var regionManager = this.Container.GetExport<IRegionManager>().Value;

            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                regionManager.RequestNavigate("MainRegion", "IntroView");

            }), DispatcherPriority.ApplicationIdle);
            
        }
        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();
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

        protected override IModuleCatalog CreateModuleCatalog()
        {
            return new ModuleCatalog(new List<ModuleInfo>()
            {
                new ModuleInfo("ScenarioModule", "ScenarioModule")
                {
                    InitializationMode = InitializationMode.WhenAvailable,
                    Ref = new Uri(typeof(ScenarioModule).Assembly.Location, UriKind.RelativeOrAbsolute).AbsoluteUri
                },
                new ModuleInfo("CoreModule", "CoreModule")
                {
                    InitializationMode = InitializationMode.WhenAvailable,
                    Ref = new Uri(typeof(CoreModule).Assembly.Location, UriKind.RelativeOrAbsolute).AbsoluteUri
                },
                new ModuleInfo("RogueModule", "RogueModule")
                {
                    InitializationMode = InitializationMode.WhenAvailable,
                    Ref = new Uri(typeof(RogueModule).Assembly.Location, UriKind.RelativeOrAbsolute).AbsoluteUri
                },
                new ModuleInfo("ScenarioEditorModule", "ScenarioEditorModule")
                {
                    InitializationMode = InitializationMode.WhenAvailable,
                    Ref = new Uri(typeof(ScenarioEditorModule).Assembly.Location, UriKind.RelativeOrAbsolute).AbsoluteUri
                }
            });
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
                view.IsDialogMode = false;
                view.DataContext = this.Container.GetExport<ItemGridViewModel>().Value.Equipment;
                return view;
            });

            regionManager.RegisterViewWithRegion("EquipmentSelectionRegion", () =>
            {
                var view = this.Container.GetExport<ItemGrid>().Value;
                view.Mode = ItemGridModes.Equipment;
                view.IntendedAction = ItemGridActions.Equip;
                view.IsDialogMode = false;
                view.DataContext = this.Container.GetExport<ItemGridViewModel>().Value.Equipment;
                return view;
            });

            regionManager.RegisterViewWithRegion("PlayerSubpanelConsumablesRegion", () =>
            {
                var view = this.Container.GetExport<ItemGrid>().Value;
                view.Mode = ItemGridModes.Consumable;
                view.IntendedAction = ItemGridActions.Consume;
                view.IsDialogMode = false;
                view.DataContext = this.Container.GetExport<ItemGridViewModel>().Value.Consumables;
                return view;
            });

            regionManager.RegisterViewWithRegion("PlayerSubpanelInventoryRegion", () =>
            {
                var view = this.Container.GetExport<ItemGrid>().Value;
                view.Mode = ItemGridModes.Inventory;
                view.IntendedAction = ItemGridActions.Drop;
                view.IsDialogMode = false;
                view.DataContext = this.Container.GetExport<ItemGridViewModel>().Value.Inventory;
                return view;
            });

            regionManager.RegisterViewWithRegion("UncurseItemGridRegion", () =>
            {
                var view = this.Container.GetExport<ItemGrid>().Value;
                view.Mode = ItemGridModes.Uncurse;
                view.IntendedAction = ItemGridActions.Uncurse;
                view.IsDialogMode = true;
                view.DataContext = this.Container.GetExport<ItemGridViewModel>().Value.UncurseEquipment;
                return view;
            });
            regionManager.RegisterViewWithRegion("ImbueItemGridRegion", () =>
            {
                var view = this.Container.GetExport<ItemGrid>().Value;
                view.Mode = ItemGridModes.Imbue;
                view.IntendedAction = ItemGridActions.Imbue;
                view.IsDialogMode = true;
                view.DataContext = this.Container.GetExport<ItemGridViewModel>().Value.ImbueEquipment;
                return view;
            });
            regionManager.RegisterViewWithRegion("IdentifyItemGridRegion", () =>
            {
                var view = this.Container.GetExport<ItemGrid>().Value;
                view.Mode = ItemGridModes.Identify;
                view.IntendedAction = ItemGridActions.Identify;
                view.IsDialogMode = true;
                view.DataContext = this.Container.GetExport<ItemGridViewModel>().Value.IdentifyInventory;
                return view;
            });
            regionManager.RegisterViewWithRegion("EnchantItemGridRegion", () =>
            {
                var view = this.Container.GetExport<ItemGrid>().Value;
                view.Mode = ItemGridModes.Enchant;
                view.IntendedAction = ItemGridActions.Enchant;
                view.IsDialogMode = true;
                view.DataContext = this.Container.GetExport<ItemGridViewModel>().Value.EnchantEquipment;
                return view;
            });
        }
    }
}