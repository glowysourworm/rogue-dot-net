﻿using Rogue.NET.Core.Model.Enums;
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
        bool IsPathToCellThroughWall(LevelGrid grid, CellPoint point1, CellPoint point2);
        bool IsPathToAdjacentCellBlocked(Level level, CellPoint point1, CellPoint point2, bool includeBlockedByEnemy);
        CellPoint GetPointInDirection(LevelGrid grid, CellPoint cellPoint, Compass direction);
        CellPoint GetRandomLocation(Level level, bool excludeOccupiedLocations);
        CellPoint GetRandomLocation(Level level, IEnumerable<CellPoint> otherExcludedLocations, bool excludeOccupiedLocations);
        CellPoint GetRandomAdjacentLocation(Level level, Player player, CellPoint location, bool excludeOccupiedCells);
        IEnumerable<CellPoint> GetFreeAdjacentLocations(Level level, Player player, CellPoint location);
        IEnumerable<CellPoint> GetFreeAdjacentLocationsForMovement(Level level, Player player, CellPoint location);
        IEnumerable<CellPoint> GetAdjacentLocations(LevelGrid grid, CellPoint location);
        double EuclideanDistance(CellPoint p1, CellPoint p2);
        double RoguianDistance(CellPoint p1, CellPoint p2);
        Compass GetDirectionBetweenAdjacentPoints(CellPoint cell1, CellPoint cell2);
    }
}
