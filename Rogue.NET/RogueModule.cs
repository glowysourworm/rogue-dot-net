using Prism.Events;
using Prism.Mef.Modularity;
using Prism.Modularity;
using Prism.Regions;
using Rogue.NET.Core.Event.Splash;
using Rogue.NET.Core.Logic.Processing.Enum;
using Rogue.NET.Splash;
using Rogue.NET.Splash.ViewModel;
using Rogue.NET.View;
using Rogue.NET.ViewModel;
using System;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Media;

namespace Rogue.NET
{
    [ModuleExport("Rogue", typeof(RogueModule))]
    public class RogueModule : IModule
    {
        readonly IEventAggregator _eventAggregator;
        readonly IRegionManager _regionManager;

        Window _splashWindow;

        [ImportingConstructor]
        public RogueModule(IEventAggregator eventAggregator, IRegionManager regionManager)
        {
            _eventAggregator = eventAggregator;
            _regionManager = regionManager;
        }

        public void Initialize()
        {
        }
    }
}
