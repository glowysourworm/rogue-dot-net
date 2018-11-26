﻿using Prism.Events;
using Prism.Mef.Modularity;
using Prism.Modularity;
using Rogue.NET.Common.Events.Splash;
using Rogue.NET.Core.Event.Core;
using Rogue.NET.Core.Event.Splash;
using Rogue.NET.Core.Logic.Processing;
using Rogue.NET.Core.Logic.Processing.Enum;
using Rogue.NET.Core.Service.Interface;
using System.ComponentModel.Composition;

namespace Rogue.NET.Core
{
    [ModuleExport("Core", typeof(CoreModule))]
    public class CoreModule : IModule
    {
        readonly IEventAggregator _eventAggregator;
        readonly IScenarioResourceService _scenarioResourceService;

        [ImportingConstructor]
        public CoreModule(
            IEventAggregator eventAggregator, 
            IScenarioResourceService scenarioResourceService)
        {
            _eventAggregator = eventAggregator;
            _scenarioResourceService = scenarioResourceService;
        }

        public void Initialize()
        {
            // Show the Splash Sceen
            _eventAggregator.GetEvent<SplashEvent>().Publish(new SplashUpdate()
            {
                SplashAction = SplashAction.Show,
                SplashType = SplashEventType.Splash
            });

            _scenarioResourceService.LoadAllConfigurations();

            // Hide the Splash Sceen
            _eventAggregator.GetEvent<SplashEvent>().Publish(new SplashUpdate()
            {
                SplashAction = SplashAction.Hide,
                SplashType = SplashEventType.Splash
            });

            // Notify listeners
            _eventAggregator.GetEvent<ResourcesInitializedEvent>().Publish();
        }
    }
}
