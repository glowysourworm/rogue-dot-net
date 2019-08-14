using Prism.Events;
using Prism.Mef.Modularity;
using Prism.Modularity;
using Prism.Regions;
using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.Common.Extension.Prism.RegionManager.Interface;
using System.ComponentModel.Composition;
using System.Windows;

namespace Rogue.NET
{
    [ModuleExport("RogueModule", typeof(RogueModule))]
    public class RogueModule : IModule
    {
        readonly IRogueEventAggregator _eventAggregator;
        readonly IRogueRegionManager _regionManager;

        [ImportingConstructor]
        public RogueModule(IRogueEventAggregator eventAggregator, IRogueRegionManager regionManager)
        {
            _eventAggregator = eventAggregator;
            _regionManager = regionManager;
        }

        public void Initialize()
        {
        }
    }
}
