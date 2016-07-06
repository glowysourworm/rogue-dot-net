using Microsoft.Practices.Prism.Events;
using Rogue.NET.Common.Events.Splash;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Model.Generation
{
    public abstract class GeneratorBase
    {
        readonly IEventAggregator _eventAggregator;

        public GeneratorBase(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        protected virtual void PublishLoadingMessage(string message, double percentLoaded)
        {
            _eventAggregator.GetEvent<CreatingScenarioEvent>().Publish(new CreatingScenarioEvent()
            {
                Message = message,
                Progress = percentLoaded
            });
        }
    }
}
