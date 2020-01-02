using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Math.Geometry;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.Scenario.Content.Layout;

using System.Collections.Generic;
using System.Linq;

namespace Rogue.NET.Core.Model.Scenario.Dynamic.Layout
{
    /// <summary>
    /// Component that keeps track of affected locations for auras
    /// </summary>
    public class AuraGrid
    {
        readonly CharacterVisibility _visibilityGrid;

        // Primary 2D array - stores aura identifier
        List<string>[,] _playerAlignedAuraGrid;
        List<string>[,] _enemyAlignedAuraGrid;

        // Dictionary lookup - stores affected locations per aura
        Dictionary<string, IEnumerable<GridLocation>> _auraDict;

        public AuraGrid(CharacterVisibility visibilityGrid)
        {
            _visibilityGrid = visibilityGrid;

            // Initialize private collections
            _playerAlignedAuraGrid = new List<string>[visibilityGrid.Boundary.Width, visibilityGrid.Boundary.Height];
            _enemyAlignedAuraGrid = new List<string>[visibilityGrid.Boundary.Width, visibilityGrid.Boundary.Height];
            _auraDict = new Dictionary<string, IEnumerable<GridLocation>>();
        }

        public void Update(CharacterBase character, GridLocation characterLocation)
        {
            var playerAligned = (character is Player) ||
                                (character as NonPlayerCharacter).AlignmentType == CharacterAlignmentType.PlayerAligned;

            // Update all auras for this character
            foreach (var aura in character.Alteration.GetAuras())
            {
                var auraLocations = _visibilityGrid[character].VisibleLocations
                                                              .Where(location => Metric.EuclideanDistance(characterLocation, location) <= aura.Item2.AuraRange)
                                                              .Actualize();

                UpdateAura(aura.Item1.Id, auraLocations, playerAligned ? CharacterAlignmentType.PlayerAligned : CharacterAlignmentType.EnemyAligned);
            }
        }

        private void UpdateAura(string auraId, IEnumerable<GridLocation> locations, CharacterAlignmentType alignment)
        {
            // Remove old aura locations
            if (_auraDict.ContainsKey(auraId))
            {
                var oldLocations = _auraDict[auraId];

                // Remove aura from old grid
                foreach (var location in oldLocations)
                {
                    if (_enemyAlignedAuraGrid[location.Column, location.Row].Contains(auraId))
                        _enemyAlignedAuraGrid[location.Column, location.Row].Remove(auraId);

                    if (_playerAlignedAuraGrid[location.Column, location.Row].Contains(auraId))
                        _playerAlignedAuraGrid[location.Column, location.Row].Remove(auraId);
                }

                // Remove dictionary entry
                _auraDict.Remove(auraId);
            }

            // Add new aura locations
            _auraDict.Add(auraId, locations);

            foreach (var location in locations)
            {
                if (alignment == CharacterAlignmentType.EnemyAligned)
                    _enemyAlignedAuraGrid[location.Column, location.Row].Add(auraId);

                else
                    _playerAlignedAuraGrid[location.Column, location.Row].Add(auraId);
            }
        }

        public IEnumerable<string> GetPlayerAlignedAuraIds(GridLocation location)
        {
            return _playerAlignedAuraGrid[location.Column, location.Row];
        }

        public IEnumerable<string> GetEnemyAlignedAuraIds(GridLocation location)
        {
            return _enemyAlignedAuraGrid[location.Column, location.Row];
        }

        public IEnumerable<GridLocation> GetAuraAffectedLocations(string alterationEffectId)
        {
            return _auraDict[alterationEffectId];
        }
    }
}
