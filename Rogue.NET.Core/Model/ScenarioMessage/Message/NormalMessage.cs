using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Model.ScenarioMessage.Message
{
    public class NormalMessage : ScenarioMessage
    {
        public string Message { get; set; }

        public NormalMessage(ScenarioMessagePriority priority) : base(priority)
        {
        }
    }
}
