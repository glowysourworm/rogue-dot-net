using Rogue.NET.Core.Logic.Event;
using Rogue.NET.Core.Model.Enums;

namespace Rogue.NET.Core.Logic.Processing.Interface
{
    public interface ILevelUpdate
    {
        ScenarioUpdateType ScenarioUpdateType { get; set; }
        LevelUpdateType LevelUpdateType { get; set; }
        SplashEventType SplashEventType { get; set; }

        int LevelNumber { get; set; }
        PlayerStartLocation StartLocation { get; set; }
    }
}