namespace Rogue.NET.Core.GameRouter.GameEvent.Backend.Enum
{
    /// <summary>
    /// Enum to specify major scenario event updates
    /// </summary>
    public enum ScenarioUpdateType
    {
        /// <summary>
        /// Process a change of level
        /// </summary>
        LevelChange,

        /// <summary>
        /// Player Death event
        /// </summary>
        PlayerDeath,

        /// <summary>
        /// Save Scenario from a save point
        /// </summary>
        Save,

        /// <summary>
        /// Occurs after end of processing on the primary scenario engine
        /// </summary>
        StatisticsTick,

        /// <summary>
        /// Occurs when enemy has died and stats need to be updated
        /// </summary>
        StatisticsEnemyDeath,

        /// <summary>
        /// Occurs when item has been found and stats need to be updated
        /// </summary>
        StatisticsItemFound,

        /// <summary>
        /// Occurs when doodad has been used and stats need to be updated
        /// </summary>
        StatisticsDoodadUsed,

        /// <summary>
        /// Occurs when scenario is completed with objective acheived
        /// </summary>
        ScenarioCompleted
    }
}
