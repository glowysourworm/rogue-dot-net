using Rogue.NET.Core.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Model.Common.Interface
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
