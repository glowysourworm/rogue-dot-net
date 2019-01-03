using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Logic.Content.Enum
{
    /// <summary>
    /// Defines interaction types for physical character combat
    /// </summary>
    public enum PhysicalAttackType
    {
        /// <summary>
        /// Hand-to-hand combat
        /// </summary>
        Melee,

        /// <summary>
        /// Range combat
        /// </summary>
        Range
    }

    /// <summary>
    /// Defines an interaction type between two characters
    /// </summary>
    public enum InteractionType
    {
        /// <summary>
        /// This would define any strength, agility, or speed based physical interaction
        /// </summary>
        Physical,

        /// <summary>
        /// This defines any intelligence based interaction
        /// </summary>
        Mental
    }
}
