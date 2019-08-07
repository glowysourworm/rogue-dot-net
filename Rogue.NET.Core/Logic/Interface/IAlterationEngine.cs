using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.Core.Model.Scenario.Character;

namespace Rogue.NET.Core.Logic.Interface
{
    /// <summary>
    /// Component responsible for processing events involved with creating a character alteration. This includes
    /// animation, and post-animation processing.
    /// </summary>
    public interface IAlterationEngine : IRogueEngine
    {
        /// <summary>
        /// Validates the parameters involved with the alteration and returns true if they are met.
        /// </summary>
        bool Validate(Character actor, AlterationBase alteration);

        /// <summary>
        /// Begins process of invoking character alteration. This queues animations and post-processing
        /// actions.
        /// </summary>
        void Queue(Character actor, AlterationBase alteration);

        /// <summary>
        /// Process alteration parameters to apply to affected characters. This should happen after animations have played
        /// or if it is to be invoked without processing animations first.
        /// </summary>
        void Process(Character actor, AlterationBase alteration);
    }
}
