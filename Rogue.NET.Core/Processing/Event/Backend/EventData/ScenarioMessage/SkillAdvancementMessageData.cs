using Rogue.NET.Core.Processing.Event.Backend.EventData.ScenarioMessage.Enum;

namespace Rogue.NET.Core.Processing.Event.Backend.EventData.ScenarioMessage
{
    public class SkillAdvancementMessageData : ScenarioMessageData
    {
        public SkillAdvancementMessageData(ScenarioMessagePriority priority) : base(priority)
        {
        }

        public string SkillDisplayName { get; set; }
        public int SkillLevel { get; set; }
    }
}
