﻿using Rogue.NET.Common.Collection;
using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.Core.Model.Scenario;
using System;
using System.Collections.Generic;

namespace Rogue.NET.Scenario.Processing.Event.Content
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
        public SimpleDictionary<string, bool> ScenarioObjectiveUpdates { get; set; } 
    }
    public class GameUpdateEvent : RogueEvent<GameUpdateEventArgs>
    {

    }
}
