﻿using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.Scenario.Content.Religion;
using Rogue.NET.Core.Model.ScenarioConfiguration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rogue.NET.Core.Model.Scenario
{
    public class ScenarioContainer
    {
        public List<Level> LoadedLevels { get; set; }
        public Player Player { get; set; }
        public PlayerStartLocation SaveLocation { get; set; }
        public int Seed { get; set; }
        public int SaveLevel { get; set; }
        public int CurrentLevel { get; set; }
        public bool SurvivorMode { get; set; }

        // Statistics for the scenario
        public ScenarioStatistics Statistics { get; set; }

        // Store copy of the configuration for reference
        public ScenarioConfigurationContainer Configuration { get; set; }

        // Store Item Metadata
        public IDictionary<string, ScenarioMetaData> ScenarioEncyclopedia { get; set; }

        // Religions
        public IDictionary<string, Religion> Religions { get; set; }

        public ScenarioContainer()
        {
            this.Player = new Player();
            this.LoadedLevels = new List<Level>();
            this.CurrentLevel = 1;
            this.ScenarioEncyclopedia = new Dictionary<string, ScenarioMetaData>();
            this.Religions = new Dictionary<string, Religion>();
            this.SaveLocation = PlayerStartLocation.StairsUp;
            this.Statistics = new ScenarioStatistics();
        }
    }
}

