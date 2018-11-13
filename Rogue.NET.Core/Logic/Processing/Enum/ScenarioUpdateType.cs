namespace Rogue.NET.Core.Logic.Processing.Enum
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
        Save
    }
}
