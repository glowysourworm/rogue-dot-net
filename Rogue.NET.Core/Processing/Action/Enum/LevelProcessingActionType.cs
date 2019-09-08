namespace Rogue.NET.Core.Processing.Action.Enum
{
    public enum LevelProcessingActionType
    {
        /// <summary>
        /// Process Character Reaction
        /// </summary>
        Reaction,

        /// <summary>
        /// Process Character Alteration
        /// </summary>
        CharacterAlteration,

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
