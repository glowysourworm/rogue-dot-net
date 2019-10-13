using Prism.Mef.Modularity;
using Prism.Modularity;
using Rogue.NET.Common.Extension.Prism.EventAggregator;
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

        }
    }
}
