using Rogue.NET.Core.Logic.Event;
using Rogue.NET.Core.Logic.Processing.Interface;
using Rogue.NET.Core.Model.Enums;

namespace Rogue.NET.Core.Logic.Processing
{
    public class LevelUpdate : ILevelUpdate
    {
        public ScenarioUpdateType ScenarioUpdateType { get; set; }
        public LevelUpdateType LevelUpdateType { get; set; }
        public SplashEventType SplashEventType { get; set; }

        public int LevelNumber { get; set; }
        public PlayerStartLocation StartLocation { get; set; }
    }
}
