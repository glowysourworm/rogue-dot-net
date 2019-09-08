using Rogue.NET.Common.Extension.Prism.EventAggregator;

namespace Rogue.NET.Scenario.Processing.Event.Content
{
    public enum ShiftDisplayType
    {
        Left,
        Right,
        Up,
        Down,
        CenterOnPlayer
    }

    public class ShiftDisplayEvent : RogueEvent<ShiftDisplayType>
    {
    }
}
