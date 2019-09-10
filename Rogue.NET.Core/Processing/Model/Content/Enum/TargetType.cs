using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Processing.Model.Content.Enum
{
    /// <summary>
    /// Type for specifying what the user targeted during a UI targeting sequence
    /// </summary>
    public enum TargetType
    {
        /// <summary>
        /// User canceled the targeting event (OR) nothing currently stored as a target
        /// </summary>
        None,

        /// <summary>
        /// User targeted a NON-EMPTY location on the level grid
        /// </summary>
        Location,

        /// <summary>
        /// User targeted a character
        /// </summary>
        Character
    }
}
