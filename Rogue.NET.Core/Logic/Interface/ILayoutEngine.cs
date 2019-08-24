using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using System.Collections.Generic;

namespace Rogue.NET.Core.Logic.Interface
{
    public interface ILayoutEngine : IRogueEngine
    {
        void Search(LevelGrid grid, GridLocation location);
        void ToggleDoor(LevelGrid grid, Compass direction, GridLocation characterLocation);
        bool IsPathToCellThroughDoor(LevelGrid grid, GridLocation location1, Compass openingDirection1, out GridLocation openingPosition1, out GridLocation openingPosition2, out Compass openingDirection2, out bool shouldMoveToOpeningPosition2);
        bool IsPathToCellThroughWall(Level level, GridLocation point1, GridLocation point2, bool includeBlockedByEnemy);
        bool IsPathToAdjacentCellBlocked(Level level, GridLocation point1, GridLocation point2, bool includeBlockedByEnemy);
        GridLocation GetRandomAdjacentLocation(Level level, Player player, GridLocation location, bool excludeOccupiedCells);
        IEnumerable<GridLocation> GetFreeAdjacentLocations(Level level, Player player, GridLocation location);
        IEnumerable<GridLocation> GetFreeAdjacentLocationsForMovement(Level level, Player player, GridLocation location);
        IEnumerable<GridLocation> GetLocationsInRange(Level level, GridLocation location, int cellRange);
    }
}
