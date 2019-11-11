using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Processing.Service.Interface
{
    /// <summary>
    /// Stateful service sub-component of the IModelService. This should be loaded once per level with the
    /// new level data.
    /// </summary>
    public interface IModelLayoutService
    {
        /// <summary>
        /// Calculates effective lighting value by combining light sources from the level. This should be run 
        /// every turn.
        /// </summary>
        void CalculateEffectiveLighting();

        /// <summary>
        /// Checks to see if path to adjacent cell is blocked for character movement. Can optionally specify character alignemnt type
        /// </summary>
        bool IsPathToAdjacentCellBlocked(GridLocation location1,
                                         GridLocation location2,
                                         bool includeBlockedByCharacters,
                                         CharacterAlignmentType excludedAlignmentType = CharacterAlignmentType.None);

        /// <summary>
        /// Returns random location or CellPoint.Empty. The "excludeOccupiedLocations" and the "otherExcludedLocations" options are 
        /// mutually orthogonal (they don't affect each other). The routine automatically excludes walls and doors.
        /// </summary>
        GridLocation GetRandomLocation(bool excludeOccupiedLocations, IEnumerable<GridLocation> otherExcludedLocations = null);

        /// <summary>
        /// Returns adjacent random location for character movement. Checks character alignment type to see if they can swap locations.
        /// </summary>
        GridLocation GetRandomAdjacentLocationForMovement(GridLocation location, CharacterAlignmentType swappableAlignmentType = CharacterAlignmentType.None);

        /// <summary>
        /// Returns free adjacent locations. The returned locations have no content (items, characters, doodads), and
        /// also are not walls or doors.
        /// </summary>
        IEnumerable<GridLocation> GetFreeAdjacentLocations(GridLocation location);

        /// <summary>
        /// Returns free adjacent locations for character movement with the specified alignment. This will exclude cells with 
        /// opposite alignment characters, and walls.
        /// </summary>
        IEnumerable<GridLocation> GetFreeAdjacentLocationsForMovement(GridLocation location, CharacterAlignmentType swappableAlignmentType = CharacterAlignmentType.None);

        /// <summary>
        /// Returns all locations in range of the specified locations
        /// </summary>
        IEnumerable<GridLocation> GetLocationsInRange(GridLocation location, int cellRange, bool includeSourceLocation);

        /// <summary>
        /// Returns 1st of 2 off diagonal cells in the specified non-cardinal direction (Example: NE -> N cell)
        /// </summary>
        /// <param name="direction">NE, NW, SE, SW</param>
        GridCell GetOffDiagonalCell1(GridLocation location, Compass direction, out Compass cardinalDirection1);

        /// <summary>
        /// Returns 2nd of 2 off diagonal cells in the specified non-cardinal direction (Example: NE -> E cell)
        /// </summary>
        /// <param name="direction">NE, NW, SE, SW</param>
        GridCell GetOffDiagonalCell2(GridLocation location, Compass direction, out Compass cardinalDirection2);

        /// <summary>
        /// Returns 8-way adjacent locations in the level that are cells
        /// </summary>
        IEnumerable<GridLocation> GetAdjacentLocations(GridLocation location);

        /// <summary>
        /// Returns 4-way adjacent locations in the level that are cells
        /// </summary>
        IEnumerable<GridLocation> GetCardinalAdjacentLocations(GridLocation location);

        /// <summary>
        /// Returns 8-way adjacent cells in the level (with null checks)
        /// </summary>
        IEnumerable<GridCell> GetAdjacentCells(GridCell cell);

        /// <summary>
        /// Returns direction of the adjacent location
        /// </summary>
        Compass GetDirectionOfAdjacentLocation(GridLocation location, GridLocation adjacentLocation);

        /// <summary>
        /// Returns point in the specified direction
        /// </summary>
        GridLocation GetPointInDirection(GridLocation location, Compass direction);
    }
}
