using Rogue.NET.Core.Model.Scenario;
using Rogue.NET.Core.Model.Scenario.Abstract;

namespace Rogue.NET.Scenario.Processing.Service.Interface
{
    public interface IScenarioStatisticsService
    {
        /// <summary>
        /// Does processing on ScenarioStatistics to update scenario ticks
        /// </summary>
        void ProcessScenarioTick(ScenarioStatistics statistics);

        /// <summary>
        /// Does processing on ScenarioStatistics to update scenario statistics for Doodads
        /// </summary>
        void ProcessDoodadStatistics(ScenarioStatistics statistics, ScenarioMetaData scenarioMetaData);

        /// <summary>
        /// Does processing on ScenarioStatistics to update scenario statistics for Items
        /// </summary>
        void ProcessItemStatistics(ScenarioStatistics statistics, ScenarioMetaData scenarioMetaData);

        /// <summary>
        /// Does processing on ScenarioStatistics to update scenario statistics for Enemies
        /// </summary>
        void ProcessEnemyStatistics(ScenarioStatistics statistics, ScenarioMetaData scenarioMetaData);
    }
}
