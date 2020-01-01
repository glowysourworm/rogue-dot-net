using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.Scenario.Content.Layout.Interface;
using Rogue.NET.Core.Processing.Model.Algorithm;
using Rogue.NET.Core.Processing.Model.Extension;

using System.Collections.Generic;
using System.Linq;

namespace Rogue.NET.Core.Model.Scenario.Dynamic.Layout
{
    /// <summary>
    /// Component for keeping track of visibility information that is calculated; but not stored. 
    /// Exmaple: Explored, Revealed, and Line-of-sight
    /// </summary>
    public class VisibilityGrid
    {
        readonly LayoutGrid _layoutGrid;

        // Primary visibility 2D array
        Dictionary<CharacterBase, CharacterBase>[,] _visibilityGrid;
        Dictionary<CharacterBase, CharacterBase>[,] _lineOfSightGrid;

        // Primary visibility calculations
        Dictionary<CharacterBase, IEnumerable<DistanceLocation>> _visibleDict;
        Dictionary<CharacterBase, IEnumerable<DistanceLocation>> _lineOfSightDict;

        List<GridLocation> _exploredLocations;
        List<GridLocation> _revealedLocations;

        // Expose the primary grid boundary
        public RegionBoundary Boundary { get { return _layoutGrid.Bounds; } }

        /// <summary>
        /// Constructor for the CharacterLayoutInformation should be called once per
        /// level and updated on each turn.
        /// </summary>
        public VisibilityGrid(LayoutGrid layoutGrid)
        {
            _layoutGrid = layoutGrid;

            _visibleDict = new Dictionary<CharacterBase, IEnumerable<DistanceLocation>>();
            _lineOfSightDict = new Dictionary<CharacterBase, IEnumerable<DistanceLocation>>();

            _visibilityGrid = new Dictionary<CharacterBase, CharacterBase>[layoutGrid.Bounds.Width, layoutGrid.Bounds.Height];
            _lineOfSightGrid = new Dictionary<CharacterBase, CharacterBase>[layoutGrid.Bounds.Width, layoutGrid.Bounds.Height];

            _exploredLocations = new List<GridLocation>();
            _revealedLocations = new List<GridLocation>();

            _visibilityGrid.Iterate((column, row) => _visibilityGrid[column, row] = new Dictionary<CharacterBase, CharacterBase>());
            _lineOfSightGrid.Iterate((column, row) => _lineOfSightGrid[column, row] = new Dictionary<CharacterBase, CharacterBase>());
        }

        public void Update(Player player, GridLocation playerLocation)
        {
            // Player - Calculate Revealed / Explored locations
            //
            // TODO: Consider better design because this does work on the
            //       LevelGrid and it's supposed to just be a calculation.

            // Calculate visible locations
            var visibleLocations = VisibilityCalculator.CalculateVisibility(_layoutGrid, playerLocation);

            // Visible Cells -> Explored / No Longer Revealed
            foreach (var location in visibleLocations.Select(x => x.Location))
            {
                var cell = _layoutGrid[location];

                cell.IsExplored = true;
                cell.IsRevealed = false;
            }

            // Update / Add to collections
            UpdateVisibleLocations(player, visibleLocations);

            // Update Explored / Revealed locations
            _exploredLocations = _layoutGrid.FullMap
                                            .GetLocations()
                                            .Where(x => _layoutGrid[x].IsExplored)
                                            .ToList();

            _revealedLocations = _layoutGrid.FullMap
                                            .GetLocations()
                                            .Where(x => _layoutGrid[x].IsRevealed)
                                            .ToList();
        }

        public void Update(NonPlayerCharacter nonPlayerCharacter, GridLocation nonPlayerCharacterLocation)
        {
            // Calculate visible locations
            var visibleLocations = VisibilityCalculator.CalculateVisibility(_layoutGrid, nonPlayerCharacterLocation);

            // Update / Add to collections
            UpdateVisibleLocations(nonPlayerCharacter, visibleLocations);
        }

        private void UpdateVisibleLocations(CharacterBase character, IEnumerable<DistanceLocation> visibleLocations)
        {
            // Remove old character data from the grids
            if (_visibleDict.ContainsKey(character))
            {
                var oldVisibilityLocations = _visibleDict[character];
                var oldLineOfSightLocations = _lineOfSightDict[character];

                // Update 2D arrays
                foreach (var location in oldVisibilityLocations)
                    _visibilityGrid[location.Location.Column, location.Location.Row].Remove(character);

                foreach (var location in oldLineOfSightLocations)
                    _lineOfSightGrid[location.Location.Column, location.Location.Row].Remove(character);
            }

            // TODO:TERRAIN - RE-CALCULATE VISIBLE / LINE-OF-SIGHT WITH NEW VISION PARAMETER
            if (!_visibleDict.ContainsKey(character))
                _visibleDict.Add(character, visibleLocations);

            else
                _visibleDict[character] = visibleLocations;

            // TODO:TERRAIN - RE-CALCULATE VISIBLE / LINE-OF-SIGHT WITH NEW VISION PARAMETER
            if (!_lineOfSightDict.ContainsKey(character))
                _lineOfSightDict.Add(character, visibleLocations);

            else
                _lineOfSightDict[character] = visibleLocations;

            // TODO:TERRAIN - RE-CALCULATE VISIBLE / LINE-OF-SIGHT WITH NEW VISION PARAMETER
            // Update 2D array
            foreach (var location in visibleLocations)
            {
                _visibilityGrid[location.Location.Column, location.Location.Row].Add(character, character);
                _lineOfSightGrid[location.Location.Column, location.Location.Row].Add(character, character);
            }
        }

        public bool IsVisibleTo(IGridLocator location, CharacterBase character)
        {
            return _visibilityGrid[location.Column, location.Row].Values.Contains(character);
        }

        public bool IsLineOfSight(IGridLocator location, CharacterBase character)
        {
            return _lineOfSightGrid[location.Column, location.Row].Values.Contains(character);
        }

        public IEnumerable<GridLocation> GetLineOfSightLocations(CharacterBase character)
        {
            return _lineOfSightDict[character].Select(x => x.Location).Actualize();
        }

        public IEnumerable<GridLocation> GetVisibleLocations(CharacterBase character)
        {
            return _visibleDict[character].Select(x => x.Location).Actualize();
        }

        public IEnumerable<GridLocation> GetExploredLocations()
        {
            return _exploredLocations;
        }

        public IEnumerable<GridLocation> GetRevealedLocations()
        {
            return _revealedLocations;
        }
    }
}
