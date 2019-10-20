using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using System.Collections.Generic;

namespace Rogue.NET.Core.Processing.Model.Algorithm.Interface
{
    /// <summary>
    /// Component to calculate path finding algorithm
    /// </summary>
    public interface IPathFinder
    {
        /// <summary>
        /// Returns next point towards point2 taking into account character movement. If limits are reached algorithm returns GridLocation.Empty
        /// </summary>
        GridLocation FindCharacterNextPathLocation(GridLocation point1, GridLocation point2, bool canOpenDoors, CharacterAlignmentType alignmentType);

        /// <summary>
        /// Returns next point towards point2. If limits are reached algorithm returns GridLocation.Empty
        /// </summary>
        GridLocation FindNextPathLocation(GridLocation point1, GridLocation point2);

        /// <summary>
        /// Returns all locations along path from point1 to point2 taking into account character movement. Returns empty collection for failed attempt.
        /// </summary>
        IEnumerable<GridLocation> FindCharacterPath(GridLocation point1, GridLocation point2, bool canOpenDoors, CharacterAlignmentType alignmentType);

        /// <summary>
        /// Returns all locations along path from point1 to point2 taking into account character movement. Returns empty collection for failed attempt.
        /// </summary>
        IEnumerable<GridLocation> FindPath(GridLocation point1, GridLocation point2);
    }
}
