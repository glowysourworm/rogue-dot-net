using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Math.Geometry;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Processing.Model.Algorithm;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Rogue.NET.Core.Model.Scenario.Dynamic.Layout
{
    public class CharacterMovementPlanner
    {
        // Primary layout grid
        readonly LayoutGrid _layoutGrid;

        // NOTE*** REFERENCES SET BY THE PRIMARY LAYOUT GRID. NO ADDITIONAL MEMORY ALLOCATION
        //
        Dictionary<GridLocation, GridLocation> _visibleLocations;

        // Collection of regions that have been fully searched
        List<ConnectedRegion<GridLocation>> _searchedRegions;

        // Search grid for the current region
        SearchGrid<GridLocation> _searchGrid;

        // Route to plan movement between two regions
        ConnectedRegion<GridLocation> _nextRegion;

        public SearchGrid<GridLocation> SearchGrid { get { return _searchGrid; } }

        public IEnumerable<GridLocation> VisibleLocations { get { return _visibleLocations.Values; } }

        public CharacterMovementPlanner(LayoutGrid layoutGrid)
        {
            _layoutGrid = layoutGrid;

            _searchedRegions = new List<ConnectedRegion<GridLocation>>();
            _visibleLocations = new Dictionary<GridLocation, GridLocation>();
        }

        public bool IsVisible(GridLocation location)
        {
            // ~ O(1)
            return _visibleLocations.ContainsKey(location);
        }

        /// <summary>
        /// Updates character visibility and route planning
        /// </summary>
        public void UpdateVisibility(GridLocation location)
        {
            _visibleLocations.Clear();

            // Calculate visible locations -> MUST PREVENT NON-WALKABLE LOCATIONS FROM BEING SET ON THE SEARCH GRID
            VisibilityCalculator.CalculateVisibility(_layoutGrid, location, (column, row, isVisible) =>
            {
                if (isVisible && _layoutGrid.WalkableMap[column, row] != null)
                {
                    // Fetch the location from the primary layout grid
                    var cell = _layoutGrid[column, row];

                    // Maintain visibility hash
                    if (!_visibleLocations.ContainsKey(cell.Location))
                         _visibleLocations.Add(cell.Location, cell.Location);

                    // Set visibility for this location if:
                    //
                    // 1) Search sweep is active
                    // 2) Connection region exists for this location
                    // 3) Search region is equal to the connection region
                    //
                    if (_searchGrid != null &&
                        _layoutGrid.ConnectionMap[column, row] != null &&
                        _searchGrid.Region == _layoutGrid.ConnectionMap[column, row])
                        _searchGrid.SetSearched(cell.Location);
                }
            });

            // MUST ADD CHARACTER'S POSITION TO THE SEARCH GRID VISIBILITY
            //
            if (!_visibleLocations.ContainsKey(location))
                 _visibleLocations.Add(location, location);

            if (_searchGrid != null &&
                _layoutGrid.ConnectionMap[location] != null &&
                _searchGrid.Region == _layoutGrid.ConnectionMap[location])
            {                
                _searchGrid.SetSearched(location);
            }
        }

        public GridLocation GetNextSearchLocation(GridLocation currentLocation)
        {
            // Case 0:  Character is started in non-connected region - SearchGrid is not instantiated
            // Case 1:  Character is currently searching out a region - returns the next location
            // Case 2:  Character's current search region is fully searched
            //          - Multiple Regions: begin route to next region
            //          - Single Region: Reset search grid and start over
            // Case 3:  Character is en-route to next region
            // Case 4:  Character has arrived at next region - begin search sweep
            //

            // Case 3 | Case 4  (Character is en-route or has arrived at next region)
            if (_nextRegion != null)
            {
                // NOTE*** Connection map isn't actually connected. It just STORES region
                //         connections (like a graph) that HAVE been connected by the layout builder.
                //
                var currentRegion = _layoutGrid.ConnectionMap[currentLocation];

                // Case 3
                if (_nextRegion != currentRegion)
                {
                    // NOTE*** Just need to get the character to the region to let the search grid take
                    //         over and begin a sweep.
                    //
                    //         So, going to use the closest vertex to the current location as a destination
                    //
                    var vertex = _layoutGrid.ConnectionMap
                                             .ConnectionGraph
                                             .Find(_nextRegion.Id)
                                             .MinBy(vertex => Metric.EuclideanDistance(vertex, currentLocation));

                    return _layoutGrid[vertex].Location;
                }

                // Case 4
                else
                {
                    // Begin next search sweep
                    _searchGrid = new SearchGrid<GridLocation>(_nextRegion);

                    // Nullify region reference to signal that character has begun a sweep
                    _nextRegion = null;

                    // Initialize the new visibility
                    UpdateVisibility(currentLocation);

                    // Recursively return next search location - in case there are no locations here to search
                    return GetNextSearchLocation(currentLocation);
                }
            }

            // Case 1
            if (_searchGrid != null &&
               !_searchGrid.IsFullySearched())
                return _searchGrid.GetNextSearchLocation();

            // Case 2 - Begin
            else
            {
                // CHECK FOR ONLY A SINGLE REGION IN THE LAYER
                if (_layoutGrid.ConnectionMap.Regions.Count() == 1)
                {
                    // Case 0
                    if (_searchGrid == null)
                    {
                        _searchGrid = new SearchGrid<GridLocation>((ConnectedRegion<GridLocation>)_layoutGrid.ConnectionMap.Regions.First());

                        // Initialize the new visibility
                        UpdateVisibility(currentLocation);
                    }

                    // Just clear the search and start over
                    _searchGrid.Clear();

                    // Return the first search location
                    return _searchGrid.GetNextSearchLocation();
                }

                // Plan route to next region
                else
                {
                    // Calculate unsearched regions - excluding the one that was just completed
                    var unSearchedRegions = _layoutGrid.ConnectionMap
                                                       .Regions
                                                       .Except(_searchedRegions);

                    // Remove search that was just completed
                    if (_searchGrid != null)
                        unSearchedRegions = unSearchedRegions.Except(new ConnectedRegion<GridLocation>[] { _searchGrid.Region });

                    // Reset search and start over from the current region
                    if (!unSearchedRegions.Any())
                        _searchedRegions.Clear();

                    // Current region is finished - so plan a route to the next region
                    if (_searchGrid != null)
                        _searchedRegions.Add(_searchGrid.Region);

                    // Calculate the next region using Breadth first search
                    if (_searchGrid != null)
                        _nextRegion = TraversalAlgorithm.FindNextSearchRegion(_layoutGrid.ConnectionMap, _searchedRegions, _searchGrid.Region);

                    // Case 0
                    else
                    {
                        // Query for the closest connection region location
                        var connectedLocation = _layoutGrid.GetClosestNonOccupiedLocation(LayoutGrid.LayoutLayer.Connection, currentLocation);

                        // Set the next region from this location
                        _nextRegion = (ConnectedRegion<GridLocation>)_layoutGrid.ConnectionMap[connectedLocation];
                    }

                    if (_nextRegion == null)
                        throw new Exception("Un-connected region found in the LayoutGrid connection layer map - SearchPlanner.GetNextSearchLocation");

                    // Recursively return next search location using new _nextRegion reference
                    return GetNextSearchLocation(currentLocation);
                }
            }
        }
    }
}
