using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.PubSubEvents;
using Microsoft.Practices.Unity;
using Rogue.NET.Common.Events.Splash;
using Rogue.NET.Model.Generation;
using Rogue.NET.Model.Logic;
using Rogue.NET.Model.Physics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Model
{
    public class ModelModule : IModule
    {
        readonly IUnityContainer _unityContainer;
        readonly IEventAggregator _eventAggregator;

        public ModelModule(
            IEventAggregator eventAggregator, 
            IUnityContainer unityContainer)
        {
            _unityContainer = unityContainer;
            _eventAggregator = eventAggregator;
        }

        public void Initialize()
        {
            _eventAggregator.GetEvent<SplashUpdateEvent>().Publish(new SplashUpdateEvent()
            {
                Message = "Loading Model Module...",
                Progress = 30
            });

            _unityContainer.RegisterType<IDungeonGenerator, ScenarioGenerator>(new ContainerControlledLifetimeManager());
            _unityContainer.RegisterType<IModelController, ModelController>(new ContainerControlledLifetimeManager());
            _unityContainer.RegisterType<IRayTracer, SimpleRayTracer>(new ContainerControlledLifetimeManager());

            var rayTracer = _unityContainer.Resolve<IRayTracer>();

            // register singletons
            _unityContainer.RegisterInstance<IResourceService>(new ResourceService(_eventAggregator));
            _unityContainer.RegisterInstance<ScenarioLogic>(new ScenarioLogic(_unityContainer, _eventAggregator));
            _unityContainer.RegisterInstance<MovementLogic>(new MovementLogic(_eventAggregator, _unityContainer, rayTracer));
            _unityContainer.RegisterInstance<InteractionLogic>(new InteractionLogic(_eventAggregator, _unityContainer));
            
            _unityContainer.Resolve(typeof(ModelController));

        }
    }
}
