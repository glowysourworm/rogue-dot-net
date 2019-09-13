using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using System.Collections.Generic;

namespace Rogue.NET.Core.Processing.Model.Content.Interface
{
    public interface ILayoutEngine 
    {
        void Search(GridLocation location);
        void ToggleDoor(Compass direction, GridLocation characterLocation);

        /// <summary>
        /// Checks to see how a character can deal with a path that is through a door. It's assumed that
        /// if the method returns true - there's a way to move into position to open the door. The out parameters
        /// should tell you what to do.
        /// </summary>
        /// <param name="location1">Starting Location</param>
        /// <param name="openingDirection1">First way to open the door - specified direction</param>
        /// <param name="openingPosition1">Second way to open the door (other side)</param>
        /// <param name="openingPosition2">Second position to open the door from</param>
        /// <param name="openingDirection2">Second direction to open the door from</param>
        /// <param name="shouldMoveToOpeningPosition2">This tells you that you're on the "2nd" side of the door - so move to position 2.</param>
        /// <returns></returns>
        bool IsPathToCellThroughDoor(GridLocation location1, 
                                     Compass openingDirection1, 
                                     out GridLocation openingPosition1, 
                                     out GridLocation openingPosition2, 
                                     out Compass openingDirection2, 
                                     out bool shouldMoveToOpeningPosition2);

        bool IsPathToCellThroughWall(GridLocation point1, 
                                     GridLocation point2, 
                                     bool includeBlockedByAnyCharacter,
                                     CharacterAlignmentType excludedAlignmentType = CharacterAlignmentType.None);

        bool IsPathToAdjacentCellBlocked(GridLocation point1, 
                                         GridLocation point2, 
                                         bool includeBlockedByAnyCharacter,
                                         CharacterAlignmentType excludedAlignmentType = CharacterAlignmentType.None);

        /// <summary>
        /// Will exclude alignment type for blocking movement if provided
        /// </summary>
        GridLocation GetRandomAdjacentLocationForMovement(GridLocation location, CharacterAlignmentType swappableAlignmentType = CharacterAlignmentType.None);

        /// <summary>
        /// Returns adjacent cells that do not have ANY level content in them.
        /// </summary>
        IEnumerable<GridLocation> GetFreeAdjacentLocations(GridLocation location);

        /// <summary>
        /// Will exclude alignment type for blocking movement if provided
        /// </summary>
        IEnumerable<GridLocation> GetFreeAdjacentLocationsForMovement(GridLocation location, CharacterAlignmentType swappableAlignmentType = CharacterAlignmentType.None);

        /// <summary>
        /// Returns locations in range (using "Roguian" metric)
        /// </summary>
        /// <param name="location">Source location</param>
        /// <param name="cellRange">Number of cells outward to search (1 => adjacent cells)</param>
        /// <param name="includeSourceLocation">Can include source in the results</param>
        IEnumerable<GridLocation> GetLocationsInRange(GridLocation location, int cellRange, bool includeSourceLocation);
    }
}
