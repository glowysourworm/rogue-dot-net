using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Scenario.Processing.Controller.Enum
{
    /// <summary>
    /// Game commands will be routed based on what's happening during play. This will be tracked
    /// by using this command mode in the Shell (or some sub-component)
    /// </summary>
    public enum GameCommandMode
    {
        /// <summary>
        /// User wishes to issue commands for UI specific things: [Targeting]
        /// </summary>
        FrontendCommand,

        /// <summary>
        /// User wishes to issue commands for the backend: [Level Commands / Player Commands]
        /// </summary>
        BackendCommand
    }
}
