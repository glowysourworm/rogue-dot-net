using Rogue.NET.Core.Model.Scenario.Character;

namespace Rogue.NET.Core.Processing.Model.Content.Calculator.Interface
{
    /// <summary>
    /// Component that processes operations on Enemy character objects
    /// </summary>
    public interface INonPlayerCharacterCalculator
    {
        /// <summary>
        /// Processes beginning-of-turn application of malign alterations.
        /// </summary>
        void ApplyBeginningOfTurn(NonPlayerCharacter character);

        /// <summary>
        /// Processes end-of-turn changes. This does not process death of the Enemy. Player is used here to 
        /// calculate aura effects on the Enemy.
        /// </summary>
        void ApplyEndOfTurn(NonPlayerCharacter character, Player player, bool actionTaken);
    }
}
