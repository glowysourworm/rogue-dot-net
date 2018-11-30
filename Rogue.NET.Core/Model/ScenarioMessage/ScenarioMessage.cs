using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Model.ScenarioMessage
{
    /// <summary>
    /// Base marker class for scenario messages
    /// </summary>
    public abstract class ScenarioMessage
    {
        public ScenarioMessagePriority Priority { get; set; }

        public ScenarioMessage(ScenarioMessagePriority priority)
        {
            this.Priority = priority;
        }
    }
}
