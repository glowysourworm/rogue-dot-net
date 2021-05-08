using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Processing.Model.Generator.Layout.Construction;

using System.Collections.Generic;
using System.Linq;

namespace Rogue.NET.Core.Processing.Model.Algorithm.Component
{
    public class DijkstraPathGenerator : DijkstraMapBase
    {
        /// <summary>
        /// Callback that allows setting properties of the embedded path cells
        /// </summary>
        public delegate void DijkstraPathCallback(GridCellInfo pathCell);

        public DijkstraPathGenerator(int width, int height,
                                     IEnumerable<Region<GridCellInfo>> avoidRegions,
                                     GridCellInfo sourceLocation,
                                     IEnumerable<GridCellInfo> targetLocations,
                                     bool obeyCardinalMovement,
                                     DijkstraMapLocatorCallback dijkstraMapCallback)
             : base(width,
                    height,
                    obeyCardinalMovement,
                    sourceLocation,
                    targetLocations,
                    new DijkstraMapCostCallback((column1, row1, column2, row2) =>
                    {
                        // MOVEMENT COST BASED ON THE DESTINATION CELL
                        //
                        if (avoidRegions.Any(region => region[column2, row2] != null))
                            return DijkstraMapBase.MapCostAvoid;

                        else
                            return 0;

                    }), dijkstraMapCallback)
        {
            // Initializes the map to run
            Initialize(sourceLocation, targetLocations);
        }

        /// <summary>
        /// Calculates path cells from source to all target locations
        /// </summary>
        public void CalculatePaths(DijkstraPathCallback callback)
        {
            Run();

            // Create paths for each target
            foreach (var targetLocation in this.TargetLocations)
            {
                foreach (var cell in GeneratePath(targetLocation))
                {
                    // Allow setting properties on cells from the new path
                    callback(cell as GridCellInfo);
                }
            }
        }
    }
}
