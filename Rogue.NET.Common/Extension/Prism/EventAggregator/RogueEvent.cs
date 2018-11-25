using Prism.Events;
using System;
using System.Collections.Generic;

namespace Rogue.NET.Common.Extension.Prism.EventAggregator
{
    public class RogueEvent<T> : EventBase
    {
        readonly IDictionary<string, Action<T>> _actions;

        public RogueEvent()
        {
            _actions = new Dictionary<string, Action<T>>();
        }

        public string Subscribe(Action<T> action)
        {
            var token = Guid.NewGuid().ToString();

            _actions.Add(token, action);

            return token;
        }

        public void Publish(T payload)
        {
            foreach (var action in _actions.Values)
                action(payload);
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
    public class RogueEvent : EventBase
    {
        readonly IDictionary<string, Action> _actions;

        public RogueEvent()
        {
            _actions = new Dictionary<string, Action>();
        }

        public string Subscribe(Action action)
        {
            var token = Guid.NewGuid().ToString();

            _actions.Add(token, action);

            return token;
        }
        public void Unsubscribe(string token)
        {
            _actions.Remove(token);
        }
        public void Publish()
        {
            foreach (var action in _actions.Values)
                action.Invoke();
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
