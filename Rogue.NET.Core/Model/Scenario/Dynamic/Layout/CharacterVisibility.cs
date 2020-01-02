using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.Scenario.Content;
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
    /// Exmaple: Explored, Revealed, and Visible locations. Also, contians character route planning
    /// information for exploring the map.
    /// </summary>
    public class CharacterVisibility
    {
        readonly LayoutGrid _layoutGrid;
        readonly ContentGrid _contentGrid;

        // Character visibility calculations PER REGION (RE-CREATED EACH TIME CHARACTER ENTERS NEW REGION)
        Dictionary<CharacterBase, SearchGrid<GridLocation>> _searchDict;

        // Player - Content Visibility
        Dictionary<ScenarioObject, ScenarioObject> _contentVisibilityDict;

        // Primary visibility information for the Player
        Dictionary<GridLocation, GridLocation> _visibleLocations;
        Dictionary<GridLocation, GridLocation> _exploredLocations;
        Dictionary<GridLocation, GridLocation> _revealedLocations;

        // Expose the primary grid boundary
        public RegionBoundary Boundary { get { return _layoutGrid.Bounds; } }

        /// <summary>
        /// Returns the search grid for the specified character. This grid contains visibility information
        /// for the character's current region
        /// </summary>
        public SearchGrid<GridLocation> this[CharacterBase character]
        {
            get { return _searchDict[character]; }
        }

        /// <summary>
        /// Constructor for the CharacterLayoutInformation should be called once per
        /// level and updated on each turn.
        /// </summary>
        public CharacterVisibility(LayoutGrid layoutGrid, ContentGrid contentGrid)
        {
            _layoutGrid = layoutGrid;
            _contentGrid = contentGrid;

            _searchDict = new Dictionary<CharacterBase, SearchGrid<GridLocation>>();
            _contentVisibilityDict = new Dictionary<ScenarioObject, ScenarioObject>();

            _visibleLocations = new Dictionary<GridLocation, GridLocation>();
            _exploredLocations = new Dictionary<GridLocation, GridLocation>();
            _revealedLocations = new Dictionary<GridLocation, GridLocation>();

            // Initialize Explored / Revealed locations
            _exploredLocations = _layoutGrid.FullMap
                                            .GetLocations()
                                            .Where(x => _layoutGrid[x].IsExplored)
                                            .ToDictionary(x => x, x => x);

            _revealedLocations = _layoutGrid.FullMap
                                            .GetLocations()
                                            .Where(x => _layoutGrid[x].IsRevealed)
                                            .ToDictionary(x => x, x => x);
        }

        public void Update(Player player, GridLocation playerLocation)
        {
            // Player - Calculate Revealed / Explored locations
            //

            // Clear current visibility
            _visibleLocations.Clear();
            _contentVisibilityDict.Clear();

            // Calculate visible locations
            VisibilityCalculator.CalculateVisibility(_layoutGrid, playerLocation, (column, row, isVisible) =>
            {
                // Visible Cells -> Explored / No Longer Revealed
                if (isVisible)
                {
                    // Set properties from the primary
                    var cell = _layoutGrid[column, row];

                    cell.IsExplored = true;
                    cell.IsRevealed = false;

                    // Update player collections (~ O(1) each call)
                    //
                    // Visible
                    if (!_visibleLocations.ContainsKey(cell.Location))
                        _visibleLocations.Add(cell.Location, cell.Location);

                    // Explored
                    if (!_exploredLocations.ContainsKey(cell.Location))
                        _exploredLocations.Add(cell.Location, cell.Location);

                    // Revealed
                    if (_revealedLocations.ContainsKey(cell.Location))
                        _revealedLocations.Remove(cell.Location);

                    // Set content visibility
                    foreach (var content in _contentGrid[column, row])
                    {
                        if (!_contentVisibilityDict.ContainsKey(content))
                            _contentVisibilityDict.Add(content, content);
                    }
                }
            });

            // TODO: REMOVE THIS 
            _revealedLocations = _layoutGrid.FullMap
                                            .GetLocations()
                                            .Where(location => _layoutGrid[location].IsRevealed)
                                            .ToDictionary(x => x, x => x);
        }

        public void Update(NonPlayerCharacter nonPlayerCharacter, GridLocation nonPlayerCharacterLocation)
        {
            // Check to see if the current entry is for the current region
            //
            // 1) Check first for a connected region to use for route planning
            // 2) If that doesn't work, fall back to the walkable layer
            var connectedRegion = _layoutGrid.ConnectionMap[nonPlayerCharacterLocation];
            var walkableRegion = _layoutGrid.WalkableMap[nonPlayerCharacterLocation];

            var currentRegion = connectedRegion ?? walkableRegion;

            // Add a new entry for this character
            if (!_searchDict.ContainsKey(nonPlayerCharacter))
                _searchDict.Add(nonPlayerCharacter, new SearchGrid<GridLocation>(currentRegion));

            else
            {
                // Have to re-initialize the search grid
                if (_searchDict[nonPlayerCharacter].Region != currentRegion)
                    _searchDict[nonPlayerCharacter] = new SearchGrid<GridLocation>(currentRegion);
            }

            // Fetch the current search grid
            var searchGrid = _searchDict[nonPlayerCharacter];

            // Clear visibility
            searchGrid.ClearVisible();

            // Calculate visible locations
            VisibilityCalculator.CalculateVisibility(_layoutGrid, nonPlayerCharacterLocation, (column, row, isVisible) =>
            {
                if (isVisible)
                {
                    // Fetch the location from the primary layout grid
                    var cell = _layoutGrid[column, row];

                    // Set visibility for this location
                    searchGrid.SetVisible(cell.Location);
                }
            });
        }

        public bool IsVisibleTo(IGridLocator location, CharacterBase character)
        {
            // ~ O(1)
            return _searchDict[character].IsVisible(_layoutGrid[location].Location);
        }

        public bool IsVisible(ScenarioObject scenarioObject)
        {
            // ~ O(1)
            return _contentVisibilityDict.ContainsKey(scenarioObject);
        }

        public IEnumerable<GridLocation> GetVisibleLocations()
        {
            return _visibleLocations.Values;
        }

        public IEnumerable<GridLocation> GetExploredLocations()
        {
            return _exploredLocations.Values;
        }

        public IEnumerable<GridLocation> GetRevealedLocations()
        {
            return _revealedLocations.Values;
        }
    }
}
