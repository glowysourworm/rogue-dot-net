using Prism.Events;
using Rogue.NET.Model.Scenario;

namespace Rogue.NET.Model.Events
{
    public class EnemyTargetedEvent : PubSubEvent
    {
        public Enemy TargetedEnemy { get; set; }
        public bool TargetingEnded { get; set; }
    }
}
