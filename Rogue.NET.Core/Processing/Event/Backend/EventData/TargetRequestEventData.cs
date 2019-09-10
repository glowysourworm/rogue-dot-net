using Rogue.NET.Core.Processing.Event.Backend.Enum;

namespace Rogue.NET.Core.Processing.Event.Backend.EventData
{
    public class TargetRequestEventData
    {
        public string AssociatedId { get; set; }
        public TargetRequestType Type { get; set; }

        public TargetRequestEventData(TargetRequestType type, string associatedId)
        {
            this.AssociatedId = associatedId;
            this.Type = type;
        }
    }
}
