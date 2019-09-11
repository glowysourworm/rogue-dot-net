using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using System.Collections.Generic;

namespace Rogue.NET.Core.Processing.Model.Content.Interface
{
    public interface ILayoutEngine 
    {
        void Search(GridLocation location);
        void ToggleDoor(Compass direction, GridLocation characterLocation);
        bool IsPathToCellThroughDoor(GridLocation location1, Compass openingDirection1, out GridLocation openingPosition1, out GridLocation openingPosition2, out Compass openingDirection2, out bool shouldMoveToOpeningPosition2);
        bool IsPathToCellThroughWall(GridLocation point1, GridLocation point2, bool includeBlockedByEnemy);
        bool IsPathToAdjacentCellBlocked(GridLocation point1, GridLocation point2, bool includeBlockedByEnemy);
        GridLocation GetRandomAdjacentLocation(GridLocation location, bool excludeOccupiedCells);
        IEnumerable<GridLocation> GetFreeAdjacentLocations(GridLocation location);
        IEnumerable<GridLocation> GetFreeAdjacentLocationsForMovement(GridLocation location);
        IEnumerable<GridLocation> GetLocationsInRange(GridLocation location, int cellRange);
    }
}
