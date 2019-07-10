using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Generator.Interface;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Logic.Interface;
using Rogue.NET.Core.Logic.Processing.Interface;
using Rogue.NET.Core.Logic.Processing;
using Rogue.NET.Core.Logic.Processing.Enum;
using Rogue.NET.Core.Model.Scenario;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.Scenario.Content.Extension;
using Rogue.NET.Core.Model.ScenarioMessage;
using Rogue.NET.Core.Service.Interface;
using Rogue.NET.Core.Logic.Static;
using Rogue.NET.Core.Logic.Processing.Factory.Interface;

namespace Rogue.NET.Core.Logic
{
    [Export(typeof(ILayoutEngine))]
    public class LayoutEngine : ILayoutEngine
    {
        readonly IRandomSequenceGenerator _randomSequenceGenerator;
        readonly IScenarioMessageService _scenarioMessageService;
        readonly IModelService _modelService;
        readonly IRogueUpdateFactory _rogueUpdateFactory;

        public event EventHandler<RogueUpdateEventArgs> RogueUpdateEvent;
        public event EventHandler<ILevelProcessingAction> LevelProcessingActionEvent;

        [ImportingConstructor]
        public LayoutEngine(IRandomSequenceGenerator randomSequenceGenerator, 
                            IScenarioMessageService scenarioMessageService,
                            IModelService modelService,
                            IRogueUpdateFactory rogueUpdateFactory)
        {
            _randomSequenceGenerator = randomSequenceGenerator;
            _scenarioMessageService = scenarioMessageService;
            _modelService = modelService;
            _rogueUpdateFactory = rogueUpdateFactory;
        }

        #region (public) Player Action Methods
        public void Search(LevelGrid grid, CellPoint location)
        {
            Cell c = grid[location.Column, location.Row];
            Cell n = grid[location.Column, location.Row - 1];
            Cell s = grid[location.Column, location.Row + 1];
            Cell e = grid[location.Column + 1, location.Row];
            Cell w = grid[location.Column - 1, location.Row];
            Cell ne = grid[location.Column + 1, location.Row - 1];
            Cell nw = grid[location.Column - 1, location.Row - 1];
            Cell se = grid[location.Column + 1, location.Row + 1];
            Cell sw = grid[location.Column - 1, location.Row + 1];

            var cells = new Cell[] { c, n, s, e, w, ne, sw, se, sw }.Where(x => x != null).ToArray();
            var visibleDoors = cells.Select(x => x.VisibleDoors).ToArray();

            // Search all cells current and adjacent
            cells.ForEach(cell => cell.SearchDoors());

            var topologyChange = false;
            for (int i=0;i<cells.Length && !topologyChange;i++)
            {
                topologyChange = topologyChange || (cells[i].VisibleDoors != visibleDoors[i]);
            }

            if (topologyChange)
            {
                _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "Door found!");

                _modelService.UpdateVisibleLocations();

                RogueUpdateEvent(this, _rogueUpdateFactory.Update(LevelUpdateType.LayoutTopology, ""));
            }
            else
                _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "Search " + Enumerable.Range(1, _randomSequenceGenerator.Get(2, 5)).Aggregate<int,string>("", (accum, x) => accum + "."));
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

                var characterCell = grid[characterLocation.Column, characterLocation.Row];
                var openingPositionCell = grid[openingPosition2.Column, openingPosition2.Row];

                characterCell.OpenDoor(direction);
                openingPositionCell.OpenDoor(openingDirection2);

                _modelService.UpdateVisibleLocations();
                _modelService.UpdateContents();

                RogueUpdateEvent(this, _rogueUpdateFactory.Update(LevelUpdateType.LayoutTopology, ""));
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

            var location2 = grid.GetPointInDirection(location1, openingDirection1);

            Cell cell1 = grid[location1.Column, location1.Row];
            Cell cell2 = grid[location2.Column, location2.Row];

            if (cell1 == null || cell2 == null)
                return false;

            var direction = LevelGridExtension.GetDirectionBetweenAdjacentPoints(location1, location2);
            var oppositeDirection = LevelGridExtension.GetOppositeDirection(direction);

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

                        var cardinal1Opposite = LevelGridExtension.GetOppositeDirection(cardinal1);
                        var cardinal2Opposite = LevelGridExtension.GetOppositeDirection(cardinal2);

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
        public bool IsPathToCellThroughWall(Level level, CellPoint location1, CellPoint location2, bool includeBlockedByEnemy)
        {
            var grid = level.Grid;
            var cell1 = grid[location1.Column, location1.Row];
            var cell2 = grid[location2.Column, location2.Row];

            if (cell1 == null || cell2 == null)
                return false;

            if (level.IsCellOccupiedByEnemy(cell2.Location) && includeBlockedByEnemy)
                return true;

            var direction = LevelGridExtension.GetDirectionBetweenAdjacentPoints(location1, location2);
            var oppositeDirection = LevelGridExtension.GetOppositeDirection(direction);

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

                        var oppositeCardinal1 = LevelGridExtension.GetOppositeDirection(cardinal1);
                        var oppositeCardinal2 = LevelGridExtension.GetOppositeDirection(cardinal2);

                        if (diag1 == null && diag2 == null)
                            return true;

                        bool b1 = diag1 == null;
                        bool b2 = diag2 == null;

                        if (diag1 != null)
                        {
                            b1 |= (diag1.Walls & oppositeCardinal1) != 0;
                            b1 |= (cell2.Walls & oppositeCardinal2) != 0;
                            b1 |= (level.IsCellOccupiedByEnemy(diag1.Location) && includeBlockedByEnemy);
                        }
                        if (diag2 != null)
                        {
                            b2 |= (diag2.Walls & oppositeCardinal2) != 0;
                            b2 |= (cell2.Walls & oppositeCardinal1) != 0;
                            b2 |= (level.IsCellOccupiedByEnemy(diag2.Location) && includeBlockedByEnemy);
                        }
                        return b1 && b2;
                    }
            }
            return false;
        }
        public bool IsPathToAdjacentCellBlocked(Level level, CellPoint location1, CellPoint location2, bool includeBlockedByEnemy)
        {
            var cell1 = level.Grid[location1.Column, location1.Row];
            var cell2 = level.Grid[location2.Column, location2.Row];

            if (cell1 == null || cell2 == null)
                return true;

            if (level.IsCellOccupiedByEnemy(cell2.Location) && includeBlockedByEnemy)
                return true;

            var direction = LevelGridExtension.GetDirectionBetweenAdjacentPoints(location1, location2);
            var oppositeDirection = LevelGridExtension.GetOppositeDirection(direction);

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

                        var oppositeCardinal1 = LevelGridExtension.GetOppositeDirection(cardinal1);
                        var oppositeCardinal2 = LevelGridExtension.GetOppositeDirection(cardinal2);

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
        public CellPoint GetRandomAdjacentLocation(Level level, Player player, CellPoint location, bool excludeOccupiedCells)
        {
            var adjacentLocations = level.
                                    Grid.
                                    GetAdjacentLocations(location).
                                    Where(x => !(excludeOccupiedCells && level.IsCellOccupied(x, player.Location)));

            return adjacentLocations.Any() ? adjacentLocations.ElementAt(_randomSequenceGenerator.Get(0, adjacentLocations.Count()))
                                           : CellPoint.Empty;

        }
        public IEnumerable<CellPoint> GetFreeAdjacentLocations(Level level, Player player, CellPoint location)
        {
            var adjacentLocations = level.Grid.GetAdjacentLocations(location);

            return adjacentLocations.Where(x => x != null && !level.IsCellOccupied(x, player.Location));
        }
        public IEnumerable<CellPoint> GetFreeAdjacentLocationsForMovement(Level level, Player player, CellPoint location)
        {
            var adjacentLocations = level.Grid.GetAdjacentLocations(location);

            return adjacentLocations.Where(x => x != null && !level.IsCellOccupiedByEnemy(x) && !(player.Location == location));
        }
        #endregion

        #region (private) Methods
        public void ApplyEndOfTurn()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
