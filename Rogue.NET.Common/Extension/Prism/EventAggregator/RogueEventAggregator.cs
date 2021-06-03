using Rogue.NET.Common.Collection;

using System;
using System.ComponentModel.Composition;
using System.Linq;

namespace Rogue.NET.Common.Extension.Prism.EventAggregator
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(IRogueEventAggregator))]
    public class RogueEventAggregator : IRogueEventAggregator
    {
        readonly SimpleDictionary<Type, RogueEventBase> _eventDict;

        public RogueEventAggregator()
        {
            _eventDict = new SimpleDictionary<Type, RogueEventBase>();
        }

        public TEventType GetEvent<TEventType>() where TEventType : RogueEventBase
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
