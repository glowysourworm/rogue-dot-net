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

namespace Rogue.NET.Core.Logic
{
    [Export(typeof(ILayoutEngine))]
    public class LayoutEngine : ILayoutEngine
    {
        readonly IRandomSequenceGenerator _randomSequenceGenerator;

        public event EventHandler<IScenarioUpdate> ScenarioUpdateEvent;
        public event EventHandler<ISplashUpdate> SplashUpdateEvent;
        public event EventHandler<ILevelUpdate> LevelUpdateEvent;
        public event EventHandler<IAnimationUpdate> AnimationUpdateEvent;
        public event EventHandler<ILevelProcessingAction> LevelProcessingActionEvent;

        [ImportingConstructor]
        public LayoutEngine(IRandomSequenceGenerator randomSequenceGenerator)
        {
            _randomSequenceGenerator = randomSequenceGenerator;
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
        }
        public void ToggleDoor(LevelGrid grid, Compass direction, CellPoint characterLocation)
        {
            var nextPoint = GetPointInDirection(grid, characterLocation, direction);
            var openingPosition = CellPoint.Empty;
            var openingDirection = Compass.Null;

            if (IsCellThroughDoor(grid, characterLocation, nextPoint, out openingPosition, out openingDirection))
            {
                var characterCell = grid.GetCell(characterLocation);
                var openingPositionCell = grid.GetCell(openingPosition);

                characterCell.ToggleDoor(direction);
                openingPositionCell.ToggleDoor(GetOppositeDirection(direction));
            }
        }
        #endregion

        #region (public) Query Methods
        public bool IsCellThroughDoor(LevelGrid grid, CellPoint point1, CellPoint point2, out CellPoint openingPosition, out Compass openingDirection)
        {
            openingPosition = CellPoint.Empty;
            openingDirection = Compass.Null;

            Cell c1 = grid.GetCell(point1);
            Cell c2 = grid.GetCell(point2);
            if (c1 == null || c2 == null)
                return false;

            Compass c = GetDirectionBetweenAdjacentPoints(point1, point2);
            switch (c)
            {
                case Compass.N:
                    openingPosition = point1;
                    openingDirection = c;
                    return ((c1.Doors & Compass.N) != 0) && ((c2.Doors & Compass.S) != 0);
                case Compass.S:
                    openingPosition = point1;
                    openingDirection = c;
                    return ((c1.Doors & Compass.S) != 0) && ((c2.Doors & Compass.N) != 0);
                case Compass.E:
                    openingPosition = point1;
                    openingDirection = c;
                    return ((c1.Doors & Compass.E) != 0) && ((c2.Doors & Compass.W) != 0);
                case Compass.W:
                    openingPosition = point1;
                    openingDirection = c;
                    return ((c1.Doors & Compass.W) != 0) && ((c2.Doors & Compass.E) != 0);
                case Compass.NE:
                    {
                        Cell diag1 = grid.GetCell(point1.Column, point1.Row - 1);
                        Cell diag2 = grid.GetCell(point1.Column + 1, point1.Row);
                        if (diag1 == null && diag2 == null)
                            return true;

                        if (diag1 != null)
                        {
                            if ((diag1.Doors & Compass.S) != 0)
                            {
                                openingPosition = point1;
                                openingDirection = Compass.N;
                            }
                            if ((c2.Doors & Compass.W) != 0)
                            {
                                openingPosition = diag1.Location;
                                openingDirection = Compass.E;
                            }
                        }
                        if (diag2 != null)
                        {
                            if ((diag2.Doors & Compass.W) != 0)
                            {
                                openingPosition = point1;
                                openingDirection = Compass.E;
                            }
                            if ((c2.Doors & Compass.S) != 0)
                            {
                                openingPosition = diag2.Location;
                                openingDirection = Compass.N;
                            }
                        }
                        return openingPosition != CellPoint.Empty;
                    }
                case Compass.NW:
                    {
                        Cell diag1 = grid.GetCell(point1.Column, point1.Row - 1);
                        Cell diag2 = grid.GetCell(point1.Column - 1, point1.Row);
                        if (diag1 == null && diag2 == null)
                            return true;

                        if (diag1 != null)
                        {
                            if ((diag1.Doors & Compass.S) != 0)
                            {
                                openingPosition = point1;
                                openingDirection = Compass.N;
                            }
                            if ((c2.Doors & Compass.E) != 0)
                            {
                                openingPosition = diag1.Location;
                                openingDirection = Compass.W;
                            }
                        }
                        if (diag2 != null)
                        {
                            if ((diag2.Doors & Compass.E) != 0)
                            {
                                openingPosition = point1;
                                openingDirection = Compass.W;
                            }
                            if ((c2.Doors & Compass.S) != 0)
                            {
                                openingPosition = diag2.Location;
                                openingDirection = Compass.N;
                            }
                        }
                        return openingPosition != CellPoint.Empty;
                    }
                case Compass.SE:
                    {
                        Cell diag1 = grid.GetCell(point1.Column, point1.Row + 1);
                        Cell diag2 = grid.GetCell(point1.Column + 1, point1.Row);
                        if (diag1 == null && diag2 == null)
                            return true;

                        if (diag1 != null)
                        {
                            if ((diag1.Doors & Compass.N) != 0)
                            {
                                openingPosition = point1;
                                openingDirection = Compass.S;
                            }
                            if ((c2.Doors & Compass.W) != 0)
                            {
                                openingPosition = diag1.Location;
                                openingDirection = Compass.E;
                            }
                        }
                        if (diag2 != null)
                        {
                            if ((diag2.Doors & Compass.W) != 0)
                            {
                                openingPosition = point1;
                                openingDirection = Compass.E;
                            }
                            if ((c2.Doors & Compass.N) != 0)
                            {
                                openingPosition = diag2.Location;
                                openingDirection = Compass.S;
                            }
                        }
                        return openingPosition != CellPoint.Empty;
                    }
                case Compass.SW:
                    {
                        Cell diag1 = grid.GetCell(point1.Column, point1.Row + 1);
                        Cell diag2 = grid.GetCell(point1.Column - 1, point1.Row);
                        if (diag1 == null && diag2 == null)
                            return true;

                        if (diag1 != null)
                        {
                            if ((diag1.Doors & Compass.N) != 0)
                            {
                                openingPosition = point1;
                                openingDirection = Compass.S;
                            }
                            if ((c2.Doors & Compass.E) != 0)
                            {
                                openingPosition = diag1.Location;
                                openingDirection = Compass.W;
                            }
                        }
                        if (diag2 != null)
                        {
                            if ((diag2.Doors & Compass.E) != 0)
                            {
                                openingPosition = point1;
                                openingDirection = Compass.W;
                            }
                            if ((c2.Doors & Compass.N) != 0)
                            {
                                openingPosition = diag2.Location;
                                openingDirection = Compass.S;
                            }
                        }
                        return openingPosition != CellPoint.Empty;
                    }
            }
            return false;
        }
        public bool IsCellThroughWall(LevelGrid grid, CellPoint point1, CellPoint point2)
        {
            Cell cell1 = grid.GetCell(point1);
            Cell cell2 = grid.GetCell(point2);

            if (cell1 == null || cell2 == null)
                return false;

            Compass direction = GetDirectionBetweenAdjacentPoints(point1, point2);
            switch (direction)
            {
                case Compass.N:
                    return ((cell1.Walls & Compass.N) != 0) && ((cell2.Walls & Compass.S) != 0);
                case Compass.S:
                    return ((cell1.Walls & Compass.S) != 0) && ((cell2.Walls & Compass.N) != 0);
                case Compass.E:
                    return ((cell1.Walls & Compass.E) != 0) && ((cell2.Walls & Compass.W) != 0);
                case Compass.W:
                    return ((cell1.Walls & Compass.W) != 0) && ((cell2.Walls & Compass.E) != 0);
                case Compass.NE:
                    {
                        Cell diag1 = grid.GetCell(point1.Column, point1.Row - 1);
                        Cell diag2 = grid.GetCell(point1.Column + 1, point1.Row);
                        if (diag1 == null && diag2 == null)
                            return true;

                        bool b1 = diag1 == null;
                        bool b2 = diag2 == null;
                        if (diag1 != null)
                        {
                            b1 |= (diag1.Walls & Compass.S) != 0;
                            b1 |= (cell2.Walls & Compass.W) != 0;
                        }
                        if (diag2 != null)
                        {
                            b2 |= (diag2.Walls & Compass.W) != 0;
                            b2 |= (cell2.Walls & Compass.S) != 0;
                        }
                        return b1 && b2;
                    }
                case Compass.NW:
                    {
                        Cell diag1 = grid.GetCell(point1.Column, point1.Row - 1);
                        Cell diag2 = grid.GetCell(point1.Column - 1, point1.Row);
                        if (diag1 == null && diag2 == null)
                            return true;

                        bool b1 = diag1 == null;
                        bool b2 = diag2 == null;
                        if (diag1 != null)
                        {
                            b1 |= (diag1.Walls & Compass.S) != 0;
                            b1 |= (cell2.Walls & Compass.E) != 0;
                        }
                        if (diag2 != null)
                        {
                            b2 |= (diag2.Walls & Compass.E) != 0;
                            b2 |= (cell2.Walls & Compass.S) != 0;
                        }
                        return b1 && b2;
                    }
                case Compass.SE:
                    {
                        Cell diag1 = grid.GetCell(point1.Column, point1.Row + 1);
                        Cell diag2 = grid.GetCell(point1.Column + 1, point1.Row);
                        if (diag1 == null && diag2 == null)
                            return true;

                        bool b1 = diag1 == null;
                        bool b2 = diag2 == null;
                        if (diag1 != null)
                        {
                            b1 |= (diag1.Walls & Compass.N) != 0;
                            b1 |= (cell2.Walls & Compass.W) != 0;
                        }
                        if (diag2 != null)
                        {
                            b2 |= (diag2.Walls & Compass.W) != 0;
                            b2 |= (cell2.Walls & Compass.N) != 0;
                        }
                        return b1 && b2;
                    }
                case Compass.SW:
                    {
                        Cell diag1 = grid.GetCell(point1.Column, point1.Row + 1);
                        Cell diag2 = grid.GetCell(point1.Column - 1, point1.Row);
                        if (diag1 == null && diag2 == null)
                            return true;

                        bool b1 = diag1 == null;
                        bool b2 = diag2 == null;
                        if (diag1 != null)
                        {
                            b1 |= (diag1.Walls & Compass.N) != 0;
                            b1 |= (cell2.Walls & Compass.E) != 0;
                        }
                        if (diag2 != null)
                        {
                            b2 |= (diag2.Walls & Compass.E) != 0;
                            b2 |= (cell2.Walls & Compass.N) != 0;
                        }
                        return b1 && b2;
                    }
            }
            return false;
        }
        public bool IsPathToAdjacentCellBlocked(Level level, CellPoint point1, CellPoint point2)
        {
            var cell1 = level.Grid.GetCell(point1);
            var cell2 = level.Grid.GetCell(point2);

            if (cell1 == null || cell2 == null)
                return true;

            if (level.IsCellOccupiedByEnemy(cell2.Location))
                return true;

            switch (GetDirectionBetweenAdjacentPoints(point1, point2))
            {
                case Compass.N:
                    {
                        bool b = ((cell1.Doors & Compass.N) != 0) && ((cell2.Doors & Compass.S) != 0);
                        b |= ((cell1.Walls & Compass.N) != 0) && ((cell2.Walls & Compass.S) != 0);
                        return b;
                    }
                case Compass.S:
                    {
                        bool b = ((cell1.Doors & Compass.S) != 0) && ((cell2.Doors & Compass.N) != 0);
                        b |= ((cell1.Walls & Compass.S) != 0) && ((cell2.Walls & Compass.N) != 0);
                        return b;
                    }
                case Compass.E:
                    {
                        bool b = ((cell1.Doors & Compass.E) != 0) && ((cell2.Doors & Compass.W) != 0);
                        b |= ((cell1.Walls & Compass.E) != 0) && ((cell2.Walls & Compass.W) != 0);
                        return b;
                    }
                case Compass.W:
                    {
                        bool b = ((cell1.Doors & Compass.W) != 0) && ((cell2.Doors & Compass.E) != 0);
                        b |= ((cell1.Walls & Compass.W) != 0) && ((cell2.Walls & Compass.E) != 0);
                        return b;
                    }
                case Compass.NE:
                    {
                        Cell diag1 = level.Grid.GetCell(point1.Column, point1.Row - 1);
                        Cell diag2 = level.Grid.GetCell(point1.Column + 1, point1.Row);
                        if (diag1 == null && diag2 == null)
                            return true;

                        bool b1 = (diag1 == null);
                        bool b2 = (diag2 == null);
                        if (diag1 != null)
                        {
                            b1 |= (diag1.Doors & Compass.S) != 0;
                            b1 |= (cell2.Doors & Compass.W) != 0;
                            b1 |= (diag1.Walls & Compass.S) != 0;
                            b1 |= (cell2.Walls & Compass.W) != 0;
                            b1 |= level.IsCellOccupiedByEnemy(diag1.Location);
                        }
                        if (diag2 != null)
                        {
                            b2 |= (diag2.Doors & Compass.W) != 0;
                            b2 |= (cell2.Doors & Compass.S) != 0;
                            b2 |= (diag2.Walls & Compass.W) != 0;
                            b2 |= (cell2.Walls & Compass.S) != 0;
                            b2 |= level.IsCellOccupiedByEnemy(diag2.Location);
                        }
                        return (b1 && b2);
                    }
                case Compass.NW:
                    {
                        Cell diag1 = level.Grid.GetCell(point1.Column, point1.Row - 1);
                        Cell diag2 = level.Grid.GetCell(point1.Column - 1, point1.Row);
                        if (diag1 == null && diag2 == null)
                            return true;

                        bool b1 = (diag1 == null);
                        bool b2 = (diag2 == null);
                        if (diag1 != null)
                        {
                            b1 |= (diag1.Doors & Compass.S) != 0;
                            b1 |= (cell2.Doors & Compass.E) != 0;
                            b1 |= (diag1.Walls & Compass.S) != 0;
                            b1 |= (cell2.Walls & Compass.E) != 0;
                            b1 |= level.IsCellOccupiedByEnemy(diag1.Location);
                        }
                        if (diag2 != null)
                        {
                            b2 |= (diag2.Doors & Compass.E) != 0;
                            b2 |= (cell2.Doors & Compass.S) != 0;
                            b2 |= (diag2.Walls & Compass.E) != 0;
                            b2 |= (cell2.Walls & Compass.S) != 0;
                            b2 |= level.IsCellOccupiedByEnemy(diag2.Location);
                        }
                        return (b1 && b2);
                    }
                case Compass.SE:
                    {
                        Cell diag1 = level.Grid.GetCell(point1.Column, point1.Row + 1);
                        Cell diag2 = level.Grid.GetCell(point1.Column + 1, point1.Row);
                        if (diag1 == null && diag2 == null)
                            return true;

                        bool b1 = (diag1 == null);
                        bool b2 = (diag2 == null);
                        if (diag1 != null)
                        {
                            b1 |= (diag1.Doors & Compass.N) != 0;
                            b1 |= (cell2.Doors & Compass.W) != 0;
                            b1 |= (diag1.Walls & Compass.N) != 0;
                            b1 |= (cell2.Walls & Compass.W) != 0;
                            b1 |= level.IsCellOccupiedByEnemy(diag1.Location);
                        }
                        if (diag2 != null)
                        {
                            b2 |= (diag2.Doors & Compass.W) != 0;
                            b2 |= (cell2.Doors & Compass.N) != 0;
                            b2 |= (diag2.Walls & Compass.W) != 0;
                            b2 |= (cell2.Walls & Compass.N) != 0;
                            b2 |= level.IsCellOccupiedByEnemy(diag2.Location);
                        }
                        return (b1 && b2);
                    }
                case Compass.SW:
                    {
                        Cell diag1 = level.Grid.GetCell(point1.Column, point1.Row + 1);
                        Cell diag2 = level.Grid.GetCell(point1.Column - 1, point1.Row);
                        if (diag1 == null && diag2 == null)
                            return true;

                        bool b1 = (diag1 == null);
                        bool b2 = (diag2 == null);
                        if (diag1 != null)
                        {
                            b1 |= (diag1.Doors & Compass.N) != 0;
                            b1 |= (cell2.Doors & Compass.E) != 0;
                            b1 |= (diag1.Walls & Compass.N) != 0;
                            b1 |= (cell2.Walls & Compass.E) != 0;
                            b1 |= level.IsCellOccupiedByEnemy(diag1.Location);
                        }
                        if (diag2 != null)
                        {
                            b2 |= (diag2.Doors & Compass.E) != 0;
                            b2 |= (cell2.Doors & Compass.N) != 0;
                            b2 |= (diag2.Walls & Compass.E) != 0;
                            b2 |= (cell2.Walls & Compass.N) != 0;
                            b2 |= level.IsCellOccupiedByEnemy(diag2.Location);
                        }
                        return (b1 && b2);
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
        public CellPoint GetRandomLocation(Level level, bool excludeOccupiedCells)
        {
            // Get cell array from the grid
            var cells = level.Grid.GetCells();

            // Slower operation
            if (excludeOccupiedCells)
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
        public CellPoint GetRandomAdjacentLocation(Level level, CellPoint location, bool excludeOccupiedCells)
        {
            var adjacentLocations = GetAdjacentLocations(level.Grid, location).
                                    Where(x => (excludeOccupiedCells && level.IsCellOccupied(x)));

            return adjacentLocations.Any() ? adjacentLocations.ElementAt(_randomSequenceGenerator.Get(0, adjacentLocations.Count()))
                                           : CellPoint.Empty;

        }
        public IEnumerable<CellPoint> GetFreeAdjacentLocations(Level level, CellPoint location)
        {
            var adjacentLocations = GetAdjacentLocations(level.Grid, location);

            return adjacentLocations.Where(x => x != null && !level.IsCellOccupied(x));
        }
        public IEnumerable<CellPoint> GetFreeAdjacentLocationsForMovement(Level level, CellPoint location)
        {
            var adjacentLocations = GetAdjacentLocations(level.Grid, location);

            return adjacentLocations.Where(x => x != null && !level.IsCellOccupiedByEnemy(x));
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
