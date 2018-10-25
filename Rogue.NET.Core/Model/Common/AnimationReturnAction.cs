using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Model.Common
{
    /// <summary>
    /// Tells the Core components how to process a given Spell after a UI animation event.
    /// </summary>
    public enum AnimationReturnAction
    {
        /// <summary>
        /// Take no action
        /// </summary>
        None,

        /// <summary>
        /// Process spell as having been casted by the Player
        /// </summary>
        ProcessPlayerSpell,

        /// <summary>
        /// Process spell as having been casted by an Enemy
        /// </summary>
        ProcessEnemySpell
    }
}
