using Rogue.NET.Core.Processing.Event.Backend.EventData.ScenarioMessage.Enum;

namespace Rogue.NET.Core.Processing.Event.Backend.EventData.ScenarioMessage
{
    public abstract class ScenarioMessageData
    {
        public ScenarioMessagePriority Priority { get; set; }

        public ScenarioMessageData(ScenarioMessagePriority priority)
        {
            this.Priority = priority;
        }
    }
}
