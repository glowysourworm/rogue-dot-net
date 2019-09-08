using Rogue.NET.Common.Extension.Prism.EventAggregator;
using System;

namespace Rogue.NET.Scenario.Processing.Event
{
    public class ToggleLevelViewControlsEventArgs : EventArgs
    {
        public enum ToggleLevelViewControlsType
        {
            LeftHandSide,
            RightHandSide,
            All
        }

        public ToggleLevelViewControlsType Type { get; set; }
    }

    public class ToggleLevelViewControlsEvent : RogueEvent<ToggleLevelViewControlsEventArgs>
    {

    }
}
