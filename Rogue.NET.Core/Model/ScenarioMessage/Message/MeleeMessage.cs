using Rogue.NET.Core.Model.Scenario.Alteration;
using Rogue.NET.Core.Model.Scenario.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Model.ScenarioMessage.Message
{
    public class MeleeMessage : ScenarioMessage
    {
        public MeleeMessage(ScenarioMessagePriority priority) : base(priority)
        {
        }

        public string AttackerDisplayName { get; set; }
        public string DefenderDisplayName { get; set; }
        public double BaseHit { get; set; }
        public bool IsCriticalHit { get; set; }
        public bool AnySpecializedHits { get; set; }

        public IDictionary<ScenarioImage, double> SpecializedHits { get; set; }
    }
}
