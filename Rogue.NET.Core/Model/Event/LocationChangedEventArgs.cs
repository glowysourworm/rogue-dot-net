using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using System;

namespace Rogue.NET.Core.Model.Event
{
    public class LocationChangedEventArgs : EventArgs
    {
        public ScenarioObject ScenarioObject { get; set; }
        public GridLocation OldLocation { get; set; }
        public GridLocation NewLocation { get; set; }
    }
}