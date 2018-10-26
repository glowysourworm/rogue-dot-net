﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Logic.Processing
{
    public enum LevelProcessingActionType
    {
        /// <summary>
        /// Process Enemy Reaction
        /// </summary>
        EnemyReaction,

        /// <summary>
        /// Primary End-Of-Turn for Scenario. { Player End-Of-Turn, Content End-Of-Turn }
        /// </summary>
        EndOfTurn,

        /// <summary>
        /// Primary End-Of-Turn for Scenario with no Player regeneration
        /// </summary>
        EndOfTurnNoRegenerate
    }
}
