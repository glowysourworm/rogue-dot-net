using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.Core.Model.Scenario.Alteration.Effect;
using Rogue.NET.Core.Model.Scenario.Character;
using System.Collections.Generic;

namespace Rogue.NET.Core.Processing.Model.Content.Interface
{
    /// <summary>
    /// Component responsible for processing events involved with creating a character alteration. This includes
    /// animation, and post-animation processing.
    /// </summary>
    public interface IAlterationProcessor
    {
        /// <summary>
        /// Validates the parameters involved with the alteration and returns true if they are met. Also, 
        /// publishes Scenario Messages for Player about failed Validation.
        /// </summary>
        bool Validate(Character actor, AlterationContainer alteration);

        /// <summary>
        /// Begins process of invoking character alteration. This queues animations and post-processing
        /// actions. (CALCULATES AFFECTED CHARACTERS - EXCEPT for enemies hit by player attack - which 
        /// currently is Equipment Attack Alteration ONLY)
        /// </summary>
        void Queue(Character actor, AlterationContainer alteration);

        /// <summary>
        /// (PRE-SPECIFIED AFFECTED CHARACTERS) Begins process of invoking character alteration. This queues 
        /// animations and post-processing actions using affected characters that have already been calculated
        /// </summary>
        void Queue(Character actor, IEnumerable<Character> affectedCharacters, AlterationContainer alteration);

        /// <summary>
        /// Process alteration parameters to apply to affected characters. This should happen after animations have played
        /// or if it is to be invoked without processing animations first.
        /// </summary>
        void Process(Character actor, IEnumerable<Character> affectedCharacters, AlterationContainer alteration);

        /// <summary>
        /// Processes a Transmute Alteration Effect using the selected items from a player interaction with 
        /// the dialog. (This breaks the normal pattern of player -> dialog interaction because the effect
        /// itself requires a dialog)
        /// </summary>
        void ProcessTransmute(TransmuteAlterationEffect effect, IEnumerable<string> itemIds);
    }
}
