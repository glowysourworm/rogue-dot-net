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

        // TODO: MAKE THIS EXPLICIT IN THE BOOTSTRAPPER
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

            // Show progress 25%
            _eventAggregator.GetEvent<SplashUpdateEvent>().Publish(new SplashUpdateEventArgs()
            {
                Message = "Loading Fighter Scenario Configuration...",
                Progress = 25
            });
            _scenarioResourceService.LoadScenarioConfiguration(Model.Enums.ConfigResources.Fighter);

            // Show progress 50%
            _eventAggregator.GetEvent<SplashUpdateEvent>().Publish(new SplashUpdateEventArgs()
            {
                Message = "Loading Paladin Scenario Configuration...",
                Progress = 50
            });
            _scenarioResourceService.LoadScenarioConfiguration(Model.Enums.ConfigResources.Paladin);

            // Show progress 75%
            _eventAggregator.GetEvent<SplashUpdateEvent>().Publish(new SplashUpdateEventArgs()
            {
                Message = "Loading Witch Scenario Configuration...",
                Progress = 75
            });
            _scenarioResourceService.LoadScenarioConfiguration(Model.Enums.ConfigResources.Witch);

            // Show progress 100%
            _eventAggregator.GetEvent<SplashUpdateEvent>().Publish(new SplashUpdateEventArgs()
            {
                Message = "Loading Sorcerer Scenario Configuration...",
                Progress = 100
            });
            _scenarioResourceService.LoadScenarioConfiguration(Model.Enums.ConfigResources.Sorcerer);

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