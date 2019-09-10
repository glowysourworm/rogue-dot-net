using System;
using System.Collections.Generic;
using System.Windows;

namespace Rogue.NET.Common.Extension.Prism.EventAggregator
{
    public class RogueEvent<T> : RogueEventBase
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
            var actions = _actions.Values.Copy();

            foreach (var action in actions)
                action.Invoke(payload);
        }

        public void UnSubscribe(string token)
        {
            _actions.Remove(token);
        }
    }
    public class RogueEvent : RogueEventBase
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
        public void UnSubscribe(string token)
        {
            _actions.Remove(token);
        }
        public void Publish()
        {
            var actions = _actions.Values.Copy();

            foreach (var action in actions)
                action.Invoke();
        }
    }
}
