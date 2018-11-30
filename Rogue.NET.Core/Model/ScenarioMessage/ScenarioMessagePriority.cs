using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Model.ScenarioMessage
{
    public enum ScenarioMessagePriority
    {
        /// <summary>
        /// Normal message priortiy
        /// </summary>
        Normal,

        /// <summary>
        /// Message that is good for the Player
        /// </summary>
        Good,

        /// <summary>
        /// Message that is bad for the player
        /// </summary>
        Bad,

        /// <summary>
        /// Message that the player has acheived an objective (VERY GOOD)
        /// </summary>
        Objective
    }
}
