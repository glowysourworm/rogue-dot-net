using System;
using System.Collections.Generic;
using System.Linq;

namespace Rogue.NET.Common.Extension.Prism.EventAggregator
{
    public class RogueEvent<T> : RogueEventBase
    {
        readonly IDictionary<RogueEventKey, Action<T>> _actions;

        public RogueEvent()
        {
            _actions = new Dictionary<RogueEventKey, Action<T>>();
        }

        public string Subscribe(Action<T> action, RogueEventPriority priority = RogueEventPriority.None)
        {
            var eventKey = new RogueEventKey(priority);

            _actions.Add(eventKey, action);

            return eventKey.Token;
        }

        public void UnSubscribe(string token)
        {
            var eventKey = _actions.Keys.FirstOrDefault(key => key.Token == token);

            if (eventKey == null)
                throw new Exception("Trying to unsubscribe from missing event token RogueEvent<T>");

            _actions.Remove(eventKey);
        }

        public void Publish(T payload)
        {
            // Copying the collection because the actions may have subscriptions in them that modify the _actions
            // collection
            var actions = _actions.OrderBy(x => x.Key.Priority)
                                  .Select(x => x.Value)
                                  .Actualize();

            foreach (var action in actions)
                action.Invoke(payload);
        }
    }
    public class RogueEvent : RogueEventBase
    {
        readonly IDictionary<RogueEventKey, Action> _actions;

        public RogueEvent()
        {
            _actions = new Dictionary<RogueEventKey, Action>();
        }

        public string Subscribe(Action action, RogueEventPriority priority = RogueEventPriority.None)
        {
            var eventKey = new RogueEventKey(priority);

            _actions.Add(eventKey, action);

            return eventKey.Token;
        }

        public void UnSubscribe(string token)
        {
            var eventKey = _actions.Keys.FirstOrDefault(key => key.Token == token);

            if (eventKey == null)
                throw new Exception("Trying to unsubscribe from missing event token RogueEvent");

            _actions.Remove(eventKey);
        }

        public void Publish()
        {
            // Copying the collection because the actions may have subscriptions in them that modify the _actions
            // collection
            var actions = _actions.OrderBy(x => x.Key.Priority)
                                  .Select(x => x.Value)
                                  .Actualize();

            foreach (var action in actions)
                action.Invoke();
        }
    }
}
