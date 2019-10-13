using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.ScenarioConfiguration;
using System;
using System.Collections.Generic;

namespace Rogue.NET.Core.Model.Scenario
{
    [Serializable]
    public class ScenarioContainer
    {
        public List<Level> Levels { get; set; }
        public Player Player { get; set; }
        public PlayerStartLocation SaveLocation { get; set; }
        public int Seed { get; set; }
        public int SaveLevel { get; set; }
        public int CurrentLevel { get; set; }
        public bool SurvivorMode { get; set; }

        // UI Parameters
        public double ZoomFactor { get; set; }

        // Statistics for the scenario
        public ScenarioStatistics Statistics { get; set; }

        // Store copy of the configuration for reference
        public ScenarioConfigurationContainer Configuration { get; set; }

        // Store Item Metadata
        public ScenarioEncyclopedia Encyclopedia { get; set; }

        public ScenarioContainer()
        {
            this.Player = new Player();
            this.Levels = new List<Level>();
            this.CurrentLevel = 1;
            this.Encyclopedia = new ScenarioEncyclopedia();
            this.SaveLocation = PlayerStartLocation.StairsUp;
            this.Statistics = new ScenarioStatistics();
            this.ZoomFactor = 1.0D;
        }
    }
}

