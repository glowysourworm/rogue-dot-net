using Rogue.NET.Common.Collection;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rogue.NET.Common.Extension.Prism.EventAggregator
{
    public class RogueAsyncEvent<T> : RogueEventBase
    {
        readonly SimpleDictionary<RogueEventKey, Func<T, Task>> _functions;

        public RogueAsyncEvent()
        {
            _functions = new SimpleDictionary<RogueEventKey, Func<T, Task>>();
        }

        public string Subscribe(Func<T, Task> func, RogueEventPriority priority = RogueEventPriority.None)
        {
            var eventKey = new RogueEventKey(priority);

            _functions.Add(eventKey, func);

            return eventKey.Token;
        }

        public void UnSubscribe(string token)
        {
            var eventKey = _functions.Keys.FirstOrDefault(key => key.Token == token);

            if (eventKey == null)
                throw new Exception("Trying to unsubscribe from missing event token RogueAsyncEvent");

            _functions.Remove(eventKey);
        }

        public async Task Publish(T payload)
        {
            // Copying the collection because the functions may have subscriptions in them that modify the _functions
            // collection
            var functions = _functions.OrderBy(x => x.Key.Priority)
                                      .Select(x => x.Value)
                                      .Actualize();

            foreach (var function in functions)
                await function(payload);
        }
    }
}
