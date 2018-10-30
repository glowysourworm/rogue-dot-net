using Prism.Events;
using Rogue.NET.Model.Scenario;
using System.Collections.Generic;

namespace Rogue.NET.Model.Events
{
    public class AnimationCompletedEventArgs : System.EventArgs
    {
        public AnimationReturnAction ReturnAction { get; set; }
        public Alteration Alteration { get; set; }
        public Character Source { get; set; }
        public List<Character> Targets { get; set; }
    }
    public class AnimationCompletedEvent : PubSubEvent<AnimationCompletedEventArgs>
    {

    }
}
