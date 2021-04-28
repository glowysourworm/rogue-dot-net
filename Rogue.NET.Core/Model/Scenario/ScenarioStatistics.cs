using Rogue.NET.Core.Model.Scenario.Statistic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Model.Scenario
{
    [Serializable]
    public class ScenarioStatistics
    {
        public int Ticks { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime CompletedTime { get; set; }

        /// <summary>
        /// Enemies slain
        /// </summary>
        public List<EnemyStatistic> EnemyStatistics { get; set; }

        /// <summary>
        /// Items found (but not necessarily in inventory)
        /// </summary>
        public List<ItemStatistic> ItemStatistics { get; set; }

        /// <summary>
        /// Doodads used
        /// </summary>
        public List<DoodadStatistic> DoodadStatistics { get; set; }

        public ScenarioStatistics()
        {
            this.StartTime = DateTime.Now;
            this.EnemyStatistics = new List<EnemyStatistic>();
            this.ItemStatistics = new List<ItemStatistic>();
            this.DoodadStatistics = new List<DoodadStatistic>();
        }
    }
}
