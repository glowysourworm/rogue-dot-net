namespace Rogue.NET.Core.Logic.Processing.Enum
{
    public enum LevelProcessingActionType
    {
        /// <summary>
        /// Process Enemy Reaction
        /// </summary>
        EnemyReaction,

        /// <summary>
        /// Process Player Spell
        /// </summary>
        PlayerSpell,

        /// <summary>
        /// Process Enemy Magic spell (happens after enemy reaction)
        /// </summary>
        EnemySpell,

        /// <summary>
        /// Primary End-Of-Turn for Scenario. { Player End-Of-Turn, Content End-Of-Turn }
        /// </summary>
        EndOfTurn,

        /// <summary>
        /// Primary End-Of-Turn for Scenario with no Player regeneration
        /// </summary>
        EndOfTurnNoRegenerate
    }
}
