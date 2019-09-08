using Rogue.NET.Core.Model.Scenario;
using Rogue.NET.Core.Model.Scenario.Statistic;
using Rogue.NET.Scenario.Processing.Service.Interface;
using System.ComponentModel.Composition;

namespace Rogue.NET.Scenario.Processing.Service
{
    [Export(typeof(IScenarioStatisticsService))]
    public class ScenarioStatisticsService : IScenarioStatisticsService
    {
        public void ProcessDoodadStatistics(ScenarioStatistics statistics, ScenarioMetaData scenarioMetaData)
        {
            statistics.DoodadStatistics.Add(new DoodadStatistic()
            {
                RogueName = scenarioMetaData.RogueName,
                Score = (int)(3 *
                        (scenarioMetaData.IsUnique ? 2 : 1) *
                        (scenarioMetaData.IsObjective ? 10 : 1)),
                IsObjective = scenarioMetaData.IsObjective,
                IsUnique = scenarioMetaData.IsUnique
            });
        }

        public void ProcessEnemyStatistics(ScenarioStatistics statistics, ScenarioMetaData scenarioMetaData)
        {
            statistics.EnemyStatistics.Add(new EnemyStatistic()
            {
                RogueName = scenarioMetaData.RogueName,
                Score = (int)(10 *
                        (scenarioMetaData.IsUnique ? 2 : 1) *
                        (scenarioMetaData.IsObjective ? 10 : 1)),
                IsObjective = scenarioMetaData.IsObjective,
                IsUnique = scenarioMetaData.IsUnique                
            });
        }

        public void ProcessItemStatistics(ScenarioStatistics statistics, ScenarioMetaData scenarioMetaData)
        {
            statistics.ItemStatistics.Add(new ItemStatistic()
            {
                RogueName = scenarioMetaData.RogueName,
                Score = (1) *
                        (scenarioMetaData.IsUnique ? 2 : 1) *
                        (scenarioMetaData.IsObjective ? 10 : 1),
                IsCursed = scenarioMetaData.IsCursed,
                IsObjective = scenarioMetaData.IsObjective,
                IsUnique = scenarioMetaData.IsUnique
            });
        }

        public void ProcessScenarioTick(ScenarioStatistics statistics)
        {
            statistics.Ticks++;
        }
    }
}
