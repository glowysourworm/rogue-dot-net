﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Logic.Processing
{
    /// <summary>
    /// Enum to specify major scenario event updates
    /// </summary>
    public enum ScenarioUpdateType
    {
        /// <summary>
        /// Nothing to process
        /// </summary>
        None,

        /// <summary>
        /// Process a change of level
        /// </summary>
        LevelChange,

        /// <summary>
        /// Player Death event
        /// </summary>
        PlayerDeath
    }
}
