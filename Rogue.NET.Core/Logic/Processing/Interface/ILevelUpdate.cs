using Rogue.NET.Core.Logic.Processing.Enum;
using System.Collections.Generic;

namespace Rogue.NET.Core.Logic.Processing.Interface
{
    public interface ILevelUpdate
    {
        LevelUpdateType LevelUpdateType { get; set; }

        IEnumerable<string> ContentIds { get; set; }
    }
}