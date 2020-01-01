using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.Scenario.Content.Layout;

using System.Collections.Generic;

namespace Rogue.NET.Core.Model.Scenario.Dynamic.Layout.Interface
{
    /// <summary>
    /// Component that is created once per level and dynamically calculates light radius, 
    /// line-of-sight, and aura information per character
    /// </summary>
    public interface ICharacterLayoutInformation
    {
        /// <summary>
        /// Applies blanket update for the layout information - creating new data and clearing out
        /// old character data. Also, applies update to LevelGrid cells for IsExplored / IsRevealed
        /// because the IRayTracer calculation is run here. 
        /// 
        /// (TODO: Optimize by doing smaller changes based on a single character movement)
        /// </summary>
        void ApplyUpdate(IEnumerable<CharacterBase> characters);

        /// <summary>
        /// Calculates paths for all Non-Player Characters and stores the results.
        /// </summary>
        void CalculateCharacterPaths();

        /// <summary>
        /// Gets next location along path to nearest target for the specified character
        /// </summary>
        GridLocation GetNextPathLocation(CharacterBase character);

        IEnumerable<GridLocation> GetLineOfSightLocations(CharacterBase character);
        IEnumerable<GridLocation> GetVisibleLocations(CharacterBase character);
        IEnumerable<GridLocation> GetAuraAffectedLocations(CharacterBase character, string alterationEffectId);

        /// <summary>
        /// Explored locations calculation done during other calculations to improve performance
        /// </summary>
        IEnumerable<GridLocation> GetExploredLocations();

        /// <summary>
        /// Revealed locations calculation done during other calculations to improve performance
        /// </summary>
        IEnumerable<GridLocation> GetRevealedLocations();
    }
}
