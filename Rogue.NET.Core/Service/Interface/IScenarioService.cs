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
        void IssueCommand(ILevelCommandAction levelCommand);

        // Methods to show queue status
        bool AnyLevelEvents();
        bool AnyAnimationEvents();
        bool AnyScenarioEvents();
        bool AnySplashEvents();

        /// <summary>
        /// Scenario Update Events are 2nd in priority (Animation -> Scenario -> Splash -> UI)
        /// </summary>
        IScenarioUpdate DequeueScenarioUpdate();

        /// <summary>
        /// Splash Update Events are 3rd in priority (Animation -> Scenario -> Splash -> UI)
        /// </summary>
        ISplashUpdate DequeueSplashUpdate();

        /// <summary>
        /// Level Update Events are 4th in priority (Animation -> Scenario -> Splash -> UI)
        /// </summary>
        ILevelUpdate DequeueLevelUpdate();

        /// <summary>
        /// Animation Update Events are 1st in priority (Animation -> Scenario -> Splash -> UI)
        /// </summary>
        IAnimationUpdate DequeueAnimationUpdate();
    }
}
