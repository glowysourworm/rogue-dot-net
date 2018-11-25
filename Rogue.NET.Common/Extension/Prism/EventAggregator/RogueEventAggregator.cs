using Prism.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace Rogue.NET.Common.Extension.Prism.EventAggregator
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(IEventAggregator))]
    public class RogueEventAggregator : IEventAggregator
    {
        readonly IDictionary<Type, EventBase> _eventDict;

        public RogueEventAggregator()
        {
            _eventDict = new Dictionary<Type, EventBase>();
        }

        public TEventType GetEvent<TEventType>() where TEventType : EventBase, new()
        {
            var type = typeof(TEventType);

            if (_eventDict.Keys.Any(x => x == typeof(TEventType)))
                return (TEventType)_eventDict[type];

            var newEvent = Construct<TEventType>();

            _eventDict[type] = newEvent;

            return newEvent;
        }

        private T Construct<T>()
        {
            var constructor = typeof(T).GetConstructor(new Type[] { });
            return (T)constructor.Invoke(new object[] { });
        }
    }
}
