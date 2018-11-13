using Rogue.NET.Core.Logic.Processing.Enum;
using Rogue.NET.Core.Logic.Processing.Interface;
using System.Collections.Generic;

namespace Rogue.NET.Core.Logic.Processing
{
    public class LevelUpdate : ILevelUpdate
    {
        public LevelUpdateType LevelUpdateType { get; set; }

        /// <summary>
        /// Ids of Scenario Object involved
        /// </summary>
        public IEnumerable<string> ContentIds { get; set; }
    }
}
