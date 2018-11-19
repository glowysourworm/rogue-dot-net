using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Scenario.Events.Content
{
    public enum ShiftDisplayType
    {
        Left,
        Right,
        Up,
        Down,
        CenterOnPlayer
    }

    public class ShiftDisplayEvent : PubSubEvent<ShiftDisplayType>
    {
    }
}
