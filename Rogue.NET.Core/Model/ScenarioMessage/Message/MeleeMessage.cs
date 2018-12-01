using Rogue.NET.Core.Model.Scenario.Alteration;
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

        public string ActorDisplayName { get; set; }
        public string ActeeDisplayName { get; set; }
        public double BaseHit { get; set; }
        public bool IsCriticalHit { get; set; }
        public bool AnyAttackAttributes { get; set; }

        public IDictionary<AttackAttribute, double> AttackAttributeHit { get; set; }
    }
}
