using Prism.Events;
using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.Core.Model.Scenario;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Scenario.Events.Content
{
    public class GameUpdateEventArgs : EventArgs
    {
        public string ScenarioName { get; set; }
        public ScenarioStatistics Statistics { get; set; }
        public bool IsObjectiveAcheived { get; set; }
        public bool IsSurvivorMode { get; set; }
        public int Seed { get; set; }
        public int LevelNumber { get; set; }
    }
    public class GameUpdateEvent : RogueEvent<GameUpdateEventArgs>
    {

    }
}
