using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Math.Geometry;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Processing.Model.Algorithm;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Rogue.NET.Core.Model.Scenario.Dynamic.Layout
{
    /// <summary>
    /// Component for planning character movement between search and rest periods
    /// </summary>
    public class CharacterMovementPlanner
    {
        // Primary layout grid
        readonly LayoutGrid _layoutGrid;

        // Rest location for character rest periods
        readonly GridLocation _restLocation;
        readonly int _restPeriod;
        readonly int _searchRadius;

        // NOTE*** REFERENCES SET BY THE PRIMARY LAYOUT GRID. NO ADDITIONAL MEMORY ALLOCATION
        //
        Dictionary<GridLocation, GridLocation> _visibleLocations;

        // Collection of regions that have been fully searched
        List<ConnectedRegion<GridLocation>> _searchedRegions;

        // Mark regions within search radius - BEFORE EACH NEW SWEEP
        List<ConnectedRegion<GridLocation>> _searchableRegions;

        // Search grid for the current region
        SearchGrid<GridLocation> _searchGrid;

        // Route to plan movement between two regions
        ConnectedRegion<GridLocation> _nextRegion;

        // Current rest clock - keeps track of rest period
        int _restClock;

        /// <summary>
        /// Constructs a movement planner for characters that rest at the location where they finish their search
        /// sweep.
        /// </summary>
        public CharacterMovementPlanner(LayoutGrid layoutGrid, int searchRadius, int restPeriod)
        {
            _layoutGrid = layoutGrid;

            _restPeriod = restPeriod;
            _searchRadius = searchRadius;

            _searchedRegions = new List<ConnectedRegion<GridLocation>>();
            _searchableRegions = new List<ConnectedRegion<GridLocation>>();
            _visibleLocations = new Dictionary<GridLocation, GridLocation>();
        }

        /// <summary>
        /// Constructs a movement planner for characters that rest at the specified location
        /// </summary>
        public CharacterMovementPlanner(LayoutGrid layoutGrid, GridLocation restLocation, int searchRadius, int restPeriod)
        {
            _layoutGrid = layoutGrid;

            _restLocation = restLocation;
            _restPeriod = restPeriod;
            _searchRadius = searchRadius;

            _searchedRegions = new List<ConnectedRegion<GridLocation>>();
            _searchableRegions = new List<ConnectedRegion<GridLocation>>();
            _visibleLocations = new Dictionary<GridLocation, GridLocation>();
        }

        public SearchGrid<GridLocation> SearchGrid 
        { 
            get { return _searchGrid; } 
        }

        public IEnumerable<GridLocation> VisibleLocations 
        { 
            get { return _visibleLocations.Values; } 
        }

        public bool IsVisible(GridLocation location)
        {
            // ~ O(1)
            return _visibleLocations.ContainsKey(location);
        }

        public bool IsResting()
        {
            return _restClock > 0;
        }

        /// <summary>
        /// Updates character visibility and route planning
        /// </summary>
        public void Update(GridLocation location)
        {
            // Decrement rest clock
            if (_restClock > 0)
                _restClock--;

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
            if (_restClock > 0)
                throw new Exception("Character is currently resting - must check before calling CharacterMovementPlanner.GetNextSearchLocation");

            // INITIALIZE SEARCH SWEEP - SET SEARCHABLE REGIONS
            if (_searchGrid == null &&
                _nextRegion == null)
                _searchableRegions = _layoutGrid.ConnectionMap
                                                .Regions
                                                .Cast<ConnectedRegion<GridLocation>>()
                                                .Where(region =>
                {
                    return region.Locations
                                 .Any(location => Metric.EuclideanDistance(location, currentLocation) <= _searchRadius);

                }).ToList();

            // Search is fully exhausted -> character heads to rest location -> set rest clock
            if (_searchedRegions.Count == _searchableRegions.Count)
            {
                // Character is at the rest location
                if (_restLocation == null ||
                    _restLocation.Equals(currentLocation))
                {
                    // SET THE REST CLOCK
                    _restClock = _restPeriod;

                    // Clear out search history
                    _searchedRegions.Clear();

                    // Rest at this location for the rest period
                    return currentLocation;
                }

                else
                    return _restLocation;
            }

            // Search not begun
            else if (_searchGrid == null)
            {
                // Case 1: Character's FIRST search sweep
                // Case 2: Character en-route to new region

                // NOTE*** Connection map isn't actually connected. It just STORES region
                //         connections (like a graph) that HAVE been connected by the layout builder.
                //
                var currentRegion = _layoutGrid.ConnectionMap[currentLocation];

                // Case 1 (VERY FIRST SWEEP ONLY)
                if (_nextRegion == null)
                    _nextRegion = GetNextRegion(currentLocation);

                // Case 2 (OR) Case 1 -> Case 2
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

                // CHARACTER HAS ARRIVED! BEGIN A NEW SWEEP
                else
                {
                    // Begin next search sweep
                    _searchGrid = new SearchGrid<GridLocation>(_nextRegion, _restLocation ?? currentLocation, _searchRadius);

                    // Nullify region reference to signal that character has begun a sweep
                    _nextRegion = null;

                    // Initialize the new visibility
                    Update(currentLocation);

                    // Recursively return next search location - in case there are no locations here to search
                    return GetNextSearchLocation(currentLocation);
                }
            }

            // Search just completed -> Mark the region and recurse
            else if (_searchGrid.IsFullySearched())
            {
                // Mark the current region as searched
                _searchedRegions.Add(_searchGrid.Region);

                // Calculate the next region
                if (_searchedRegions.Count == _layoutGrid.ConnectionMap.Regions.Count())
                    _nextRegion = null;

                else
                    _nextRegion = GetNextRegion(currentLocation);

                // NULLIFY THE SEARCH GRID
                _searchGrid = null;

                return GetNextSearchLocation(currentLocation);
            }

            // Continue search...
            else
                return _searchGrid.GetNextSearchLocation();
        }

        private ConnectedRegion<GridLocation> GetNextRegion(GridLocation currentLocation)
        {
            // CHECK FOR ONLY A SINGLE REGION IN THE LAYER
            if (_layoutGrid.ConnectionMap.Regions.Count() == 1)
                return (ConnectedRegion<GridLocation>)_layoutGrid.ConnectionMap.Regions.First();

            // Plan route to next region
            else
            {
                // Get the current region - MAY NOT BE CONNECTED REGION
                var currentRegion = _layoutGrid.ConnectionMap[currentLocation];

                // Calculate the next region using Breadth first search from the current region
                if (currentRegion != null)
                    return TraversalAlgorithm.FindNextSearchRegion(_layoutGrid.ConnectionMap, 
                                                                   _searchableRegions, 
                                                                   _searchedRegions, 
                                                                   currentRegion as ConnectedRegion<GridLocation>);

                // Starting search from the closest non-occupied connected region
                else
                {
                    // Query for the closest connection region location
                    var connectedLocation = _layoutGrid.GetClosestNonOccupiedLocation(LayoutGrid.LayoutLayer.Connection, currentLocation);

                    // Set the next region from this location
                    return (ConnectedRegion<GridLocation>)_layoutGrid.ConnectionMap[connectedLocation];
                }
            }
        }
    }
}
