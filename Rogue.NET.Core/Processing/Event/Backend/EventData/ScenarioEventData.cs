using Rogue.NET.Core.GameRouter.GameEvent.Backend.Enum;
using Rogue.NET.Core.Model.Enums;

namespace Rogue.NET.Core.Processing.Event.Backend.EventData
{
    public class ScenarioEventData
    {
        public ScenarioUpdateType ScenarioUpdateType { get; set; }

        // Player Death Parameters
        public string PlayerDeathMessage { get; set; }

        // Level Load Parameters
        public int LevelNumber { get; set; }
        public PlayerStartLocation StartLocation { get; set; }

        // Statistics Update Parameters
        public string ContentRogueName { get; set; }
    }
}
