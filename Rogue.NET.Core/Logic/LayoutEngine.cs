using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Generator.Interface;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Rogue.NET.Core.Logic.Interface;
using Rogue.NET.Core.Model.Scenario;
using Rogue.NET.Core.Logic.Processing.Interface;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Logic.Processing;
using Rogue.NET.Core.Logic.Processing.Enum;
using Rogue.NET.Core.Service.Interface;

namespace Rogue.NET.Core.Logic
{
    [Export(typeof(ILayoutEngine))]
    public class LayoutEngine : ILayoutEngine
    {
        readonly IRandomSequenceGenerator _randomSequenceGenerator;
        readonly IScenarioMessageService _scenarioMessageService;

        public event EventHandler<IScenarioUpdate> ScenarioUpdateEvent;
        public event EventHandler<ISplashUpdate> SplashUpdateEvent;
        public event EventHandler<ILevelUpdate> LevelUpdateEvent;
        public event EventHandler<IAnimationUpdate> AnimationUpdateEvent;
        public event EventHandler<ILevelProcessingAction> LevelProcessingActionEvent;

        [ImportingConstructor]
        public LayoutEngine(IRandomSequenceGenerator randomSequenceGenerator, IScenarioMessageService scenarioMessageService)
        {
            _randomSequenceGenerator = randomSequenceGenerator;
            _scenarioMessageService = scenarioMessageService;
        }

        #region (public) Player Action Methods
        public void Search(LevelGrid grid, CellPoint location)
        {
            Cell c = grid.GetCell(location);
            Cell n = grid[location.Column, location.Row - 1];
            Cell s = grid[location.Column, location.Row + 1];
            Cell e = grid[location.Column + 1, location.Row];
            Cell w = grid[location.Column - 1, location.Row];
            Cell ne = grid[location.Column + 1, location.Row - 1];
            Cell nw = grid[location.Column - 1, location.Row - 1];
            Cell se = grid[location.Column + 1, location.Row + 1];
            Cell sw = grid[location.Column - 1, location.Row + 1];

            var cells = new Cell[] { c, n, s, e, w, ne, sw, se, sw };

            var visibleDoors = new Compass[] {
                c?.VisibleDoors ?? Compass.Null,
                n?.VisibleDoors ?? Compass.Null,
                s?.VisibleDoors ?? Compass.Null,
                e?.VisibleDoors ?? Compass.Null,
                w?.VisibleDoors ?? Compass.Null,
                ne?.VisibleDoors ?? Compass.Null,
                nw?.VisibleDoors ?? Compass.Null,
                se?.VisibleDoors ?? Compass.Null,
                sw?.VisibleDoors  ?? Compass.Null};

            if (c != null)
            {
                c.NorthDoorSearchCounter--;
                c.SouthDoorSearchCounter--;
                c.EastDoorSearchCounter--;
                c.WestDoorSearchCounter--;
            }
            if (n != null)
            {
                n.SouthDoorSearchCounter--;
                n.EastDoorSearchCounter--;
                n.WestDoorSearchCounter--;
            }
            if (s != null)
            {
                s.NorthDoorSearchCounter--;
                s.EastDoorSearchCounter--;
                s.WestDoorSearchCounter--;
            }
            if (e != null)
            {
                e.WestDoorSearchCounter--;
                e.NorthDoorSearchCounter--;
                e.SouthDoorSearchCounter--;
            }
            if (w != null)
            {
                w.EastDoorSearchCounter--;
                w.NorthDoorSearchCounter--;
                w.SouthDoorSearchCounter--;
            }

            if (ne != null)
            {
                ne.SouthDoorSearchCounter--;
                ne.WestDoorSearchCounter--;
            }
            if (nw != null)
            {
                nw.SouthDoorSearchCounter--;
                nw.EastDoorSearchCounter--;
            }
            if (se != null)
            {
                se.NorthDoorSearchCounter--;
                se.WestDoorSearchCounter--;
            }
            if (sw != null)
            {
                sw.NorthDoorSearchCounter--;
                sw.EastDoorSearchCounter--;
            }

            var topologyChange = false;
            for (int i=0;i<cells.Length && !topologyChange;i++)
            {
                topologyChange = topologyChange || (cells[i]?.VisibleDoors ?? Compass.Null) != visibleDoors[i];
            }

            if (topologyChange)
            {
                _scenarioMessageService.Publish("Door found!");

                LevelUpdateEvent(this, new LevelUpdate() { LevelUpdateType = LevelUpdateType.LayoutTopology });
            }
            else
                _scenarioMessageService.Publish("Search " + Enumerable.Range(1, _randomSequenceGenerator.Get(2, 5)).Aggregate<int,string>("", (accum, x) => accum + "."));
        }
        public void ToggleDoor(LevelGrid grid, Compass direction, CellPoint characterLocation)
        {
            var openingPosition1 = CellPoint.Empty;
            var openingPosition2 = CellPoint.Empty;
            var openingDirection2 = Compass.Null;
            var shouldMoveToOpeningPosition1 = false;

            if (IsPathToCellThroughDoor(grid, characterLocation, direction, out openingPosition1, out openingPosition2, out openingDirection2, out shouldMoveToOpeningPosition1))
            {
                // Have to move into position first
                if (shouldMoveToOpeningPosition1)
                    return;

                var characterCell = grid.GetCell(characterLocation);
                var openingPositionCell = grid.GetCell(openingPosition2);

                characterCell.ToggleDoor(direction);
                openingPositionCell.ToggleDoor(openingDirection2);

                LevelUpdateEvent(this, new LevelUpdate()
                {
                    LevelUpdateType = LevelUpdateType.LayoutTopology
                });
            }
        }
        #endregion

        #region (public) Query Methods
        public bool IsPathToCellThroughDoor(
            LevelGrid grid, 
            CellPoint location1, 
            Compass openingDirection1,              // Represents the Door for location1
            out CellPoint openingPosition1,         // Represents the opening position for the door
            out CellPoint openingPosition2,         // Represents the same door opposite cell
            out Compass openingDirection2,          // Represents the Door for location2
            out bool shouldMoveToOpeningPosition1)  // Should move into position for opening the door before opening
        {
            openingPosition1 = CellPoint.Empty;
            openingPosition2 = CellPoint.Empty;
            openingDirection2 = Compass.Null;
            shouldMoveToOpeningPosition1 = false;

            var location2 = GetPointInDirection(grid, location1, openingDirection1);

            Cell cell1 = grid.GetCell(location1);
            Cell cell2 = grid.GetCell(location2);

            if (cell1 == null || cell2 == null)
                return false;

            var direction = GetDirectionBetweenAdjacentPoints(location1, location2);
            var oppositeDirection = GetOppositeDirection(direction);

            switch (direction)
            {
                case Compass.N:
                case Compass.S:
                case Compass.E:
                case Compass.W:
                    {
                        openingDirection2 = oppositeDirection;
                        openingPosition1 = location1;
                        openingPosition2 = location2;
                        return ((cell1.Doors & direction) != 0) &&          // Position 1 -> Door in this direction
                               ((cell2.Doors & oppositeDirection) != 0);    // Position 2 -> Door in opposite direction
                    }
                case Compass.NE:
                case Compass.NW:
                case Compass.SE:
                case Compass.SW:
                    {
                        Compass cardinal1 = Compass.Null;
                        Compass cardinal2 = Compass.Null;

                        var diag1 = grid.GetOffDiagonalCell1(location1, direction, out cardinal1);
                        var diag2 = grid.GetOffDiagonalCell2(location1, direction, out cardinal2);

                        if (diag1 == null && diag2 == null)
                            return false;

                        var cardinal1Opposite = GetOppositeDirection(cardinal1);
                        var cardinal2Opposite = GetOppositeDirection(cardinal2);

                        if (diag1 != null)
                        {
                            // Current cell -> 1st off-diagonal cell
                            if ((diag1.Doors & cardinal1Opposite) != 0 &&
                                (cell1.Doors & cardinal1) != 0)
                            {
                                openingPosition1 = location1;
                                openingPosition2 = diag1.Location;
                                openingDirection2 = cardinal1Opposite;

                                return true;
                            }
                            // 1st off-diagonal cell -> Desired cell
                            else if ((diag1.Doors & cardinal2) != 0 &&
                                     (cell2.Doors & cardinal2Opposite) != 0)
                            {
                                openingPosition1 = diag1.Location;
                                openingPosition2 = location2;
                                openingDirection2 = cardinal2Opposite;

                                // Used for enemy movement
                                shouldMoveToOpeningPosition1 = true;

                                return true;
                            }

                            return false;
                        }
                        else if (diag2 != null)
                        {
                            // Current cell -> 2nd off-diagonal cell
                            if ((cell1.Doors & cardinal2) != 0 &&
                                (diag2.Doors & cardinal2Opposite) != 0)
                            {
                                openingPosition1 = location1;
                                openingPosition2 = diag2.Location;
                                openingDirection2 = cardinal2Opposite;

                                return true;
                            }
                            // 2nd off-diagonal cell -> Desired cell
                            else if ((cell2.Doors & cardinal1Opposite) != 0 &&
                                     (diag2.Doors & cardinal1) != 0)
                            {
                                openingPosition1 = diag2.Location;
                                openingPosition2 = location2;
                                openingDirection2 = cardinal1Opposite;

                                // Used for enemy movement
                                shouldMoveToOpeningPosition1 = true;

                                return true;
                            }
                        }

                        return false;
                    }
            }
            return false;
        }
        public bool IsPathToCellThroughWall(LevelGrid grid, CellPoint location1, CellPoint location2)
        {
            var cell1 = grid.GetCell(location1);
            var cell2 = grid.GetCell(location2);

            if (cell1 == null || cell2 == null)
                return false;

            var direction = GetDirectionBetweenAdjacentPoints(location1, location2);
            var oppositeDirection = GetOppositeDirection(direction);

            switch (direction)
            {
                case Compass.N:
                case Compass.S:
                case Compass.E:
                case Compass.W:
                    return ((cell1.Walls & direction) != 0) && ((cell2.Walls & oppositeDirection) != 0);
                case Compass.NE:
                case Compass.NW:
                case Compass.SE:
                case Compass.SW:
                    {
                        Compass cardinal1;
                        Compass cardinal2;

                        var diag1 = grid.GetOffDiagonalCell1(location1, direction, out cardinal1);
                        var diag2 = grid.GetOffDiagonalCell2(location1, direction, out cardinal2);

                        var oppositeCardinal1 = GetOppositeDirection(cardinal1);
                        var oppositeCardinal2 = GetOppositeDirection(cardinal2);

                        if (diag1 == null && diag2 == null)
                            return true;

                        bool b1 = diag1 == null;
                        bool b2 = diag2 == null;

                        if (diag1 != null)
                        {
                            b1 |= (diag1.Walls & oppositeCardinal1) != 0;
                            b1 |= (cell2.Walls & oppositeCardinal2) != 0;
                        }
                        if (diag2 != null)
                        {
                            b2 |= (diag2.Walls & oppositeCardinal2) != 0;
                            b2 |= (cell2.Walls & oppositeCardinal1) != 0;
                        }
                        return b1 && b2;
                    }
            }
            return false;
        }
        public bool IsPathToAdjacentCellBlocked(Level level, CellPoint location1, CellPoint location2, bool includeBlockedByEnemy)
        {
            var cell1 = level.Grid.GetCell(location1);
            var cell2 = level.Grid.GetCell(location2);

            if (cell1 == null || cell2 == null)
                return true;

            if (level.IsCellOccupiedByEnemy(cell2.Location) && includeBlockedByEnemy)
                return true;

            var direction = GetDirectionBetweenAdjacentPoints(location1, location2);
            var oppositeDirection = GetOppositeDirection(direction);

            switch (direction)
            {
                case Compass.N:
                case Compass.S:
                case Compass.E:
                case Compass.W:
                    return ((cell1.Doors & direction) != 0) && ((cell2.Doors & oppositeDirection) != 0) ||
                           ((cell1.Walls & direction) != 0) && ((cell2.Walls & oppositeDirection) != 0);
                case Compass.NE:
                case Compass.NW:
                case Compass.SE:
                case Compass.SW:
                    {
                        Compass cardinal1;
                        Compass cardinal2;

                        var diag1 = level.Grid.GetOffDiagonalCell1(location1, direction, out cardinal1);
                        var diag2 = level.Grid.GetOffDiagonalCell2(location1, direction, out cardinal2);

                        var oppositeCardinal1 = GetOppositeDirection(cardinal1);
                        var oppositeCardinal2 = GetOppositeDirection(cardinal2);

                        if (diag1 == null && diag2 == null)
                            return true;

                        bool b1 = (diag1 == null);
                        bool b2 = (diag2 == null);
                        if (diag1 != null)
                        {
                            b1 |= (diag1.Doors & oppositeCardinal1) != 0;
                            b1 |= (cell2.Doors & oppositeCardinal2) != 0;
                            b1 |= (diag1.Walls & oppositeCardinal1) != 0;
                            b1 |= (cell2.Walls & oppositeCardinal2) != 0;
                            b1 |= (level.IsCellOccupiedByEnemy(diag1.Location) && includeBlockedByEnemy);
                        }
                        if (diag2 != null)
                        {
                            b2 |= (diag2.Doors & oppositeCardinal2) != 0;
                            b2 |= (cell2.Doors & oppositeCardinal1) != 0;
                            b2 |= (diag2.Walls & oppositeCardinal2) != 0;
                            b2 |= (cell2.Walls & oppositeCardinal1) != 0;
                            b2 |= (level.IsCellOccupiedByEnemy(diag2.Location) && includeBlockedByEnemy);
                        }

                        // Both paths are blocked
                        return b1 && b2;
                    }
            }
            return false;
        }
        #endregion

        #region (public) Get Methods
        public CellPoint GetPointInDirection(LevelGrid grid, CellPoint cellPoint, Compass direction)
        {
            switch (direction)
            {
                case Compass.N: return grid[cellPoint.Column, cellPoint.Row - 1]?.Location ?? CellPoint.Empty;
                case Compass.S: return grid[cellPoint.Column, cellPoint.Row + 1]?.Location ?? CellPoint.Empty;
                case Compass.E: return grid[cellPoint.Column + 1, cellPoint.Row]?.Location ?? CellPoint.Empty;
                case Compass.W: return grid[cellPoint.Column - 1, cellPoint.Row]?.Location ?? CellPoint.Empty;
                case Compass.NE: return grid[cellPoint.Column + 1, cellPoint.Row - 1]?.Location ?? CellPoint.Empty;
                case Compass.NW: return grid[cellPoint.Column - 1, cellPoint.Row - 1]?.Location ?? CellPoint.Empty;
                case Compass.SW: return grid[cellPoint.Column - 1, cellPoint.Row + 1]?.Location ?? CellPoint.Empty;
                case Compass.SE: return grid[cellPoint.Column + 1, cellPoint.Row + 1]?.Location ?? CellPoint.Empty;
                default:
                    throw new Exception("Unhandled Compass type");
            }
        }
        public CellPoint GetRandomLocation(Level level, bool excludeOccupiedLocations)
        {
            // Get cell array from the grid
            var cells = level.Grid.GetCells();

            // Slower operation
            if (excludeOccupiedLocations)
            {
                var occupiedLocations = level.GetContents().Select(x => x.Location);

                var freeCells = cells.Where(x => !occupiedLocations.Contains(x.Location));

                // Return random cell
                return freeCells.ElementAt(_randomSequenceGenerator.Get(0, freeCells.Count())).Location;
            }
            // O(1)
            else
            {
                return cells[_randomSequenceGenerator.Get(0, cells.Length)].Location;
            }
        }
        public CellPoint GetRandomLocation(Level level, IEnumerable<CellPoint> otherExcludedLocations, bool excludeOccupiedLocations)
        {
            var locations = level.Grid.GetCells()
                                  .Select(x => x.Location)
                                  .Except(otherExcludedLocations)
                                  .ToList();

            // Slower operation
            if (excludeOccupiedLocations)
            {
                var occupiedLocations = level.GetContents().Select(x => x.Location);

                var freeCells = locations.Except(occupiedLocations);

                // Return random cell
                return freeCells.ElementAt(_randomSequenceGenerator.Get(0, freeCells.Count()));
            }
            // O(1)
            else
            {
                return locations[_randomSequenceGenerator.Get(0, locations.Count)];
            }
        }
        public CellPoint GetRandomAdjacentLocation(Level level, Player player, CellPoint location, bool excludeOccupiedCells)
        {
            var adjacentLocations = GetAdjacentLocations(level.Grid, location).
                                    Where(x => !(excludeOccupiedCells && level.IsCellOccupied(x, player.Location)));

            return adjacentLocations.Any() ? adjacentLocations.ElementAt(_randomSequenceGenerator.Get(0, adjacentLocations.Count()))
                                           : CellPoint.Empty;

        }
        public IEnumerable<CellPoint> GetFreeAdjacentLocations(Level level, Player player, CellPoint location)
        {
            var adjacentLocations = GetAdjacentLocations(level.Grid, location);

            return adjacentLocations.Where(x => x != null && !level.IsCellOccupied(x, player.Location));
        }
        public IEnumerable<CellPoint> GetFreeAdjacentLocationsForMovement(Level level, Player player, CellPoint location)
        {
            var adjacentLocations = GetAdjacentLocations(level.Grid, location);

            return adjacentLocations.Where(x => x != null && !level.IsCellOccupiedByEnemy(x) && !(player.Location == location));
        }
        public IEnumerable<CellPoint> GetAdjacentLocations(LevelGrid grid, CellPoint location)
        {
            var n = grid[location.Column, location.Row - 1]?.Location ?? null;
            var s = grid[location.Column, location.Row + 1]?.Location ?? null;
            var e = grid[location.Column + 1, location.Row]?.Location ?? null;
            var w = grid[location.Column - 1, location.Row - 1]?.Location ?? null;
            var ne = grid[location.Column + 1, location.Row - 1]?.Location ?? null;
            var nw = grid[location.Column - 1, location.Row - 1]?.Location ?? null;
            var se = grid[location.Column + 1, location.Row + 1]?.Location ?? null;
            var sw = grid[location.Column - 1, location.Row + 1]?.Location ?? null;

            var result = new List<CellPoint>() { n, s, e, w, ne, nw, se, sw };

            return result.Where(x => x != null);
        }

        public double EuclideanDistance(CellPoint p1, CellPoint p2)
        {
            double x = p2.Column - p1.Column;
            double y = p2.Row - p1.Row;
            return Math.Sqrt((x * x) + (y * y));
        }
        public double RoguianDistance(CellPoint p1, CellPoint p2)
        {
            double x = Math.Abs(p2.Column - p1.Column);
            double y = Math.Abs(p2.Row - p1.Row);
            return Math.Max(x, y);
        }
        public Compass GetDirectionBetweenAdjacentPoints(CellPoint cell1, CellPoint cell2)
        {
            int deltaX = cell2.Column - cell1.Column;
            int deltaY = cell2.Row - cell1.Row;

            if (deltaX == -1)
            {
                switch (deltaY)
                {
                    case -1: return Compass.NW;
                    case 0: return Compass.W;
                    case 1: return Compass.SW;
                }
            }
            if (deltaX == 0)
            {
                switch (deltaY)
                {
                    case -1: return Compass.N;
                    case 0: return Compass.Null;
                    case 1: return Compass.S;
                }
            }
            if (deltaX == 1)
            {
                switch (deltaY)
                {
                    case -1: return Compass.NE;
                    case 0: return Compass.E;
                    case 1: return Compass.SE;
                }
            }
            return Compass.Null;
        }
        #endregion

        #region (private) Methods
        private Compass GetOppositeDirection(Compass c)
        {
            switch (c)
            {
                case Compass.N:
                    return Compass.S;
                case Compass.S:
                    return Compass.N;
                case Compass.E:
                    return Compass.W;
                case Compass.W:
                    return Compass.E;
                case Compass.NE:
                    return Compass.SW;
                case Compass.NW:
                    return Compass.SE;
                case Compass.SE:
                    return Compass.SW;
                case Compass.SW:
                    return Compass.SE;
                default:
                    return Compass.Null;
            }
        }

        public void ApplyEndOfTurn()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
