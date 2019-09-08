using Rogue.NET.Core.GameRouter.GameEvent.Backend.Enum;

using System.Collections.Generic;

namespace Rogue.NET.Core.Processing.Event.Backend.EventData
{
    public class LevelEventData
    {
        public LevelEventType LevelUpdateType { get; set; }

        /// <summary>
        /// Ids of Scenario Object involved
        /// </summary>
        public IEnumerable<string> ContentIds { get; set; }
    }
}
