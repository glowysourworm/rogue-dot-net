using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Logic.Processing.Enum
{
    public enum RogueUpdatePriority
    {
        Low,
        High,

        /// <summary>
        /// PROCESSED DIRECTLY BEFORE CONTINUING BACKEND PROCESSING. EXAMPLE:  LEVEL CHANGE, PLAYER DEATH
        /// </summary>
        Critical
    }
}
