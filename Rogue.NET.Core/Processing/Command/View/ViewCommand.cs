using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.Core.Processing.Command.View.CommandData;

namespace Rogue.NET.Core.Processing.Command.View
{
    public class ViewCommand : RogueAsyncEvent<ViewCommandData>
    {
    }
}
