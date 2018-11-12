using Prism.Events;
using Rogue.NET.Core.Service.Interface;
using Rogue.NET.Model.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Rogue.NET.Core.Service
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(IScenarioMessageService))]
    public class ScenarioMessageService : IScenarioMessageService
    {
        readonly IEventAggregator _eventAggregator;

        bool _blocked = false;

        [ImportingConstructor]
        public ScenarioMessageService(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        public void Block()
        {
            _blocked = true;
        }

        public void Publish(string message)
        {
            if (!_blocked)
                _eventAggregator.GetEvent<ScenarioMessageEvent>().Publish(message);
        }

        public void Publish(string message, params string[] format)
        {
        }

        public void PublishPlayerAdvancement(string header, IEnumerable<string> messages)
        {
        }

        public void UnBlock(bool send)
        {
            _blocked = false;

            // TODO: Send
        }
    }
}
