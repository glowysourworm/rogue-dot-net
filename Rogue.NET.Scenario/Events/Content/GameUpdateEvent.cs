using Prism.Events;
using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.Core.Model.Scenario;
using Rogue.NET.Scenario.Content.ViewModel.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Scenario.Events.Content
{
    public class GameUpdateEventArgs : EventArgs
    {
        public ScenarioStatistics Statistics { get; set; }
        public bool IsObjectiveAcheived { get; set; }
        public bool IsSurvivorMode { get; set; }
        public int Seed { get; set; }
        public int LevelNumber { get; set; }

        /// <summary>
        /// Dictionary of flags to signal that an objective has been completed (keyed by RogueName)
        /// </summary>
        public IDictionary<string, bool> ScenarioObjectiveUpdates { get; set; } 
    }
    public class GameUpdateEvent : RogueEvent<GameUpdateEventArgs>
    {

    }
}
