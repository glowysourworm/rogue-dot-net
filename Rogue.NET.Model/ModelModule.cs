using Prism.Events;
using Prism.Ioc;
using Prism.Mef.Modularity;
using Prism.Modularity;
using Rogue.NET.Common.Events.Splash;
using Rogue.NET.Model.Generation;
using Rogue.NET.Model.Logic;
using Rogue.NET.Model.Physics;

namespace Rogue.NET.Model
{
    [ModuleExport("Model", typeof(ModelModule))]
    public class ModelModule : IModule
    {
        readonly IEventAggregator _eventAggregator;

        public ModelModule(
            IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        public void Initialize()
        {
            _eventAggregator.GetEvent<SplashUpdateEvent>().Publish(new SplashUpdateEvent()
            {
                Message = "Loading Model Module...",
                Progress = 30
            });

            // register singletons
            _unityContainer.RegisterInstance<ScenarioLogic>(new ScenarioLogic(_unityContainer, _eventAggregator));
            _unityContainer.RegisterInstance<MovementLogic>(new MovementLogic(_eventAggregator, _unityContainer, rayTracer));
            _unityContainer.RegisterInstance<InteractionLogic>(new InteractionLogic(_eventAggregator, _unityContainer));
            
            _unityContainer.Resolve(typeof(ModelController));

        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            throw new System.NotImplementedException();
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            throw new System.NotImplementedException();
        }
    }
}
