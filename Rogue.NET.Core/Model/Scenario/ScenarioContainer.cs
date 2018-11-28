using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.ScenarioConfiguration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rogue.NET.Core.Model.Scenario
{
    public class ScenarioContainer
    {
        public List<Level> LoadedLevels { get; set; }
        public Player Player1 { get; set; }
        public PlayerStartLocation SaveLocation { get; set; }
        public int Seed { get; set; }
        public int SaveLevel { get; set; }
        public int CurrentLevel { get; set; }
        public bool SurvivorMode { get; set; }

        // Statistics for the scenario
        public ScenarioStatistics Statistics { get; set; }

        // Store copy of the configuration for reference
        public ScenarioConfigurationContainer StoredConfig { get; set; }

        // Store Item Metadata
        public IDictionary<string, ScenarioMetaData> ScenarioEncyclopedia { get; set; }

        public ScenarioContainer()
        {
            this.Player1 = new Player();
            this.LoadedLevels = new List<Level>();
            this.CurrentLevel = 1;
            this.ScenarioEncyclopedia = new Dictionary<string, ScenarioMetaData>();
            this.SaveLocation = PlayerStartLocation.StairsUp;
            this.Statistics = new ScenarioStatistics();
        }
    }
}

