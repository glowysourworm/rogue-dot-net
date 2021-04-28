using Microsoft.Practices.ServiceLocation;

using Prism.Mef;
using Prism.Modularity;
using Prism.Regions;

using Rogue.NET.Common.Extension.Prism.RegionManager.Interface;
using Rogue.NET.Common.ViewModel;
using Rogue.NET.Core;
using Rogue.NET.Scenario;
using Rogue.NET.Scenario.Constant;
using Rogue.NET.Scenario.Intro.Views;
using Rogue.NET.ScenarioEditor;
using Rogue.NET.View;

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Rogue.NET.PrismExtension
{
    public class RogueBootstrapper : MefBootstrapper
    {
        public RogueBootstrapper()
        {
        }

        protected override DependencyObject CreateShell()
        {
            return ServiceLocator.Current.GetInstance<Shell>();
        }

        protected override void InitializeModules()
        {
            var moduleManager = this.Container.GetExport<IModuleManager>().Value;

            moduleManager.LoadModuleCompleted += (obj, args) =>
            {

            };

            base.InitializeModules();

            Application.Current.MainWindow = this.Shell as Shell;
            Application.Current.MainWindow.Show();

            // Request Navigate
            var regionManager = this.Container.GetExport<IRogueRegionManager>().Value;

            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                regionManager.LoadSingleInstance(RegionName.MainRegion, typeof(IntroView));

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
    }
}