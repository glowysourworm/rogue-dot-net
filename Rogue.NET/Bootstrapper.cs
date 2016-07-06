using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.UnityExtensions;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using Rogue.NET.Common;
using Rogue.NET.Common.Events.Splash;
using Rogue.NET.Scenario;
using Rogue.NET.Model;
using Rogue.NET.Online;
using Rogue.NET.ScenarioEditor;
using Rogue.NET.Splash;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Rogue.NET.Splash.ViewModel;
using Rogue.NET.Splash.Views;
using PixelLab.Wpf.Transitions;

namespace Rogue.NET
{
    class Bootstrapper : UnityBootstrapper
    {
        private IRegionManager _regionManager;
        private IEventAggregator _eventAggregator;

        protected override DependencyObject CreateShell()
        {
            return ServiceLocator.Current.GetInstance<Shell>();
        }

        protected override void InitializeShell()
        {
            base.InitializeShell();

            var gameWindow = (Shell)this.Shell;
            gameWindow.EventAggregator = _eventAggregator;
            gameWindow.Container = this.Container;
            Application.Current.MainWindow = gameWindow;
        }

        protected override void InitializeModules()
        {
            base.InitializeModules();

            // initialize startup display
            _eventAggregator.GetEvent<SplashEvent>().Publish(new SplashEvent()
            {
                SplashAction = SplashAction.Hide,
                SplashType = SplashEventType.Splash
            });
            _regionManager.RequestNavigate("MainRegion", "IntroView");
            Application.Current.MainWindow.Show();
        }

        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();

            RegisterTypeIfMissing(typeof(IServiceLocator), typeof(UnityServiceLocatorAdapter), true);
            RegisterTypeIfMissing(typeof(IModuleInitializer), typeof(ModuleInitializer), true);
            RegisterTypeIfMissing(typeof(IModuleManager), typeof(ModuleManager), true);
            RegisterTypeIfMissing(typeof(RegionAdapterMappings), typeof(RegionAdapterMappings), true);
            RegisterTypeIfMissing(typeof(IRegionManager), typeof(RegionManager), true);
            RegisterTypeIfMissing(typeof(IEventAggregator), typeof(EventAggregator), true);
            RegisterTypeIfMissing(typeof(IRegionViewRegistry), typeof(RegionViewRegistry), true);
            RegisterTypeIfMissing(typeof(IRegionBehaviorFactory), typeof(RegionBehaviorFactory), true);
            RegisterTypeIfMissing(typeof(BorderRegionAdapter), typeof(BorderRegionAdapter), false);
            RegisterTypeIfMissing(typeof(TransitionPresenterRegionAdapater), typeof(TransitionPresenterRegionAdapater), false);

            _regionManager = (IRegionManager)this.Container.Resolve(typeof(IRegionManager));
            _eventAggregator = (IEventAggregator)this.Container.Resolve(typeof(IEventAggregator));
        }

        protected override IModuleCatalog CreateModuleCatalog()
        {
            return new AggregateModuleCatalog();
        }

        protected override RegionAdapterMappings ConfigureRegionAdapterMappings()
        {
            var mappings = base.ConfigureRegionAdapterMappings();

            mappings.RegisterMapping(typeof(Border), this.Container.Resolve<BorderRegionAdapter>());
            mappings.RegisterMapping(typeof(TransitionPresenter), this.Container.Resolve<TransitionPresenterRegionAdapater>());

            return mappings;
        }

        protected override void ConfigureModuleCatalog()
        {
            base.ConfigureModuleCatalog();

            this.ModuleCatalog.AddModule(new ModuleInfo()
            {
                InitializationMode = InitializationMode.OnDemand,
                ModuleName = "Splash",
                ModuleType = typeof(SplashModule).AssemblyQualifiedName,
                State = ModuleState.ReadyForInitialization
            });

            this.ModuleCatalog.AddModule(new ModuleInfo()
            {
                InitializationMode = InitializationMode.OnDemand,
                ModuleName = "Model",
                ModuleType = typeof(ModelModule).AssemblyQualifiedName,
                State = ModuleState.ReadyForInitialization
            });


            this.ModuleCatalog.AddModule(new ModuleInfo()
            {
                InitializationMode = InitializationMode.OnDemand,
                ModuleName = "Scenario",
                ModuleType = typeof(ScenarioModule).AssemblyQualifiedName,
                State = ModuleState.ReadyForInitialization
            });

            this.ModuleCatalog.AddModule(new ModuleInfo()
            {
                InitializationMode = InitializationMode.OnDemand,
                ModuleName = "Online",
                ModuleType = typeof(OnlineModule).AssemblyQualifiedName,
                State = ModuleState.ReadyForInitialization
            });

            this.ModuleCatalog.AddModule(new ModuleInfo()
            {
                InitializationMode = InitializationMode.OnDemand,
                ModuleName = "ScenarioEditor",
                ModuleType = typeof(ScenarioEditorModule).AssemblyQualifiedName,
                State = ModuleState.ReadyForInitialization
            });
        }
    }
}