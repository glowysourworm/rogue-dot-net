using Prism.Events;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rogue.NET.Common.Extension.Prism.EventAggregator
{
    public class RogueAsyncEvent<T> : EventBase
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

        public override bool Contains(SubscriptionToken token)
        {
            throw new NotImplementedException();
        }
        public override void Unsubscribe(SubscriptionToken token)
        {
            throw new NotImplementedException();
        }
        protected override void InternalPublish(params object[] arguments)
        {
            throw new NotImplementedException();
        }
        protected override SubscriptionToken InternalSubscribe(IEventSubscription eventSubscription)
        {
            throw new NotImplementedException();
        }
    }
}
