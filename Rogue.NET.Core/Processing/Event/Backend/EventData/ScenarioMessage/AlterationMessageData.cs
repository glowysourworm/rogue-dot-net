using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Processing.Event.Backend.EventData.ScenarioMessage.Enum;
using System.Collections.Generic;

namespace Rogue.NET.Core.Processing.Event.Backend.EventData.ScenarioMessage
{
    /// <summary>
    /// Message used for an AlterationCost or AlterationEffect - shows effected player
    /// attribute per step (or when it's calculated)
    /// </summary>
    public class AlterationMessageData : ScenarioMessageData
    {
        public AlterationMessageData(ScenarioMessagePriority priority) : base(priority)
        {
        }

        /// <summary>
        /// The display name for the Alteration (Example: Poison)
        /// </summary>
        public string AlterationDisplayName { get; set; }

        /// <summary>
        /// The display name for the effected player attribute (Example: Health)
        /// </summary>
        public string EffectedAttributeName { get; set; }

        /// <summary>
        /// Is the effect caused by attack attribute calculation
        /// </summary>
        public bool IsCausedByAttackAttributes { get; set; }

        /// <summary>
        /// Calculated non-attack-attribute effect
        /// </summary>
        public double Effect { get; set; }

        /// <summary>
        /// Calculated attack-attribute effect
        /// </summary>
        public IDictionary<ScenarioImage, double> AttackAttributeEffect { get; set; }
    }
}
