using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Processing.Model.Content.Enum;

namespace Rogue.NET.Core.Processing.Service.Interface
{
    /// <summary>
    /// Service for storing state for targeting actions
    /// </summary>
    public interface ITargetingService
    {
        /// <summary>
        /// Starts tracking target selected by user with the location provided
        /// </summary>
        void StartTargeting(GridLocation location);

        /// <summary>
        /// Moves target tracker in the specified direction. Returns true if new target location is valid
        /// </summary>
        bool MoveTarget(Compass direction);

        /// <summary>
        /// Cycles character targets in visible range
        /// </summary>
        void CycleTarget(Compass direction);

        /// <summary>
        /// Stops tracking target and captures selected location or character
        /// </summary>
        void EndTargeting();

        /// <summary>
        /// Gets the type of target the user selected
        /// </summary>
        TargetType GetTargetType();

        /// <summary>
        /// Returns targeted grid location
        /// </summary>
        GridLocation GetTargetLocation();

        /// <summary>
        /// Returns targeted character
        /// </summary>
        Character GetTargetedCharacter();

        /// <summary>
        /// Clears out target information
        /// </summary>
        void Clear();
    }
}
