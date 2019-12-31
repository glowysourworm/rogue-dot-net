using Rogue.NET.Core.Math.Geometry;
using Rogue.NET.Core.Media.SymbolEffect.Utility;
using Rogue.NET.Core.Model;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Processing.Model.Content.Calculator;
using Rogue.NET.Core.Processing.Model.Generator.Interface;
using Rogue.NET.Core.Processing.Service.Interface;

using System.Collections.Generic;
using System.Linq;

namespace Rogue.NET.Core.Processing.Service
{
    public class ModelLayoutService : IModelLayoutService
    {
        readonly Level _level;
        readonly IRandomSequenceGenerator _randomSequenceGenerator;

        /// <summary>
        /// Non-importing constructor - should be loaded once per level
        /// </summary>
        public ModelLayoutService(Level level, IRandomSequenceGenerator randomSequenceGenerator)
        {
            _level = level;
            _randomSequenceGenerator = randomSequenceGenerator;
        }

        public void CalculateEffectiveLighting()
        {
            // For now, this calculation just copies the cell's lighting value to the effective value. This will
            // soon be using all light sources in the calculation.
            //
            foreach (var location in _level.Grid.FullMap.GetLocations())
                _level.Grid[location.Column, location.Row].EffectiveLighting = ColorFilter.Discretize(_level.Grid[location.Column, location.Row].BaseLight,     
                                                                                                      ModelConstants.ColorChannelDiscretization);
        }

        public bool IsPathToAdjacentCellBlocked(GridLocation location1,
                                                GridLocation location2,
                                                bool includeBlockedByCharacters,
                                                CharacterAlignmentType excludedAlignmentType = CharacterAlignmentType.None)
        {
            var cell1 = _level.Grid[location1.Column, location1.Row];
            var cell2 = _level.Grid[location2.Column, location2.Row];

            if (cell1 == null || cell2 == null)
                return true;

            if (_level.Grid.ImpassableTerrainMap[location2.Column, location2.Row] != null)
                return true;

            // Check that the cell is occupied by a character of the other faction
            var character = _level.Content.GetAt<NonPlayerCharacter>(cell2.Location);

            if (character != null &&
                includeBlockedByCharacters &&
                character.AlignmentType != excludedAlignmentType)
                return true;

            var direction = GridCalculator.GetDirectionOfAdjacentLocation(location1, location2);

            switch (direction)
            {
                case Compass.N:
                case Compass.S:
                case Compass.E:
                case Compass.W:
                    return cell2.IsWall;

                case Compass.NE:
                case Compass.NW:
                case Compass.SE:
                case Compass.SW:
                    {
                        Compass cardinal1;
                        Compass cardinal2;

                        var diag1 = _level.Grid.GetOffDiagonalCell1(location1, direction, out cardinal1);
                        var diag2 = _level.Grid.GetOffDiagonalCell2(location1, direction, out cardinal2);

                        var oppositeCardinal1 = GridCalculator.GetOppositeDirection(cardinal1);
                        var oppositeCardinal2 = GridCalculator.GetOppositeDirection(cardinal2);

                        if (diag1 == null || diag2 == null)
                            return true;

                        bool b1 = (diag1 == null);
                        bool b2 = (diag2 == null);
                        if (diag1 != null)
                        {
                            b1 |= diag1.IsWall;
                            b1 |= cell2.IsWall;
                            b1 |= (_level.IsCellOccupiedByCharacter(diag1.Location) && includeBlockedByCharacters);
                        }
                        if (diag2 != null)
                        {
                            b1 |= diag2.IsWall;
                            b1 |= cell2.IsWall;
                            b2 |= (_level.IsCellOccupiedByCharacter(diag2.Location) && includeBlockedByCharacters);
                        }

                        // Both paths are blocked
                        return b1 || b2;
                    }
            }
            return false;
        }

        public GridLocation GetRandomLocation(bool excludeOccupiedLocations, IEnumerable<GridLocation> otherExcludedLocations = null)
        {
            // Slower operation
            if (excludeOccupiedLocations)
            {
                var nonOccupiedLocations = _level.Grid.FullMap.GetNonOccupiedLocations();

                if (otherExcludedLocations != null)
                    nonOccupiedLocations = nonOccupiedLocations.Except(otherExcludedLocations);

                // Return random cell
                return _randomSequenceGenerator.GetRandomElement(nonOccupiedLocations);
            }
            // O(1)
            else
                return _level.Grid.GetNonOccupiedLocation(LayoutGrid.LayoutLayer.Walkable, _randomSequenceGenerator, otherExcludedLocations ?? new GridLocation[] { });
        }

        public GridLocation GetRandomAdjacentLocationForMovement(GridLocation location, CharacterAlignmentType swappableAlignmentType = CharacterAlignmentType.None)
        {
            var adjacentLocations = _level.
                                    Grid.
                                    GetAdjacentLocations(location).
                                    Where(x => _level.Grid.ImpassableTerrainMap[x.Column, x.Row] == null).
                                    Where(x =>
                                    {
                                        var character = _level.Content.GetAt<NonPlayerCharacter>(x);

                                        // No Character (OR) Alignment Type Matches
                                        return character == null ? true : character.AlignmentType == swappableAlignmentType;
                                    });

            return adjacentLocations.Any() ? adjacentLocations.ElementAt(_randomSequenceGenerator.Get(0, adjacentLocations.Count()))
                                           : null;

        }

        public IEnumerable<GridLocation> GetFreeAdjacentLocations(GridLocation location)
        {
            // Get all adjacent locations
            //
            var adjacentLocations = _level.Grid.GetAdjacentLocations(location);

            // Checks for any level content in the cells returned
            //
            return adjacentLocations.Where(x => !_level.Grid[x.Column, x.Row].IsWall &&
                                                !_level.Grid[x.Column, x.Row].IsDoor &&
                                                _level.Grid.ImpassableTerrainMap[x.Column, x.Row] == null &&
                                                !_level.IsCellOccupied(x));
        }

        public IEnumerable<GridLocation> GetFreeAdjacentLocationsForMovement(GridLocation location, CharacterAlignmentType swappableAlignmentType = CharacterAlignmentType.None)
        {
            return _level.Grid
                         .GetAdjacentLocations(location)
                         .Where(x => _level.Grid.ImpassableTerrainMap[x.Column, x.Row] == null)
                         .Where(x =>
                          {
                              var character = _level.Content.GetAt<NonPlayerCharacter>(x);

                              // No Character (OR) Alignment Type Matches (AND) Not Wall (AND) Not Door
                              return (character == null ? true : character.AlignmentType == swappableAlignmentType) &&
                                     !_level.Grid[x.Column, x.Row].IsWall;
                          });
        }

        public IEnumerable<GridLocation> GetLocationsInRange(GridLocation location, int cellRange, bool includeSourceLocation)
        {
            // Calculate locations within a cell-range using a "pseudo-euclidean" measure to make
            // an elliptical shape. (not roguian - which would make a rectangular shape)

            // 0) Start by calculating the "square" around the location
            // 1) Narrow the result by calculating the euclidean norm of the cell location differences

            var result = new List<GridLocation>();

            // Iterate from the top left corner to the bottom right - respecting grid boundaries
            for (int i = System.Math.Max(location.Column - cellRange, 0);
                    (i <= location.Column + cellRange) &&
                    (i < _level.Grid.Bounds.Right);
                    i++)
            {
                for (int j = System.Math.Max(location.Row - cellRange, 0);
                        (j <= location.Row + cellRange) &&
                        (j < _level.Grid.Bounds.Bottom);
                        j++)
                {
                    // Check for an empty space
                    if (_level.Grid[i, j] == null)
                        continue;

                    // Check for source location
                    if (_level.Grid[i, j].Location.Equals(location) &&
                       !includeSourceLocation)
                        continue;

                    // Check the range
                    if (Metric.RoguianDistance(_level.Grid[i, j].Location, location) <= cellRange)
                        result.Add(_level.Grid[i, j].Location);
                }
            }

            return result;
        }

        public GridCell GetOffDiagonalCell1(GridLocation location, Compass direction, out Compass cardinalDirection1)
        {
            return _level.Grid.GetOffDiagonalCell1(location, direction, out cardinalDirection1);
        }

        public GridCell GetOffDiagonalCell2(GridLocation location, Compass direction, out Compass cardinalDirection2)
        {
            return _level.Grid.GetOffDiagonalCell2(location, direction, out cardinalDirection2);
        }

        public IEnumerable<GridLocation> GetAdjacentLocations(GridLocation location)
        {
            return _level.Grid.GetAdjacentLocations(location);
        }

        public IEnumerable<GridLocation> GetCardinalAdjacentLocations(GridLocation location)
        {
            return _level.Grid.GetCardinarlAdjacentLocations(location);
        }

        public IEnumerable<GridCell> GetAdjacentCells(GridCell cell)
        {
            return _level.Grid.GetAdjacentCells(cell);
        }

        public Compass GetDirectionOfAdjacentLocation(GridLocation location, GridLocation adjacentLocation)
        {
            return GridCalculator.GetDirectionOfAdjacentLocation(location, adjacentLocation);
        }

        public GridLocation GetPointInDirection(GridLocation location, Compass direction)
        {
            switch (direction)
            {
                case Compass.N: return _level.Grid[location.Column, location.Row - 1]?.Location ?? null;
                case Compass.S: return _level.Grid[location.Column, location.Row + 1]?.Location ?? null;
                case Compass.E: return _level.Grid[location.Column + 1, location.Row]?.Location ?? null;
                case Compass.W: return _level.Grid[location.Column - 1, location.Row]?.Location ?? null;
                case Compass.NE: return _level.Grid[location.Column + 1, location.Row - 1]?.Location ?? null;
                case Compass.NW: return _level.Grid[location.Column - 1, location.Row - 1]?.Location ?? null;
                case Compass.SW: return _level.Grid[location.Column - 1, location.Row + 1]?.Location ?? null;
                case Compass.SE: return _level.Grid[location.Column + 1, location.Row + 1]?.Location ?? null;
                case Compass.Null:
                default:
                    return location;
            }
        }
    }
}
