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
        /// Editing of Scenario Assets -> These are the collections stored directly under the scenario configuration container
        /// </summary>
        Assets,

        /// <summary>
        /// Editing of levels and level branches.
        /// </summary>
        Level,

        /// <summary>
        /// Editing of the scenario objectives to select what is required
        /// </summary>
        Objective,

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
