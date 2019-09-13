using Prism.Mef.Modularity;
using Prism.Modularity;
using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.Core.Processing.Event.Backend;
using Rogue.NET.Core.Processing.Event.Backend.EventData;
using Rogue.NET.Core.Processing.Event.Core;
using Rogue.NET.Core.Processing.Event.Dialog.Enum;
using Rogue.NET.Core.Processing.Service.Interface;
using System.ComponentModel.Composition;

namespace Rogue.NET.Core
{
    [ModuleExport("CoreModule", typeof(CoreModule))]
    public class CoreModule : IModule
    {
        readonly IRogueEventAggregator _eventAggregator;
        readonly IScenarioResourceService _scenarioResourceService;

        [ImportingConstructor]
        public CoreModule(
            IRogueEventAggregator eventAggregator, 
            IScenarioResourceService scenarioResourceService)
        {
            _eventAggregator = eventAggregator;
            _scenarioResourceService = scenarioResourceService;
        }

        public void Initialize()
        {
            // Show the Splash Sceen
            _eventAggregator.GetEvent<SplashEvent>().Publish(new SplashEventData()
            {
                SplashAction = SplashAction.Show,
                SplashType = SplashEventType.Loading
            });

            _scenarioResourceService.LoadCustomConfigurations();

            // Hide the Splash Sceen
            _eventAggregator.GetEvent<SplashEvent>().Publish(new SplashEventData()
            {
                SplashAction = SplashAction.Hide,
                SplashType = SplashEventType.Loading
            });

            // Notify listeners
            _eventAggregator.GetEvent<ResourcesInitializedEvent>().Publish();
        }
    }
}
