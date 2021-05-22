using Rogue.NET.Core.Model.Scenario.Content.Layout.Interface;

using System.Collections.Generic;
using System.Linq;

namespace Rogue.NET.Core.Processing.Model.Algorithm.Component
{
    public class DijkstraPathGenerator : DijkstraMapBase
    {
        /// <summary>
        /// Callback that allows setting properties of the embedded path cells
        /// </summary>
        public delegate void DijkstraPathCallback(IGridLocator pathLocator);

        public DijkstraPathGenerator(int width, int height,
                                     IGridLocator sourceLocation,
                                     IEnumerable<IGridLocator> targetLocations,
                                     bool obeyCardinalMovement,
                                     DijkstraMapCostCallback dijkstraMapCostCallback,
                                     DijkstraMapLocatorCallback dijkstraMapCallback)
             : base(width,
                    height,
                    obeyCardinalMovement,
                    sourceLocation,
                    targetLocations,
                    dijkstraMapCostCallback,
                    dijkstraMapCallback)
        {
            // Initializes the map to run
            Initialize(sourceLocation, targetLocations);
        }

        /// <summary>
        /// Calculates path cells from source to target location - in FORWARD direction
        /// </summary>
        public IEnumerable<IGridLocator> CalculatePath(IGridLocator targetLocation)
        {
            Run();

            return GeneratePath(targetLocation).Reverse();
        }

        /// <summary>
        /// Calculates path cells from source to all target locations - in FORWARD direction
        /// </summary>
        public IDictionary<IGridLocator, IEnumerable<IGridLocator>> CalculatePaths()
        {
            Run();

            // Create paths for each target
            return this.TargetLocations.ToDictionary(location => location, location => GeneratePath(location).Reverse());
        }
    }
}
