using Rogue.NET.Core.Model.Scenario.Character;

namespace Rogue.NET.Core.Logic.Content.Interface
{
    /// <summary>
    /// Component that processes operations on Enemy character objects
    /// </summary>
    public interface IEnemyProcessor
    {
        /// <summary>
        /// Processes beginning-of-turn application of malign alterations.
        /// </summary>
        void ApplyBeginningOfTurn(Enemy enemy);

        /// <summary>
        /// Processes end-of-turn changes. This does not process death of the Enemy. Player is used here to 
        /// calculate aura effects on the Enemy.
        /// </summary>
        void ApplyEndOfTurn(Enemy enemy, Player player, bool actionTaken);
    }
}
