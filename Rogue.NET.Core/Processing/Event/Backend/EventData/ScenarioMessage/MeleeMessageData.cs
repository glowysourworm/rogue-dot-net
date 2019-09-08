using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Processing.Event.Backend.EventData.ScenarioMessage.Enum;
using System.Collections.Generic;

namespace Rogue.NET.Core.Processing.Event.Backend.EventData.ScenarioMessage
{
    public class MeleeMessageData : ScenarioMessageData
    {
        public MeleeMessageData(ScenarioMessagePriority priority) : base(priority)
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
