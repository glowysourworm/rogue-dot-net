using Rogue.NET.Core.Logic.Processing;
using Rogue.NET.Core.Logic.Processing.Interface;
using System;

namespace Rogue.NET.Core.Logic.Interface
{
    /// <summary>
    /// Component specification for set of members to process the backend model. These are generic 
    /// events to raise for the primary processing queues; and methods that are specific for turn
    /// processing.
    /// </summary>
    public interface IRogueEngine
    {
        event EventHandler<RogueUpdateEventArgs> RogueUpdateEvent;
        event EventHandler<ILevelProcessingAction> LevelProcessingActionEvent;

        /// <summary>
        /// Method to run at "End of Turn": Defined as anything after enemy reactions are fully
        /// processed. Player -> Enemy Reactions -> (processing queues finished) -> End of Turn.
        /// </summary>
        void ApplyEndOfTurn(bool regenerate);
    }
}
