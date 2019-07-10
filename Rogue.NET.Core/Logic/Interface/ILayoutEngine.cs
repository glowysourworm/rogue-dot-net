using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using System.Collections.Generic;

namespace Rogue.NET.Core.Logic.Interface
{
    public interface ILayoutEngine : IRogueEngine
    {
        void Search(LevelGrid grid, CellPoint location);
        void ToggleDoor(LevelGrid grid, Compass direction, CellPoint characterLocation);
        bool IsPathToCellThroughDoor(LevelGrid grid, CellPoint location1, Compass openingDirection1, out CellPoint openingPosition1, out CellPoint openingPosition2, out Compass openingDirection2, out bool shouldMoveToOpeningPosition2);
        bool IsPathToCellThroughWall(Level level, CellPoint point1, CellPoint point2, bool includeBlockedByEnemy);
        bool IsPathToAdjacentCellBlocked(Level level, CellPoint point1, CellPoint point2, bool includeBlockedByEnemy);
        CellPoint GetRandomAdjacentLocation(Level level, Player player, CellPoint location, bool excludeOccupiedCells);
        IEnumerable<CellPoint> GetFreeAdjacentLocations(Level level, Player player, CellPoint location);
        IEnumerable<CellPoint> GetFreeAdjacentLocationsForMovement(Level level, Player player, CellPoint location);
    }
}
