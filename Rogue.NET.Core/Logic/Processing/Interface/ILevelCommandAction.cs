using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration;
using System.Collections.Generic;

namespace Rogue.NET.Core.Logic.Processing.Interface
{
    public interface ILevelCommandAction
    {
        /// <summary>
        /// Intended player action
        /// </summary>
        LevelActionType Action { get; set; }

        /// <summary>
        /// Intended direction
        /// </summary>
        Compass Direction { get; set; }

        /// <summary>
        /// Id for the object involved in the action
        /// </summary>
        string Id { get; set; }
    }
}
