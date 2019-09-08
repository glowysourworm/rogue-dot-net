
using Rogue.NET.Core.Processing.Event.Backend.EventData.ScenarioMessage.Enum;

namespace Rogue.NET.Core.Processing.Event.Backend.EventData.ScenarioMessage
{
    /// <summary>
    /// Message calculated when an enemy has cast an alteration effect on the player
    /// </summary>
    public class EnemyAlterationMessageData : ScenarioMessageData
    {
        public EnemyAlterationMessageData(ScenarioMessagePriority priority) : base(priority)
        {
        }

        /// <summary>
        /// Display name of the enemy (actor)
        /// </summary>
        public string EnemyDisplayName { get; set; }

        /// <summary>
        /// Player RogueName
        /// </summary>
        public string PlayerName { get; set; }

        /// <summary>
        /// Display name of the alteration effect cast on the player
        /// </summary>
        public string AlterationDisplayName { get; set; }
    }
}
