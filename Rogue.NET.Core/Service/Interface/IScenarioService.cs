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
        /// Tells IScenarioService to process it's data until finished (returns false).
        /// </summary>
        /// <returns>true if more to process</returns>
        bool ProcessBackend();

        /// <summary>
        /// Issues primary player command 
        /// </summary>
        /// <param name="action">Intended action</param>
        /// <param name="direction">desired direction for action</param>
        /// <param name="id">involved RogueBase.Id for action</param>
        void IssueCommand(ILevelCommand levelCommand);

        bool AnyLevelEvents();
        bool AnyAnimationEvents();

        /// <summary>
        /// Gets the next level update from the service
        /// </summary>
        ILevelUpdate DequeueLevelEvent();

        /// <summary>
        /// Gets the next animation update from the service. These are processed first
        /// </summary>
        /// <returns></returns>
        IAnimationEvent DequeueAnimation();
    }
}
