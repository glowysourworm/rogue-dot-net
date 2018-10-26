using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Logic.Processing
{
    public enum LevelUpdateType
    {
        /// <summary>
        /// Nothing to update
        /// </summary>
        None,

        /// <summary>
        /// Occurs when the layout has changed (LevelGrid)
        /// </summary>
        Layout,

        /// <summary>
        /// Occurs when the region surrounding the player changes (visible locations)
        /// </summary>
        LayoutVisible,

        /// <summary>
        /// Occurs when all content in the level needs to be updated
        /// </summary>
        AllContent,

        /// <summary>
        /// Occurs when content surrounding the player needs to be updated
        /// </summary>
        VisibleContent,

        /// <summary>
        /// Occurs when the player needs to be updated
        /// </summary>
        Player
    }
}
