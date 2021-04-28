using Rogue.NET.Core.Media.SymbolEffect.Utility;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Model.ScenarioConfiguration;
using Rogue.NET.Core.Processing.Service.Cache;
using Rogue.NET.Core.Processing.Service.FileDatabase.Interface;
using Rogue.NET.Core.Processing.Service.Interface;

using System;

namespace Rogue.NET.Core.Model.Scenario
{
    public class ScenarioContainer : IRogueFileDatabaseSerializable
    {
        // Primary reference for the level in play
        public Level CurrentLevel { get; private set; }

        // Save player referenc and data separately from Level
        public Player Player { get; private set; }

        // Contains several details to be stored in a single file record
        public ScenarioContainerDetail Detail { get; private set; }

        // Statistics for the scenario
        public ScenarioStatistics Statistics { get; private set; }

        // Store copy of the configuration for reference
        public ScenarioConfigurationContainer Configuration { get; private set; }

        // Store Item Metadata
        public ScenarioEncyclopedia Encyclopedia { get; private set; }

        [Serializable]
        public class ScenarioContainerDetail
        {
            public PlayerStartLocation SaveLocation { get; set; }
            public int Seed { get; set; }
            public int SaveLevelNumber { get; set; }
            public int CurrentLevelNumber { get; set; }
            public bool SurvivorMode { get; set; }

            // UI Parameters
            public double ZoomFactor { get; set; }

            public ScenarioContainerDetail() { }
        }

        public ScenarioContainer(ScenarioConfigurationContainer scenarioConfiguration, 
                                 ScenarioEncyclopedia scenarioEncyclopedia,
                                 Player player,
                                 Level firstLevel,
                                 int seed,
                                 bool survivorMode)
        {
            this.Player = player;
            this.CurrentLevel = firstLevel;
            this.Configuration = scenarioConfiguration;
            this.Detail = new ScenarioContainerDetail()
            {
                SaveLocation = PlayerStartLocation.StairsUp,
                Seed = seed,
                SaveLevelNumber = 0,
                CurrentLevelNumber = 1,
                SurvivorMode = survivorMode,
                ZoomFactor = 2.0D
            };
            this.Encyclopedia = scenarioEncyclopedia;
            this.Statistics = new ScenarioStatistics();
        }

        public ScenarioContainer(IRogueFileDatabaseSerializer serializer)
        {
            this.Detail = serializer.Fetch<ScenarioContainerDetail>("Detail");
            this.CurrentLevel = serializer.Fetch<Level>("Level " + this.Detail.CurrentLevelNumber.ToString());
            this.Player = serializer.Fetch<Player>("Player");
            this.Configuration = serializer.Fetch<ScenarioConfigurationContainer>("Configuration");
            this.Encyclopedia = serializer.Fetch<ScenarioEncyclopedia>("Encyclopedia");
            this.Statistics = serializer.Fetch<ScenarioStatistics>("Statistics");
        }

        public void SaveRecords(IRogueFileDatabaseSerializer serializer)
        {
            serializer.AddOrUpdate("Detail", this.Detail);
            serializer.AddOrUpdate("Level " + this.Detail.CurrentLevelNumber.ToString(), this.CurrentLevel);
            serializer.AddOrUpdate("Player", this.Player);
            serializer.AddOrUpdate("Configuration", this.Configuration);
            serializer.AddOrUpdate("Encyclopedia", this.Encyclopedia);
            serializer.AddOrUpdate("Statistics", this.Statistics);
        }

        public void LoadLevel(IRogueFileDatabaseSerializer serializer, int levelNumber)
        {
            this.CurrentLevel = serializer.Fetch<Level>("Level " + levelNumber.ToString());
            this.Detail.CurrentLevelNumber = levelNumber;
        }

        public void SaveLevel(IRogueFileDatabaseSerializer serializer, Level level)
        {
            serializer.AddOrUpdate<Level>("Level " + level.Parameters.Number.ToString(), level);
        }

        /// <summary>
        /// Creates ScenarioInfo from the serializer for a known ScenarioContianer entry in the IRogueFileDatabase
        /// </summary>
        public static ScenarioInfo CreateInfo(IRogueFileDatabaseSerializer serializer)
        {
            var detail = serializer.Fetch<ScenarioContainerDetail>("Detail");
            var player = serializer.Fetch<Player>("Player");
            var scenarioConfiguration = serializer.Fetch<ScenarioConfigurationContainer>("Configuration");

            return new ScenarioInfo()
            {
                RogueName = player.RogueName,
                ScenarioName = scenarioConfiguration.ScenarioDesign.Name,
                Seed = detail.Seed,
                SmileyExpression = player.SmileyExpression,
                SmileyBodyColor = ColorOperations.Convert(player.SmileyBodyColor),
                SmileyLineColor = ColorOperations.Convert(player.SmileyLineColor)
            };
        }

        /// <summary>
        /// Creates ScenarioInfo from this instance of the scenario container
        /// </summary>
        public ScenarioInfo CreateInfo()
        {
            return new ScenarioInfo()
            {
                RogueName = this.Player.RogueName,
                ScenarioName = this.Configuration.ScenarioDesign.Name,
                Seed = this.Detail.Seed,
                SmileyExpression = this.Player.SmileyExpression,
                SmileyBodyColor = ColorOperations.Convert(this.Player.SmileyBodyColor),
                SmileyLineColor = ColorOperations.Convert(this.Player.SmileyLineColor)
            };
        }
    }
}

