using Rogue.NET.Core.Model.Scenario.Character;

namespace Rogue.NET.Core.Processing.Event.Frontend
{
    public class EnemyTargetedEventData
    {
        public Enemy TargetedEnemy { get; set; }
        public bool TargetingEnded { get; set; }
    }
}
