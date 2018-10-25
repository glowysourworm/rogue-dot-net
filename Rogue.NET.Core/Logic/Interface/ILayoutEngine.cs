using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using System.Collections.Generic;

namespace Rogue.NET.Core.Logic.Interface
{
    public interface ILayoutEngine
    {
        void Search();
        void ToggleDoor(Compass direction, CellPoint characterLocation);

        CellPoint GetPointInDirection(CellPoint cellPoint, Compass direction);

        /// <summary>
        /// Returns collection of adjacent locations that are not occupied by any scenario object
        /// </summary>
        IEnumerable<CellPoint> GetFreeAdjacentLocations(CellPoint location);

        /// <summary>
        /// Returns collection of adjacent locations that are not occupied by any enemy
        /// </summary>
        IEnumerable<CellPoint> GetFreeAdjacentLocationsForMovement(CellPoint location);

        /// <summary>
        /// Returns collection of (non-null) adjacent locations
        /// </summary>
        IEnumerable<CellPoint> GetAdjacentLocations(CellPoint location);

        /// <summary>
        /// Returns random adjacent cell - optionally excluding occupied cells
        /// </summary>
        CellPoint GetRandomAdjacentLocation(CellPoint location, bool excludeOccupiedCells);

        Compass GetDirectionBetweenAdjacentPoints(CellPoint point1, CellPoint point2);

        /// <summary>
        /// Checks to see if path to adjacent cell is blocked by { Enemy, Wall, or Door }.
        /// </summary>
        bool IsPathToAdjacentCellBlocked(CellPoint point1, CellPoint point2);

        bool IsCellThroughDoor(CellPoint point1, CellPoint point2, out CellPoint openingPosition, out Compass openingDirection);

        /// <summary>
        /// Returns cells from the level that are non-null
        /// </summary>
        /// <param name="excludeOccupiedCells">will exclude all scenario content {Enemy, Doodad, Item}</param>
        /// <returns></returns>
        CellPoint GetRandomLocation(bool excludeOccupiedCells);


        double RoguianDistance(CellPoint p1, CellPoint p2);
        double EuclideanDistance(CellPoint p1, CellPoint p2);
    }
}
