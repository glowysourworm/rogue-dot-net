using Rogue.NET.Core.Logic.Processing.Enum;
using Rogue.NET.Core.Logic.Processing.Interface;

namespace Rogue.NET.Core.Logic.Processing
{
    public class LevelUpdate : ILevelUpdate
    {
        public LevelUpdateType LevelUpdateType { get; set; }

        /// <summary>
        /// Id of Scenario Object involved
        /// </summary>
        public string Id { get; set; }
    }
}
