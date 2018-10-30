using Prism.Events;
using Rogue.NET.Core.Model.Enums;

namespace Rogue.NET.Common.Events.Scenario
{
    public class LoadLevelEventArgs : System.EventArgs
    {
        public int LevelNumber { get; set; }
        public PlayerStartLocation StartLocation { get; set; }
    }
    public class LoadLevelEvent : PubSubEvent<LoadLevelEventArgs>
    {

    }
}
