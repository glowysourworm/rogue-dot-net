using Prism.Events;
using System;

namespace Rogue.NET.Scenario.Events
{
    public class GameSetupDisplayFinishedEventArgs : System.EventArgs
    {
        public Type NextDisplayType { get; set; }
    }

    public class GameSetupDisplayFinished : PubSubEvent<GameSetupDisplayFinishedEventArgs>
    {
    }
}
