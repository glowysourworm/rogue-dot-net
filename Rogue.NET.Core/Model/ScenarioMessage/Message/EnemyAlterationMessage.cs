using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Model.ScenarioMessage.Message
{
    /// <summary>
    /// Message calculated when an enemy has cast an alteration effect on the player
    /// </summary>
    public class EnemyAlterationMessage : ScenarioMessage
    {
        public EnemyAlterationMessage(ScenarioMessagePriority priority) : base(priority)
        {
        }

        /// <summary>
        /// Display name of the enemy (actor)
        /// </summary>
        public string EnemyDisplayName { get; set; }

        /// <summary>
        /// Display name of the alteration effect cast on the player
        /// </summary>
        public string AlterationDisplayName { get; set; }
    }
}
