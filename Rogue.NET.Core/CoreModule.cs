using Prism.Events;
using Prism.Mef.Modularity;
using Prism.Modularity;
using System.ComponentModel.Composition;

namespace Rogue.NET.Core
{
    [ModuleExport("Core", typeof(CoreModule))]
    public class CoreModule : IModule
    {
        readonly IEventAggregator _eventAggregator;

        [ImportingConstructor]
        public CoreModule(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        public void Initialize()
        {
        }

        //public void Initialize()
        //{
        //    _eventAggregator.GetEvent<SplashUpdateEvent>().Publish(new SplashUpdateEvent()
        //    {
        //        Message = "Loading Model Module...",
        //        Progress = 30
        //    });

        //    // register singletons
        //    _unityContainer.RegisterInstance<ScenarioLogic>(new ScenarioLogic(_unityContainer, _eventAggregator));
        //    _unityContainer.RegisterInstance<MovementLogic>(new MovementLogic(_eventAggregator, _unityContainer, rayTracer));
        //    _unityContainer.RegisterInstance<InteractionLogic>(new InteractionLogic(_eventAggregator, _unityContainer));

        //    _unityContainer.Resolve(typeof(ModelController));

        //}
    }
}
