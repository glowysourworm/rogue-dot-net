using Microsoft.Practices.Prism.Events;
using Rogue.NET.Common.EventArgs;

namespace Rogue.NET.Common.Events.Scenario
{
    public class UserCommandEvent : CompositePresentationEvent<UserCommandEvent>
    {
        public LevelCommandEventArgs LevelCommand { get; set; }
    }
}
