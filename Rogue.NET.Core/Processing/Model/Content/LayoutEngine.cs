﻿using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Model.Scenario;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.Scenario.Content.Extension;
using Rogue.NET.Core.Processing.Event.Backend.EventData.Factory.Interface;
using Rogue.NET.Core.Processing.Event.Backend.EventData.ScenarioMessage.Enum;
using Rogue.NET.Core.GameRouter.GameEvent.Backend.Enum;
using Rogue.NET.Core.Processing.Model.Content.Interface;
using Rogue.NET.Core.Processing.Model.Generator.Interface;
using Rogue.NET.Core.Processing.Service.Interface;
using Rogue.NET.Core.Processing.Model.Static;

namespace Rogue.NET.Core.Processing.Model.Content
{
    [Export(typeof(ILayoutEngine))]
    public class LayoutEngine : BackendEngine, ILayoutEngine
    {
        readonly IRandomSequenceGenerator _randomSequenceGenerator;
        readonly IScenarioMessageService _scenarioMessageService;
        readonly IModelService _modelService;
        readonly IBackendEventDataFactory _backendEventDataFactory;

        [ImportingConstructor]
        public LayoutEngine(IRandomSequenceGenerator randomSequenceGenerator, 
                            IScenarioMessageService scenarioMessageService,
                            IModelService modelService,
                            IBackendEventDataFactory backendEventDataFactory)
        {
            _randomSequenceGenerator = randomSequenceGenerator;
            _scenarioMessageService = scenarioMessageService;
            _modelService = modelService;
            _backendEventDataFactory = backendEventDataFactory;
        }

        #region (public) Player Action Methods
        public void Search(GridLocation location)
        {
            var grid = _modelService.Level.Grid;

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

                _modelService.UpdateVisibility();

                OnLevelEvent(_backendEventDataFactory.Event(LevelEventType.LayoutTopology, ""));
            }
            else
                _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "Search " + Enumerable.Range(1, _randomSequenceGenerator.Get(2, 5)).Aggregate<int,string>("", (accum, x) => accum + "."));
        }
        public void ToggleDoor(Compass direction, GridLocation characterLocation)
        {
            var grid = _modelService.Level.Grid;

            var openingPosition1 = GridLocation.Empty;
            var openingPosition2 = GridLocation.Empty;
            var openingDirection2 = Compass.Null;
            var shouldMoveToOpeningPosition1 = false;

            if (IsPathToCellThroughDoor(characterLocation, direction, out openingPosition1, out openingPosition2, out openingDirection2, out shouldMoveToOpeningPosition1))
            {
                // Have to move into position first
                if (shouldMoveToOpeningPosition1)
                    return;

                var characterCell = grid[characterLocation.Column, characterLocation.Row];
                var openingPositionCell = grid[openingPosition2.Column, openingPosition2.Row];

                characterCell.OpenDoor(direction);
                openingPositionCell.OpenDoor(openingDirection2);

                _modelService.UpdateVisibility();

                // Notify Front-End of a change in the layout
                OnLevelEvent(_backendEventDataFactory.Event(LevelEventType.LayoutTopology, ""));
            }
        }
        #endregion

        #region (public) Query Methods
        public bool IsPathToCellThroughDoor(
            GridLocation location1, 
            Compass openingDirection1,                 // Represents the Door for location1
            out GridLocation openingPosition1,         // Represents the opening position for the door
            out GridLocation openingPosition2,         // Represents the same door opposite cell
            out Compass openingDirection2,             // Represents the Door for location2
            out bool shouldMoveToOpeningPosition1)     // Should move into position for opening the door before opening
        {
            var grid = _modelService.Level.Grid;

            openingPosition1 = GridLocation.Empty;
            openingPosition2 = GridLocation.Empty;
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

        public bool IsPathToCellThroughWall(GridLocation location1, 
                                            GridLocation location2, 
                                            bool includeBlockedByCharacters,
                                            CharacterAlignmentType excludedAlignmentType = CharacterAlignmentType.None)
        {
            var level = _modelService.Level;
            var grid = level.Grid;
            var cell1 = grid[location1.Column, location1.Row];
            var cell2 = grid[location2.Column, location2.Row];

            if (cell1 == null || cell2 == null)
                return false;

            // Check that the cell is occupied by a character of the other faction
            var character = level.GetAt<NonPlayerCharacter>(cell2.Location);

            if (character != null && 
                includeBlockedByCharacters &&
                character.AlignmentType != excludedAlignmentType)
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
                            b1 |= (level.IsCellOccupiedByCharacter(diag1.Location, _modelService.Player.Location) && includeBlockedByCharacters);
                        }
                        if (diag2 != null)
                        {
                            b2 |= (diag2.Walls & oppositeCardinal2) != 0;
                            b2 |= (cell2.Walls & oppositeCardinal1) != 0;
                            b2 |= (level.IsCellOccupiedByCharacter(diag2.Location, _modelService.Player.Location) && includeBlockedByCharacters);
                        }
                        return b1 && b2;
                    }
            }
            return false;
        }

        public bool IsPathToAdjacentCellBlocked(GridLocation location1, 
                                                GridLocation location2, 
                                                bool includeBlockedByCharacters,
                                                CharacterAlignmentType excludedAlignmentType = CharacterAlignmentType.None)
        {
            var level = _modelService.Level;

            var cell1 = level.Grid[location1.Column, location1.Row];
            var cell2 = level.Grid[location2.Column, location2.Row];

            if (cell1 == null || cell2 == null)
                return true;

            // Check that the cell is occupied by a character of the other faction
            var character = level.GetAt<NonPlayerCharacter>(cell2.Location);

            if (character != null &&
                includeBlockedByCharacters &&
                character.AlignmentType != excludedAlignmentType)
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
                            b1 |= (level.IsCellOccupiedByCharacter(diag1.Location, _modelService.Player.Location) && includeBlockedByCharacters);
                        }
                        if (diag2 != null)
                        {
                            b2 |= (diag2.Doors & oppositeCardinal2) != 0;
                            b2 |= (cell2.Doors & oppositeCardinal1) != 0;
                            b2 |= (diag2.Walls & oppositeCardinal2) != 0;
                            b2 |= (cell2.Walls & oppositeCardinal1) != 0;
                            b2 |= (level.IsCellOccupiedByCharacter(diag2.Location, _modelService.Player.Location) && includeBlockedByCharacters);
                        }

                        // Both paths are blocked
                        return b1 && b2;
                    }
            }
            return false;
        }
        #endregion

        #region (public) Get Methods
        public GridLocation GetRandomAdjacentLocationForMovement(GridLocation location, CharacterAlignmentType swappableAlignmentType = CharacterAlignmentType.None)
        {
            var level = _modelService.Level;
            var player = _modelService.Player;

            var adjacentLocations = level.
                                    Grid.
                                    GetAdjacentLocations(location).
                                    Where(x =>
                                    {
                                        var character = level.GetAt<NonPlayerCharacter>(x);

                                        // No Character (OR) Alignment Type Matches
                                        return character == null ? true : character.AlignmentType == swappableAlignmentType;
                                    });

            return adjacentLocations.Any() ? adjacentLocations.ElementAt(_randomSequenceGenerator.Get(0, adjacentLocations.Count()))
                                           : GridLocation.Empty;

        }
        public IEnumerable<GridLocation> GetFreeAdjacentLocations(GridLocation location)
        {
            var level = _modelService.Level;
            var player = _modelService.Player;

            var adjacentLocations = level.Grid.GetAdjacentLocations(location);

            return adjacentLocations.Where(x => x != null && !level.IsCellOccupied(x, player.Location));
        }
        public IEnumerable<GridLocation> GetFreeAdjacentLocationsForMovement(GridLocation location, CharacterAlignmentType swappableAlignmentType = CharacterAlignmentType.None)
        {
            var level = _modelService.Level;

            return  level.
                    Grid.
                    GetAdjacentLocations(location).
                    Where(x =>
                    {
                        var character = level.GetAt<NonPlayerCharacter>(x);

                        // No Character (OR) Alignment Type Matches
                        return character == null ? true : character.AlignmentType == swappableAlignmentType;
                    });
        }
        public IEnumerable<GridLocation> GetLocationsInRange(GridLocation location, int cellRange, bool includeSourceLocation)
        {
            var level = _modelService.Level;

            // Calculate locations within a cell-range using a "pseudo-euclidean" measure to make
            // an elliptical shape. (not roguian - which would make a rectangular shape)

            // 0) Start by calculating the "square" around the location
            // 1) Narrow the result by calculating the euclidean norm of the cell location differences

            var result = new List<GridLocation>();

            // Iterate from the top left corner to the bottom right - respecting grid boundaries
            for (int i = Math.Max(location.Column - cellRange, 0); 
                    (i <= location.Column + cellRange) && 
                    (i < level.Grid.Bounds.Right); 
                    i++)
            {
                for (int j = Math.Max(location.Row - cellRange, 0); 
                        (j <= location.Row + cellRange) && 
                        (j < level.Grid.Bounds.Bottom); 
                        j++)
                {
                    // Check for an empty space
                    if (level.Grid[i, j] == null)
                        continue;

                    // Check for source location
                    if (level.Grid[i, j].Location == location &&
                       !includeSourceLocation)
                        continue;

                    // Check the range
                    if (Calculator.RoguianDistance(level.Grid[i, j].Location, location) <= cellRange)
                        result.Add(level.Grid[i, j].Location);
                }
            }

            return result;
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
