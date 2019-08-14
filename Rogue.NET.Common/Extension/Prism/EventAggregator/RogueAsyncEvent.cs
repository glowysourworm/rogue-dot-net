using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rogue.NET.Common.Extension.Prism.EventAggregator
{
    public class RogueAsyncEvent<T> : RogueEventBase
    {
        readonly IDictionary<string, Func<T, Task>> _functions;

        public RogueAsyncEvent()
        {
            _functions = new Dictionary<string, Func<T, Task>>();
        }

        public string Subscribe(Func<T, Task> func)
        {
            var token = Guid.NewGuid().ToString();

            _functions.Add(token, func);

            return token;
        }

        public async Task Publish(T payload)
        {
            foreach (var function in _functions.Values)
                await function(payload);
        }
    }
}
