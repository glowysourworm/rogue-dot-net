using Prism.Events;
using Rogue.NET.Core.Graveyard;

namespace Rogue.NET.Model.Events
{
    /// <summary>
    /// Occurs after player is placed
    /// </summary>
    public class LevelInitializedEvent : PubSubEvent<LevelData>
    {
    }
}
