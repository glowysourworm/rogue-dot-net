using Prism.Events;
using Rogue.NET.Common.Extension.Prism.EventAggregator;
using System;

namespace Rogue.NET.Scenario.Events
{
    public class GameSetupDisplayFinishedEventArgs : System.EventArgs
    {
        public Type NextDisplayType { get; set; }
    }

    public class GameSetupDisplayFinished : RogueEvent<GameSetupDisplayFinishedEventArgs>
    {
    }
}
