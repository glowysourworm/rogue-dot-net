using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Model.ScenarioMessage.Message
{
    public class PlayerAdvancementMessage : ScenarioMessage
    {
        public PlayerAdvancementMessage(ScenarioMessagePriority priority) : base(priority)
        {
        }

        /// <summary>
        /// Set of changes to player attributes (Example: HPMax, 0.5)
        /// </summary>
        public IDictionary<string, double> AttributeChanges { get; set; }
    }
}
