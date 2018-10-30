using Prism.Events;
using Rogue.NET.Common.EventArgs;

namespace Rogue.NET.Common.Events.Scenario
{
    public class UserCommandEvent : PubSubEvent<LevelCommandEventArgs>
    {
    }
}
