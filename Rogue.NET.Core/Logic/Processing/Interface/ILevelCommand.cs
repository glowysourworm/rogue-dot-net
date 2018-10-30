using Rogue.NET.Core.Model.Enums;

namespace Rogue.NET.Core.Logic.Processing.Interface
{
    public interface ILevelCommand
    {
        /// <summary>
        /// Intended player action
        /// </summary>
        LevelAction Action { get; set; }

        /// <summary>
        /// Intended direction
        /// </summary>
        Compass Direction { get; set; }

        /// <summary>
        /// Id for the object involved in the action
        /// </summary>
        string ScenarioObjectId { get; set; }
    }
}
