using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.ScenarioEditor.Views.Constants
{
    public enum DesignMode
    {
        /// <summary>
        /// Editing of "General" (shared) asset collections and scenario parameters like name, objective description, etc...
        /// </summary>
        General,

        /// <summary>
        /// Editing of levels and level branches.
        /// </summary>
        Level,

        /// <summary>
        /// This is the scenario overview - which includes difficulty charts and must run validation to show
        /// </summary>
        Overview,

        /// <summary>
        /// Shows the validation of the scenario
        /// </summary>
        Validation
    }
}
