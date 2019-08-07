using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Model.Generator.Interface;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using System.Collections.Generic;
using System.Linq;

namespace Rogue.NET.Core.Model.Scenario.Content.Extension
{
    /// <summary>
    /// Methods needed to decouple the IModelService from ILayoutEngine. The Model service should not have
    /// any references to the IRogueEngine components. ILayoutEngine should be allowed to have references to
    /// IModelService; but some of the Level / Grid methods were causing a circular reference.
    /// </summary>
    public static class LevelExtension
    {
        /// <summary>
        /// Returns random location from the level OR CellPoint.Empty (if result is empty)
        /// </summary>
        public static CellPoint GetRandomLocation(this Level level, bool excludeOccupiedLocations, IRandomSequenceGenerator randomSequenceGenerator)
        {
            // Get cell array from the grid
            var cells = level.Grid.GetCells();

            // Slower operation
            if (excludeOccupiedLocations)
            {
                var occupiedLocations = level.GetContents().Select(x => x.Location);

                var freeCells = cells.Where(x => !occupiedLocations.Contains(x.Location));

                // Return random cell
                return freeCells.PickRandom(randomSequenceGenerator.Get())?.Location ?? CellPoint.Empty;
            }
            // O(1)
            else
                return cells.PickRandom(randomSequenceGenerator.Get())?.Location ?? CellPoint.Empty;
        }

        /// <summary>
        /// Returns random location or CellPoint.Empty
        /// </summary>
        public static CellPoint GetRandomLocation(this Level level, IEnumerable<CellPoint> otherExcludedLocations, bool excludeOccupiedLocations, IRandomSequenceGenerator randomSequenceGenerator)
        {
            var locations = level.Grid.GetCells()
                                  .Select(x => x.Location)
                                  .Except(otherExcludedLocations)
                                  .ToList();

            if (locations.Count <= 0)
                return CellPoint.Empty;

            // Slower operation
            if (excludeOccupiedLocations)
            {
                var occupiedLocations = level.GetContents().Select(x => x.Location);

                var freeCells = locations.Except(occupiedLocations);

                // Return random cell
                return freeCells.PickRandom(randomSequenceGenerator.Get()) ?? CellPoint.Empty;
            }
            // O(1)
            else
                return locations.PickRandom(randomSequenceGenerator.Get()) ?? CellPoint.Empty;
        }
    }
}
