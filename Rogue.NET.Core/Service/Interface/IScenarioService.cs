using Rogue.NET.Core.Logic.Processing.Enum;
using Rogue.NET.Core.Logic.Processing.Interface;

namespace Rogue.NET.Core.Service.Interface
{
    /// <summary>
    /// Component has the responsibility of providing methods to interact with scenario model via 
    /// the Engine components
    /// </summary>
    public interface IScenarioService
    {
        /// <summary>
        /// Tells IScenarioService to process one backed message. If it's finished - returns false.
        /// </summary>
        /// <returns>true if more to process</returns>
        bool ProcessBackend();

        /// <summary>
        /// Tells IScenarioService to clear out all queued updates and data. This should
        /// be done between loading of IModelService (between levels)
        /// </summary>
        void ClearQueues();

        /// <summary>
        /// Issues primary level command (commands involving level actions)
        /// </summary>
        void IssueCommand(ILevelCommandAction levelCommand);

        /// <summary>
        /// Issues player commands (often returns data from dialog interactions)
        /// </summary>
        /// <param name="playerCommand"></param>
        void IssuePlayerCommand(IPlayerCommandAction playerCommand);

        /// <summary>
        /// Issues player commands that have multiple item id's (originating from dialog interaction)
        /// </summary>
        /// <param name="command"></param>
        void IssuePlayerMultiItemCommand(IPlayerMultiItemCommandAction command);

        // Methods to show queue status
        bool AnyUpdates(RogueUpdatePriority priority);

        IRogueUpdate DequeueUpdate(RogueUpdatePriority priority);
    }
}
