using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Model.ScenarioMessage.Message
{
    public class SkillAdvancementMessage : ScenarioMessage
    {
        public SkillAdvancementMessage(ScenarioMessagePriority priority) : base(priority)
        {
        }

        public string SkillDisplayName { get; set; }
        public int SkillLevel { get; set; }
    }
}
