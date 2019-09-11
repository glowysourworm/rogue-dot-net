
using Rogue.NET.Core.Processing.Event.Backend.EventData.ScenarioMessage.Enum;

namespace Rogue.NET.Core.Processing.Event.Backend.EventData.ScenarioMessage
{
    /// <summary>
    /// Message calculated when an enemy has cast an alteration effect on the player
    /// </summary>
    public class CharacterAlterationMessageData : ScenarioMessageData
    {
        public CharacterAlterationMessageData(ScenarioMessagePriority priority) : base(priority)
        {
        }

        /// <summary>
        /// Name of attacker character
        /// </summary>
        public string AtttackerName { get; set; }

        /// <summary>
        /// Name of defending character
        /// </summary>
        public string DefenderName { get; set; }

        /// <summary>
        /// Display name of the alteration effect cast on the player
        /// </summary>
        public string AlterationName { get; set; }
    }
}
