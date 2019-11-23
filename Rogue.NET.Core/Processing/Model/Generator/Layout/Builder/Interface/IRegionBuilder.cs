using Rogue.NET.Core.Math.Geometry;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.ScenarioConfiguration.Layout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Processing.Model.Generator.Layout.Builder.Interface
{
    public interface IRegionBuilder
    {
        /// <summary>
        /// Sets up regions of cells in the grid based on the template and returns the 2D cell array for the layout. Also,
        /// generates an input "feature" map for Dijkstra's algorithm.
        /// </summary>
        public GridCellInfo[,] BuildRegions(LayoutTemplate template);

        /// <summary>
        /// Sets up a default region of cells in the grid for creating a default layout
        /// </summary>
        public GridCellInfo[,] BuildDefaultRegion();
    }
}
