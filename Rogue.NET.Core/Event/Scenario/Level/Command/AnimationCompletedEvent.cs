using Prism.Events;
using Rogue.NET.Core.Model.Scenario.Alteration;
using Rogue.NET.Core.Model.Scenario.Character;
using System.Collections.Generic;

namespace Rogue.NET.Model.Events
{
    public class AnimationCompletedEventArgs : System.EventArgs
    {
        public AlterationContainer Alteration { get; set; }
        public Character Source { get; set; }
        public List<Character> Targets { get; set; }
    }
    public class AnimationCompletedEvent : PubSubEvent<AnimationCompletedEventArgs>
    {

    }
}
