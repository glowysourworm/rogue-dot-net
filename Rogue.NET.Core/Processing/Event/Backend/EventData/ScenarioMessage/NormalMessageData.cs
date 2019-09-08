
using Rogue.NET.Core.Processing.Event.Backend.EventData.ScenarioMessage.Enum;

namespace Rogue.NET.Core.Processing.Event.Backend.EventData.ScenarioMessage
{
    public class NormalMessageData : ScenarioMessageData
    {
        public string Message { get; set; }

        public NormalMessageData(ScenarioMessagePriority priority) : base(priority)
        {
        }
    }
}
