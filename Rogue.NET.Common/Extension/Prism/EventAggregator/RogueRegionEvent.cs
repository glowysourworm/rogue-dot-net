using Rogue.NET.Common.Extension.Prism.RegionManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Rogue.NET.Common.Extension.Prism.EventAggregator
{
    /// <summary>
    /// Special event class used for Region loading to pass IRogueRegion instances to the
    /// IRogueRegionManager - which lies in the IModule implementations or I[something]Controller
    /// instances. (I like to centralize control of the region manager)
    /// </summary>
    public class RogueRegionEvent : RogueEventBase
    {
        readonly IDictionary<string, Action<RogueRegion>> _actions;

        public RogueRegionEvent()
        {
            _actions = new Dictionary<string, Action<RogueRegion>>();
        }

        public string Subscribe(Action<RogueRegion> action)
        {
            var token = Guid.NewGuid().ToString();

            _actions.Add(token, action);

            return token;
        }

        public void Publish(RogueRegion region)
        {
            foreach (var action in _actions.Values)
                action(region);
        }
    }

    /// <summary>
    /// Special event class used for Region loading to pass IRogueRegion instances to the
    /// IRogueRegionManager - which lies in the IModule implementations or I[something]Controller
    /// instances. (I like to centralize control of the region manager)
    /// </summary>
    public class RogueRegionEvent<T> : RogueEventBase
    {
        readonly IDictionary<string, Action<RogueRegion, T>> _actions;

        public RogueRegionEvent()
        {
            _actions = new Dictionary<string, Action<RogueRegion, T>>();
        }

        public string Subscribe(Action<RogueRegion, T> action)
        {
            var token = Guid.NewGuid().ToString();

            _actions.Add(token, action);

            return token;
        }

        public void Publish(RogueRegion region, T payload)
        {
            foreach (var action in _actions.Values)
                action(region, payload);
        }
    }
}
