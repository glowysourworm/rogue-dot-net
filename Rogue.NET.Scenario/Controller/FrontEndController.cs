using Prism.Events;
using Rogue.NET.Core.Event.Splash;
using Rogue.NET.Core.Logic.Processing.Interface;
using Rogue.NET.Model.Events;
using Rogue.NET.Scenario.Controller.Interface;
using System;
using System.ComponentModel.Composition;

namespace Rogue.NET.Scenario.Controller
{
    [Export(typeof(IFrontEndController))]
    public class FrontEndController : IFrontEndController
    {
        readonly IEventAggregator _eventAggregator;

        public RogueMessageQueue<IAnimationUpdate> AnimationMessages { get; private set; }
        public RogueMessageQueue<ISplashUpdate> SplashMessages { get; private set; }

        [ImportingConstructor]
        public FrontEndController(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;

            this.AnimationMessages = new RogueMessageQueue<IAnimationUpdate>();
            this.SplashMessages = new RogueMessageQueue<ISplashUpdate>();

            _eventAggregator.GetEvent<AnimationCompletedEvent>().Subscribe(update =>
            {
                // Output queue is always thread safe for writing
                this.AnimationMessages.OutputQueue.Enqueue(update);
            });

            _eventAggregator.GetEvent<SplashEvent>().Subscribe(update =>
            {
                // Output queue is always safe for writing
                this.SplashMessages.OutputQueue.Enqueue(update);
            });
        }

        public void PostInputMessage<T>(T message)
        {
            if (typeof(T) == typeof(IAnimationUpdate))
            {
                // Skip queueing message because we're going to process one at a time
                ProcessAnimationMessage(message as IAnimationUpdate);
            }
            else if (typeof(T) == typeof(ISplashUpdate))
            {
                // Skip queueing message because we're going to process one at a time
                ProcessSplashMessage(message as ISplashUpdate);
            }
            else
                throw new Exception("Messge type not supported IFrontEndController");
        }

        public T ReadOutputMessage<T>()
        {
            if (typeof(T) == typeof(IAnimationUpdate))
            {
                if (this.AnimationMessages.OutputQueue.Count > 0)
                    return (T)this.AnimationMessages.OutputQueue.Dequeue();
            }
            else if (typeof(T) == typeof(ISplashUpdate))
            {
                if (this.SplashMessages.OutputQueue.Count > 0)
                    return (T)this.SplashMessages.OutputQueue.Dequeue();
            }
            else
                throw new Exception("Messge type not supported IFrontEndController");

            return default(T);
        }

        private void ProcessAnimationMessage(IAnimationUpdate message)
        {
            _eventAggregator.GetEvent<AnimationStartEvent>().Publish(message);
        }
        private void ProcessSplashMessage(ISplashUpdate message)
        {
            _eventAggregator.GetEvent<SplashEvent>().Publish(message);
        }
    }
}
