using Rogue.NET.Common.Collection;
using Rogue.NET.Core.Math.Geometry;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.Scenario.Content.Layout.Interface;

namespace Rogue.NET.Core.Model.Scenario.Dynamic.Layout
{
    public class SearchGrid<T> where T : class, IGridLocator
    {
        // Store searched / un-searched locations by:  Angle -> Distance (w.r.t. the center of the region)
        BinarySearchTree<double, BinarySearchTree<int, T>> _searchedLocations;
        BinarySearchTree<double, BinarySearchTree<int, T>> _unSearchedLocations;

        readonly ConnectedRegion<T> _region;
        readonly GridLocation _regionCenter;

        /// <summary>
        /// Returns the id of the current region
        /// </summary>
        public ConnectedRegion<T> Region { get { return _region; } }

        /// <summary>
        /// Initializes a new instance of the SearchGrid with a rest location and search radius - which
        /// is the maximum Euclidean distance (in terms of cells) that the character can search away 
        /// from their rest location.
        /// </summary>
        public SearchGrid(ConnectedRegion<T> region, GridLocation restLocation, int searchRadius)
        {
            _searchedLocations = new BinarySearchTree<double, BinarySearchTree<int, T>>();
            _unSearchedLocations = new BinarySearchTree<double, BinarySearchTree<int, T>>();

            _region = region;
            _regionCenter = region.Boundary.GetCenter();

            Initialize(restLocation, searchRadius);
        }

        private void Initialize(GridLocation restLocation, int searchRadius)
        {
            // Initialize un-searched locations
            foreach (var location in _region.Locations)
            {
                // Check to see that the search location is within the search radius
                if (Metric.EuclideanDistance(location, restLocation) <= searchRadius)
                    Insert(_unSearchedLocations, location);
            }
        }

        private void Insert(BinarySearchTree<double, BinarySearchTree<int, T>> tree, T location)
        {
            // TODO: CONSIDER USING SLOPE INSTEAD OF ANGLE FOR FASTER CALCULATION.
            //
            // ***NOTE: USING ROUNDING TO MAKE THE BST KEYS STABLE
            //
            var angle = System.Math.Atan2(_regionCenter.Row - location.Row, _regionCenter.Column - location.Column);
            var distance = Metric.RoguianDistance(_regionCenter, location);

            // O(1)
            var subTree = tree.Search(angle);

            // Intialize the angle's sub-tree
            if (subTree == null)
            {
                subTree = new BinarySearchTree<int, T>();

                // O(Log n)
                tree.Insert(angle, subTree);
            }

            // Insert the location into the distance sub-tree (O(Log n))
            if (subTree.Search(distance) == null)
                subTree.Insert(distance, location);
        }

        private void Remove(BinarySearchTree<double, BinarySearchTree<int, T>> tree, T location)
        {
            // TODO: CONSIDER USING SLOPE INSTEAD OF ANGLE FOR FASTER CALCULATION.
            //
            // ***NOTE: USING ROUNDING TO MAKE THE BST KEYS STABLE
            //
            var angle = System.Math.Atan2(_regionCenter.Row - location.Row, _regionCenter.Column - location.Column);
            var distance = Metric.RoguianDistance(_regionCenter, location);

            // O(1)
            var subTree = tree.Search(angle);

            if (subTree != null)
            {
                if (subTree.Search(distance) == null)
                    return;

                // O(Log n)
                subTree.Remove(distance);

                // Remove empty sub-tree (O(Log n))
                if (subTree.Count == 0)
                    tree.Remove(angle);
            }
        }

        /// <summary>
        /// Sets visibility and updates searched locations in the region
        /// </summary>
        public void SetSearched(T location)
        {
            // Remove / Insert are fall-through methods

            // MAINTAIN ONLY LOCATIONS IN THE REGION FOR SEARCHING. VISIBILITY IS KEPT
            // ALSO FOR LOCATIONS OUTSIDE THE REGION.

            if (_region[location] == null)
                return;

            // 2 x O(Log n) + O(1)
            Remove(_unSearchedLocations, location);

            // 2 x O(Log n) + O(1)
            Insert(_searchedLocations, location);
        }

        public bool IsFullySearched()
        {
            return _unSearchedLocations.Count == 0;
        }

        public T GetNextSearchLocation()
        {
            if (_unSearchedLocations.Count == 0)
                return null;

            // Fetch Min-by-angle (O(Log n))
            var distanceSubTree = _unSearchedLocations.Min();

            if (distanceSubTree == null)
                return null;

            // Return Min-by-distance (O(Log n))
            return distanceSubTree.Min() ?? null;
        }
    }
}
