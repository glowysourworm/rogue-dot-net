using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Math.Geometry;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Processing.Model.Algorithm;

using System;
using System.Collections.Generic;
using System.Linq;

using static Rogue.NET.Core.Model.Scenario.Content.Layout.LayoutGrid;

namespace Rogue.NET.Core.Model.Scenario.Dynamic.Layout
{
    /// <summary>
    /// Component for planning character movement between search and rest periods
    /// </summary>
    public class CharacterMovementPlanner
    {
        protected enum SearchState
        {
            // Initial state
            WakingUp,
            BeginningSearch,
            Searching,
            HeadedForRest,
            Resting
        }

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
        List<Region<GridLocation>> _searchedRegions;

        // Mark regions within search radius - BEFORE EACH NEW SWEEP
        List<Region<GridLocation>> _searchableRegions;

        // Search grid for the current region
        SearchGrid<GridLocation> _searchGrid;

        // Route to plan movement between two regions
        Region<GridLocation> _nextRegion;

        // Current rest clock - keeps track of rest period
        int _restClock;
        SearchState _state;

        /// <summary>
        /// Constructs a movement planner for characters that rest at the location where they finish their search
        /// sweep.
        /// </summary>
        public CharacterMovementPlanner(LayoutGrid layoutGrid, int searchRadius, int restPeriod)
        {
            _layoutGrid = layoutGrid;

            _restPeriod = restPeriod;
            _searchRadius = searchRadius;
            _state = SearchState.WakingUp;

            _searchedRegions = new List<Region<GridLocation>>();
            _searchableRegions = new List<Region<GridLocation>>();
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
            _state = SearchState.WakingUp;

            _searchedRegions = new List<Region<GridLocation>>();
            _searchableRegions = new List<Region<GridLocation>>();
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
            return _state == SearchState.Resting;
        }

        /// <summary>
        /// Updates character visibility and route planning
        /// </summary>
        public void Update(GridLocation location, double effectiveVision)
        {
            // Decrement rest clock
            if (_restClock > 0)
            {
                _restClock--;

                if (_restClock == 0)
                    _state = SearchState.WakingUp;
            }

            _visibleLocations.Clear();

            // Calculate visible locations -> MUST PREVENT NON-WALKABLE LOCATIONS FROM BEING SET ON THE SEARCH GRID
            VisibilityCalculator.CalculateVisibility(_layoutGrid, location, (int)(effectiveVision * ModelConstants.MaxVisibileRadiusNPC), 
            (column, row, isVisible) =>
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
                    // 2) Connection region exists for this location (SearchGrid.Region MUST BE CONNECTED REGION)
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

        public GridLocation GetNextSearchLocation(GridLocation currentLocation, double effectiveVision)
        {
            switch (_state)
            {
                case SearchState.Resting:
                    throw new Exception("Character is currently resting - must check before calling CharacterMovementPlanner.GetNextSearchLocation");
                case SearchState.WakingUp:
                    return WakingUp(currentLocation, effectiveVision);
                case SearchState.BeginningSearch:
                    return BeginningSearch(currentLocation, effectiveVision);
                case SearchState.Searching:
                    return Searching(currentLocation, effectiveVision);
                case SearchState.HeadedForRest:
                    return HeadedForRest(currentLocation);
                default:
                    throw new Exception("Unhandled SearchState case CharacterMovementPlanner.GetNextSearchLocation");
            }
        }

        private GridLocation Searching(GridLocation currentLocation, double effectiveVision)
        {
            // Search just completed
            if (_searchGrid.IsFullySearched())
            {
                // Mark the current region as searched
                _searchedRegions.Add(_searchGrid.Region);

                // Search is fully exhausted -> character heads to rest location -> set rest clock
                if (_searchedRegions.Count == _searchableRegions.Count)
                {
                    // Search sweep is finished
                    _searchGrid = null;
                    _nextRegion = null;

                    // Signal that the sweep is finished
                    _state = SearchState.HeadedForRest;
                }

                // Calculate the next region
                else
                {
                    // Search grid is finished
                    _searchGrid = null;

                    // Find the next region in the search
                    _nextRegion = GetNextRegion(currentLocation);

                    if (_nextRegion == null)
                        throw new Exception("No region found to search CharacterMovementPlanner.GetNextSearchLocation");

                    _state = SearchState.BeginningSearch;
                }

                return GetNextSearchLocation(currentLocation, effectiveVision);
            }

            // Continue search...
            else
                return _searchGrid.GetNextSearchLocation();
        }

        private GridLocation HeadedForRest(GridLocation currentLocation)
        {
            // Character begins rest period -> set rest clock
            if (_restLocation == null ||
                _restLocation.Equals(currentLocation))
            {
                // Set the clock
                _restClock = _restPeriod;

                _state = SearchState.Resting;

                return currentLocation;
            }
            else
            {
                return _restLocation ?? currentLocation;
            }
        }

        private GridLocation WakingUp(GridLocation currentLocation, double effectiveVision)
        {
            if (_searchGrid != null ||
                _nextRegion != null)
                throw new Exception("Trying to re-initialize already initialized search sweep");

            // Set searchable regions -> use rest location if it is available for the search radius
            _searchableRegions = _layoutGrid.ConnectionMap
                                            .Regions
                                            .Where(region =>
                                            {
                                                // USE REGION BOUNDARY TO MAKE THIS SIMPLE
                                                return region.Boundary
                                                             .GetCorners()
                                                             .Any(corner => Metric.Distance(corner, _restLocation ?? currentLocation) <= _searchRadius);

                                            }).ToList();

            // Clear out searched regions
            _searchedRegions.Clear();

            _state = SearchState.BeginningSearch;

            return GetNextSearchLocation(currentLocation, effectiveVision);
        }

        private GridLocation BeginningSearch(GridLocation currentLocation, double effectiveVision)
        {
            if (_searchGrid != null)
                throw new Exception("Improperly handled search grid CharacterMovementPlanner.BeginningSearch");

            // No searchable regions detected - just return this location
            if (!_searchableRegions.Any())
                return currentLocation;

            // Case 1: Character is initializing new sweep
            // Case 2: Character en-route to new region (same sweep)

            // NOTE*** Connection map isn't actually connected. It just STORES region
            //         connections (like a graph) that HAVE been connected by the layout builder.
            //
            var currentRegion = _layoutGrid.ConnectionMap[currentLocation];

            // Case 1
            if (_nextRegion == null)
                _nextRegion = GetNextRegion(currentLocation);

            // Case 2 (OR) Case 1 -> Case 2
            if (_nextRegion != currentRegion)
            {
                // NOTE*** Just need to get the character to the region to let the search grid take
                //         over and begin a sweep.
                //
                //         So, going to use the closest connection point to the current location as a destination
                //
                return  _layoutGrid.ConnectionMap
                                   .ConnectionGraph
                                   .GetConnectionPoints(_nextRegion)
                                   .MinBy(location => Metric.Distance(currentLocation, location));
            }

            // CHARACTER HAS ARRIVED! BEGIN A NEW SWEEP
            else
            {
                // Begin next search sweep
                _searchGrid = new SearchGrid<GridLocation>(_nextRegion, _restLocation ?? currentLocation, _searchRadius);

                // Nullify region reference to signal that character has begun a sweep
                _nextRegion = null;

                // Initialize the new visibility
                Update(currentLocation, effectiveVision);

                _state = SearchState.Searching;

                // Recursively return next search location - in case there are no locations here to search
                return GetNextSearchLocation(currentLocation, effectiveVision);
            }
        }

        private Region<GridLocation> GetNextRegion(GridLocation currentLocation)
        {
            // CHECK FOR ONLY A SINGLE REGION IN THE LAYER
            if (_layoutGrid.ConnectionMap.Regions.Count() == 1)
                return _layoutGrid.ConnectionMap.Regions.First();

            // Plan route to next CONNECTED region
            else
            {


                // Get the current CONNECTED region (May not be in connected layer)
                var currentRegion = _layoutGrid.ConnectionMap[currentLocation];

                // Calculate the next region using Breadth first search from the current region
                if (currentRegion != null)
                    return TraversalAlgorithm.FindNextSearchRegion(_layoutGrid.ConnectionMap, 
                                                                   _searchableRegions, 
                                                                   _searchedRegions,
                                                                   currentRegion);

                // Starting search from the closest non-occupied CONNECTED region
                else
                {
                    // Query for the closest connection region location (WALKABLE is guaranteed for current location)
                    var connectedLocation = _layoutGrid.GetClosestLocationInLayer(currentLocation, LayoutLayer.Walkable, LayoutLayer.ConnectionRoom);

                    // Set the next region from this location
                    return _layoutGrid.ConnectionMap[connectedLocation];
                }
            }
        }
    }
}
