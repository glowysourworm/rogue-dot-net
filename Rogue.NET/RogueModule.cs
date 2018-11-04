using Prism.Events;
using Prism.Mef.Modularity;
using Prism.Modularity;
using Prism.Regions;
using Rogue.NET.Core.Event.Splash;
using Rogue.NET.Core.Logic.Processing.Enum;
using Rogue.NET.Splash;
using System.ComponentModel.Composition;

namespace Rogue.NET
{
    [ModuleExport("Rogue", typeof(RogueModule))]
    public class RogueModule : IModule
    {
        readonly IEventAggregator _eventAggregator;
        readonly IRegionManager _regionManager;

        SplashThread _splashThread;

        [ImportingConstructor]
        public RogueModule(IEventAggregator eventAggregator, IRegionManager regionManager)
        {
            _eventAggregator = eventAggregator;
            _regionManager = regionManager;
        }

        public void Initialize()
        {
            _eventAggregator.GetEvent<SplashEvent>().Subscribe((e) =>
            {
                if (e.SplashAction == SplashAction.Hide)
                    HideSplash();
                else
                    ShowSplash(e.SplashType);
            });
        }

        private void HideSplash()
        {
            if (_splashThread != null)
            {
                _splashThread.Stop();
                _splashThread = null;
            }
        }

        private void ShowSplash(SplashEventType type)
        {
            if (_splashThread != null)
                HideSplash();

            _splashThread = new SplashThread(_eventAggregator, type);
        }
    }
}
