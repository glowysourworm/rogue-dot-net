using Rogue.NET.Core.Model.Scenario.Character;

namespace Rogue.NET.Core.Logic.Content.Interface
{
    /// <summary>
    /// Component that processes operations on Enemy character objects
    /// </summary>
    public interface IEnemyProcessor
    {
        /// <summary>
        /// Processes end-of-turn changes. This does not process death of the Enemy; but 
        /// Enemy variables could leave them dead and ready for removal from Level. This should
        /// occur in the primary IScenarioEngine.ProcessTurn method. Player is used here to 
        /// calculate aura effects on the Enemy.
        /// </summary>
        void ApplyEndOfTurn(Enemy enemy, Player player, bool actionTaken);
    }
}
