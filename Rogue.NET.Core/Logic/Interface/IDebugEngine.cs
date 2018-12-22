using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Logic.Interface
{
    /// <summary>
    /// Component that calculates debug operations
    /// </summary>
    public interface IDebugEngine : IRogueEngine
    {
        /// <summary>
        /// Operation that simulates a player going through the level. This will advance the
        /// Player location to the stairs down; defeat all enemies; and give the player all
        /// items in the level.
        /// </summary>
        void SimulateAdvanceToNextLevel();

        /// <summary>
        /// Identifies all items in the player's inventory
        /// </summary>
        void IdentifyAll();

        /// <summary>
        /// Gives the Player 10,000 experience
        /// </summary>
        void GivePlayerExperience();

        /// <summary>
        /// Advances player skills 1 level
        /// </summary>
        void GivePlayerSkillExperience();
    }
}
