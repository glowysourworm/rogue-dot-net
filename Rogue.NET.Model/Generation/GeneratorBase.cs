using Prism.Events;
using Rogue.NET.Common.Events.Splash;
using System.ComponentModel.Composition;

namespace Rogue.NET.Model.Generation
{
    [Export]
    public abstract class GeneratorBase
    {
        readonly IEventAggregator _eventAggregator;

        [ImportingConstructor]
        public GeneratorBase(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        protected virtual void PublishLoadingMessage(string message, double percentLoaded)
        {
            _eventAggregator.GetEvent<CreatingScenarioEvent>().Publish(new CreatingScenarioEventArgs()
            {
                Message = message,
                Progress = percentLoaded
            });
        }
    }
}
