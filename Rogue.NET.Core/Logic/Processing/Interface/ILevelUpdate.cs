using Rogue.NET.Core.Logic.Processing.Enum;

namespace Rogue.NET.Core.Logic.Processing.Interface
{
    public interface ILevelUpdate
    {
        LevelUpdateType LevelUpdateType { get; set; }

        string Id { get; set; }
    }
}