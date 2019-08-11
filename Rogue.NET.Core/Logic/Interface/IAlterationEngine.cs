﻿using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.Core.Model.Scenario.Alteration.Consumable;
using Rogue.NET.Core.Model.Scenario.Alteration.Doodad;
using Rogue.NET.Core.Model.Scenario.Alteration.Enemy;
using Rogue.NET.Core.Model.Scenario.Alteration.Equipment;
using Rogue.NET.Core.Model.Scenario.Alteration.Skill;
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
        /// Validates the parameters involved with the alteration and returns true if they are met. Also, 
        /// publishes Scenario Messages for Player about failed Validation.
        /// </summary>
        bool Validate(Character actor, AlterationCost cost);

        /// <summary>
        /// Begins process of invoking character alteration. This queues animations and post-processing
        /// actions.
        /// </summary>
        void Queue(Character actor, AlterationContainer alteration);

        /// <summary>
        /// Process alteration parameters to apply to affected characters. This should happen after animations have played
        /// or if it is to be invoked without processing animations first.
        /// </summary>
        void Process(Character actor, AlterationContainer alteration);
    }
}