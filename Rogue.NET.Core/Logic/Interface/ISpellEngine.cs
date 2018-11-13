﻿using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.Scenario.Content.Skill;

namespace Rogue.NET.Core.Logic.Interface
{
    /// <summary>
    /// Component responsible for processing events involved with using a player or enemy spell. This includes
    /// alterations, animations, and events back to UI listeners.
    /// </summary>
    public interface ISpellEngine : IRogueEngine
    {
        /// <summary>
        /// Begins process of invoking player spell (or doodad spell). This queues animations and post-processing
        /// actions.
        /// </summary>
        LevelContinuationAction QueuePlayerMagicSpell(Spell spell);

        /// <summary>
        /// Begins process of invoking enemy spell. This queues animations and post-processing
        /// actions.
        /// </summary>
        LevelContinuationAction QueueEnemyMagicSpell(Enemy enemy, Spell spell);

        /// <summary>
        /// Process spell parameters to apply to player or targets. This should happen after animations have played
        /// or if it is to be invoked without processing animations first.
        /// </summary>
        void ProcessPlayerMagicSpell(Spell spell);

        /// <summary>
        /// Process spell parameters to apply to player or enemy. This should happen after animations have played
        /// or if it is to be invoked without processing animations first.
        /// </summary>
        void ProcessEnemyMagicSpell(Enemy enemy, Spell spell);
    }
}
