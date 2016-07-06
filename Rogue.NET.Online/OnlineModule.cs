using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Modularity;
using Rogue.NET.Common.Events.Splash;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Rogue.NET.Online
{
    public class OnlineModule : IModule
    {
        readonly IEventAggregator _eventAggregator;

        public OnlineModule(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        public void Initialize()
        {
            _eventAggregator.GetEvent<SplashUpdateEvent>().Publish(new SplashUpdateEvent()
            {
                Message = "Loading Online Module...",
                Progress = 60
            });
        }
    }
}
