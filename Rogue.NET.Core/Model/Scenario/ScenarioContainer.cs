﻿using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration;
using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Model.ScenarioConfiguration;
using System.Collections.Generic;

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

        // UI Parameters
        public double ZoomFactor { get; set; }

        // Statistics for the scenario
        public ScenarioStatistics Statistics { get; set; }

        // Store copy of the configuration for reference
        public ScenarioConfigurationContainer Configuration { get; set; }

        // Store Item Metadata
        public IDictionary<string, ScenarioMetaData> ScenarioEncyclopedia { get; set; }

        // Attack Attributes
        public IDictionary<string, AttackAttribute> AttackAttributes { get; set; }

        public ScenarioContainer()
        {
            this.Player = new Player();
            this.LoadedLevels = new List<Level>();
            this.CurrentLevel = 1;
            this.ScenarioEncyclopedia = new Dictionary<string, ScenarioMetaData>();
            this.AttackAttributes = new Dictionary<string, AttackAttribute>();
            this.SaveLocation = PlayerStartLocation.StairsUp;
            this.Statistics = new ScenarioStatistics();
            this.ZoomFactor = 1.0D;
        }
    }
}

