using Rogue.NET.Common.Extension.Event;
using Rogue.NET.Core.Processing.Action;
using Rogue.NET.Core.Processing.Event.Backend.EventData;

namespace Rogue.NET.Core.Logic.Interface
{
    /// <summary>
    /// Component specification for set of members to process the backend model. These are generic 
    /// events to raise for the primary processing queues; and methods that are specific for turn
    /// processing.
    /// </summary>
    public interface IRogueEngine
    {
        event SimpleEventHandler<AnimationEventData> AnimationEvent;
        event SimpleEventHandler<DialogEventData> DialogEvent;
        event SimpleEventHandler<LevelEventData> LevelEvent;
        event SimpleEventHandler<ScenarioEventData> ScenarioEvent;
        event SimpleEventHandler<LevelProcessingAction> LevelProcessingActionEvent;

        /// <summary>
        /// Method to run at "End of Turn": Defined as anything after enemy reactions are fully
        /// processed. Player -> Enemy Reactions -> (processing queues finished) -> End of Turn.
        /// </summary>
        void ApplyEndOfTurn(bool regenerate);
    }
}
