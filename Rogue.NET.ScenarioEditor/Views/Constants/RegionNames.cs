using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.ScenarioEditor.Views.Constants
{
    /// <summary>
    /// Maintains Region Name handles for regions that have multiple sub-views
    /// </summary>
    public static class RegionNames
    {
        // Main (Duplicate) - this region is defined in the Shell
        public const string MainRegion = "MainRegion";

        // Editor
        public const string DesignRegion = "DesignRegion";

        // Asset (Container) Design Region
        public const string AssetContainerRegion = "AssetContainerRegion";

        // Tree View Region
        public const string BrowserRegion = "BrowserRegion";

        // Design (Container) Design Region
        public const string DesignContainerRegion = "DesignContainerRegion";
    }
}
