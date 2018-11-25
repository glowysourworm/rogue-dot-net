using Prism.Events;
using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.Core.Model.Scenario.Character;

namespace Rogue.NET.Model.Events
{
    public class EnemyTargetedEventArgs : System.EventArgs
    {
        public Enemy TargetedEnemy { get; set; }
        public bool TargetingEnded { get; set; }
    }

    public class EnemyTargetedEvent : RogueEvent<EnemyTargetedEventArgs>
    {

    }
}
