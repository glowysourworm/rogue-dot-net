using Rogue.NET.Common.Events.Splash;
using Rogue.NET.Splash.ViewModel;
using Rogue.NET.Splash.Views;

namespace Rogue.NET.Splash
{
    public class SplashModule : IModule
    {
        readonly IEventAggregator _eventAggregator;
        readonly IRegionManager _regionManager;

        public SplashModule(
            IUnityContainer unityContainer, 
            IEventAggregator eventAggregator,
            IRegionManager regionManager)
        {
            _regionManager = regionManager;
            _unityContainer = unityContainer;
            _eventAggregator = eventAggregator;
        }

        public void Initialize()
        {
            _unityContainer.RegisterType<SplashWindow, SplashWindow>(new ContainerControlledLifetimeManager());
            _unityContainer.RegisterType<CreatingScenarioWindow, CreatingScenarioWindow>(new PerThreadLifetimeManager());

            _unityContainer.RegisterType<SplashController, SplashController>(new ContainerControlledLifetimeManager());
            _unityContainer.Resolve<SplashController>();

            _unityContainer.RegisterType<EnchantWindow, EnchantWindow>(new PerResolveLifetimeManager());
            _unityContainer.RegisterType<IdentifyWindow, IdentifyWindow>(new PerResolveLifetimeManager());
            _unityContainer.RegisterType<ImbueWindow, ImbueWindow>(new PerResolveLifetimeManager());
            _unityContainer.RegisterType<UncurseWindow, UncurseWindow>(new PerResolveLifetimeManager());

            // show main splash
            _eventAggregator.GetEvent<SplashEvent>().Publish(new SplashEvent()
            {
                SplashAction = SplashAction.Show,
                SplashType = SplashEventType.Splash
            });
        }
    }
}
