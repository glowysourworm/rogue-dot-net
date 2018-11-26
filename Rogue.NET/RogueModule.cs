using Prism.Events;
using Prism.Mef.Modularity;
using Prism.Modularity;
using Prism.Regions;
using System.ComponentModel.Composition;
using System.Windows;

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
