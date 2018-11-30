using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Model.ScenarioMessage.Message
{
    /// <summary>
    /// Message used for an AlterationCost or AlterationEffect - shows effected player
    /// attribute per step (or when it's calculated)
    /// </summary>
    public class AlterationMessage : ScenarioMessage
    {
        public AlterationMessage(ScenarioMessagePriority priority) : base(priority)
        {
        }

        /// <summary>
        /// The display name for the Alteration (Example: Poison)
        /// </summary>
        public string AlterationDisplayName { get; set; }

        /// <summary>
        /// The display name for the effected player attribute (Example: HP)
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
        public IDictionary<string, double> AttackAttributeEffect { get; set; }
    }
}
