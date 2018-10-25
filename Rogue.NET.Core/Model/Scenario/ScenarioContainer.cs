using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.Scenario.Dynamic;
using Rogue.NET.Core.Model.ScenarioConfiguration;
using System;
using System.Collections.Generic;

namespace Rogue.NET.Core.Model.Scenario
{
    public class ScenarioContainer
    {
        public List<Level> LoadedLevels { get; set; }
        public Player Player1 { get; set; }
        public int Seed { get; set; }
        public int CurrentLevel { get; set; }
        public int TotalTicks { get; set; }
        public bool SurvivorMode { get; set; }
        public bool ObjectiveAcheived { get; set; }       
        public DateTime StartTime { get; set; }
        public DateTime CompletedTime { get; set; }

        public ScenarioConfigurationContainer StoredConfig { get; set; }

        //Store Item Metadata
        public IDictionary<string, ScenarioMetaData> ScenarioEncyclopedia { get; set; }

        public ScenarioContainer()
        {
            this.Player1 = new Player();
            this.LoadedLevels = new List<Level>();
            this.CurrentLevel = 1;
            this.ScenarioEncyclopedia = new Dictionary<string, ScenarioMetaData>();
            this.ObjectiveAcheived = false;
        }
    }
}

