using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.Core.Processing.Command.Backend.CommandData;

namespace Rogue.NET.Core.Processing.Command.Backend
{
    public class LevelCommand : RogueAsyncEvent<LevelCommandData>
    {
    }
}
