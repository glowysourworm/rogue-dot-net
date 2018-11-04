using Rogue.NET.Core.Logic.Processing.Enum;
using Rogue.NET.Core.Model.Enums;

namespace Rogue.NET.Core.Logic.Processing.Interface
{
    public interface IScenarioUpdate
    {
        ScenarioUpdateType ScenarioUpdateType { get; set; }

        /// <summary>
        /// Message for the PlayerDeath ScenarioUpdateType
        /// </summary>
        string PlayerDeathMessage { get; set; }

        /// <summary>
        /// Level number for the "ChangeOfLevel" ScenarioUpdateType
        /// </summary>
        int LevelNumber { get; set; }

        /// <summary>
        /// Start location for the "ChangeOfLevel" ScenarioUpdateType
        /// </summary>
        PlayerStartLocation StartLocation { get; set; }
    }
}
