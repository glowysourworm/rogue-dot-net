using Rogue.NET.Core.Math.Geometry;
using Rogue.NET.Core.Math.Geometry.Interface;
using Rogue.NET.Core.Model.Scenario.Content.Layout.Interface;
using Rogue.NET.Core.Processing.Model.Generator.Interface;

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Rogue.NET.Core.Model.Scenario.Content.Layout
{
    /// <summary>
    /// Serializable data structure to store calculated room information
    /// </summary>
    [Serializable]
    public class Region<T> : ISerializable, IRegionGraphWeightProvider<T> where T : class, IGridLocator
    {
        public string Id { get; private set; }
        public T[] Locations { get; private set; }
        public T[] EdgeLocations { get; private set; }
        public IEnumerable<T> OccupiedLocations { get { return _occupiedLocations; } }
        public IEnumerable<T> NonOccupiedLocations { get { return _nonOccupiedLocations; } }
        public RegionBoundary Boundary { get; private set; }

        // Occupied Location Collections
        List<T> _occupiedLocations;
        List<T> _nonOccupiedLocations;

        // Used during layout generation to store calculated nearest neighbors (STORED BY HASH CODE)
        Dictionary<string, RegionConnection<T>> _regionConnections;

        // 2D Arrays for region locations and edges
        T[,] _gridLocations;
        bool[,] _edgeLocations;
        bool[,] _occupiedLocationGrid;

        // Indexers for grid locations and edges
        public T this[int column, int row]
        {
            get { return _gridLocations[column, row]; }
        }
        public bool IsEdge(int column, int row)
        {
            return _edgeLocations[column, row];
        }
        public bool IsOccupied(int column, int row)
        {
            return _occupiedLocationGrid[column, row];
        }
        public void SetOccupied(int column, int row, bool occupied)
        {
            // Occupied
            if (_occupiedLocationGrid[column, row])
            {
                if (!occupied)
                {
                    _occupiedLocations.Remove(_gridLocations[column, row]);
                    _nonOccupiedLocations.Add(_gridLocations[column, row]);
                }
            }

            // Non-Occupied
            else
            {
                if (occupied)
                {
                    _nonOccupiedLocations.Remove(_gridLocations[column, row]);
                    _occupiedLocations.Add(_gridLocations[column, row]);
                }
            }

            _occupiedLocationGrid[column, row] = occupied;
        }

        /// <summary>
        /// Returns id's for the nearest neighbor connected regions
        /// </summary>
        public IEnumerable<string> GetConnectionIds()
        {
            return _regionConnections.Keys;
        }

        public Region(T[] locations, T[] edgeLocations, RegionBoundary boundary, RegionBoundary parentBoundary)
        {
            this.Id = Guid.NewGuid().ToString();
            this.Locations = locations;
            this.EdgeLocations = edgeLocations;
            this.Boundary = boundary;

            _regionConnections = new Dictionary<string, RegionConnection<T>>();
            _gridLocations = new T[parentBoundary.Width, parentBoundary.Height];
            _edgeLocations = new bool[parentBoundary.Width, parentBoundary.Height];
            _occupiedLocationGrid = new bool[parentBoundary.Width, parentBoundary.Height];

            _occupiedLocations = new List<T>();
            _nonOccupiedLocations = new List<T>(locations);

            // Setup grid locations
            foreach (var location in locations)
            {
                // Validate location inside boundary
                if (!boundary.Contains(location))
                    throw new Exception("Invalid location for the region boundary Region.cs");

                _gridLocations[location.Column, location.Row] = location;
            }

            // Setup edge locations
            foreach (var location in edgeLocations)
            {
                // Validate location inside boundary
                if (!boundary.Contains(location))
                    throw new Exception("Invalid edge location for the region boundary Region.cs");

                _edgeLocations[location.Column, location.Row] = true;
            }
        }
        public Region(SerializationInfo info, StreamingContext context)
        {
            this.Id = info.GetString("Id");
            this.Locations = new T[info.GetInt32("LocationsLength")];
            this.EdgeLocations = new T[info.GetInt32("EdgeLocationsLength")];
            this.Boundary = (RegionBoundary)info.GetValue("Boundary", typeof(RegionBoundary));

            var parentBoundary = (RegionBoundary)info.GetValue("ParentBoundary", typeof(RegionBoundary));

            _regionConnections = (Dictionary<string, RegionConnection<T>>)info.GetValue("RegionConnections", typeof(Dictionary<string, RegionConnection<T>>));

            _gridLocations = new T[parentBoundary.Width, parentBoundary.Height];
            _edgeLocations = new bool[parentBoundary.Width, parentBoundary.Height];
            _occupiedLocationGrid = new bool[parentBoundary.Width, parentBoundary.Height];

            // Initialize occupied collections
            _occupiedLocations = new List<T>();
            _nonOccupiedLocations = new List<T>();

            for (int i = 0; i < this.Locations.Length; i++)
            {
                var location = (T)info.GetValue("Location" + i.ToString(), typeof(T));
                var locationOccupied = info.GetBoolean("LocationOccupied" + i.ToString());

                // Add to cell array
                this.Locations[i] = location;

                if (locationOccupied)
                    _occupiedLocations.Add(location);

                else
                    _nonOccupiedLocations.Add(location);

                // Add to 2D array (AND) Occupied 2D array
                _gridLocations[location.Column, location.Row] = location;
                _occupiedLocationGrid[location.Column, location.Row] = locationOccupied;
            }

            for (int i = 0; i < this.EdgeLocations.Length; i++)
            {
                var edgeLocation = (T)info.GetValue("EdgeLocation" + i.ToString(), typeof(T));

                // Add to edge cell array
                this.EdgeLocations[i] = edgeLocation;

                // Add to 2D edge array
                _edgeLocations[edgeLocation.Column, edgeLocation.Row] = true;
            }
        }
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Id", this.Id);
            info.AddValue("LocationsLength", this.Locations.Length);
            info.AddValue("EdgeLocationsLength", this.EdgeLocations.Length);
            info.AddValue("Boundary", this.Boundary);
            info.AddValue("ParentBoundary", new RegionBoundary(0, 0, _gridLocations.GetLength(0), _gridLocations.GetLength(1)));
            info.AddValue("RegionConnections", _regionConnections);

            var counter = 0;

            foreach (var location in this.Locations)
            {
                // VALIDATE TYPE USED AS IGridLocator
                if (location.GetType() != typeof(GridLocation))
                    throw new SerializationException("Unsupported IGridLocator type during serialization Region.cs");

                info.AddValue("Location" + counter.ToString(), location);
                info.AddValue("LocationOccupied" + counter++.ToString(), _occupiedLocationGrid[location.Column, location.Row]);
            }

            counter = 0;

            foreach (var location in this.EdgeLocations)
            {
                // VALIDATE TYPE USED AS IGridLocator
                if (location.GetType() != typeof(GridLocation))
                    throw new SerializationException("Unsupported IGridLocator type during serialization Region.cs");

                info.AddValue("EdgeLocation" + counter++.ToString(), location);
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is Region<T>)
            {
                var region = obj as Region<T>;

                // Check that the number of cells matches
                if (region.Locations.Length != this.Locations.Length)
                    return false;

                // Check that the boundary matches
                if (!region.Boundary.Equals(this.Boundary))
                    return false;

                // Iterate until a mis-match is found
                foreach (var otherLocation in region.Locations)
                {
                    if (this[otherLocation.Column, otherLocation.Row] == null)
                        return false;
                }

                // Found match for each cell
                return true;
            }

            else
                return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("Locations[{0}], EdgeLocations[{1}], Boundary=[{2}]", this.Locations.Length, this.EdgeLocations.Length, this.Boundary.ToString());
        }

        #region IRegionGraphWeightProvider

        public void CalculateConnection(Region<T> adjacentRegion, IRandomSequenceGenerator randomSequenceGenerator)
        {
            // Return previously calculated weight
            if (_regionConnections.ContainsKey(adjacentRegion.Id))
                return;

            // Use a brute force O(n x m) search
            var candidateLocations = new Dictionary<T, List<T>>();
            var distance = double.MaxValue;

            foreach (var edgeLocation1 in this.EdgeLocations)
            {
                foreach (var edgeLocation2 in adjacentRegion.EdgeLocations)
                {
                    var nextDistance = Metric.EuclideanDistance(edgeLocation1, edgeLocation2);

                    // Reset candidates
                    if (nextDistance < distance)
                    {
                        // Clear out more distant locations
                        candidateLocations.Clear();

                        distance = nextDistance;

                        // Keep track of locations with this distance
                        candidateLocations.Add(edgeLocation1, new List<T>() { edgeLocation2 });
                    }
                    else if (nextDistance == distance)
                    {
                        if (!candidateLocations.ContainsKey(edgeLocation1))
                            candidateLocations.Add(edgeLocation1, new List<T>() { edgeLocation2 });

                        else
                            candidateLocations[edgeLocation1].Add(edgeLocation2);
                    }

                }
            }

            if (distance == double.MaxValue)
                throw new Exception("No adjacent node connection found Region.CalculateWeight");

            // Choose edge locations randomly
            var element = randomSequenceGenerator.GetRandomElement(candidateLocations);
            var location = element.Key;
            var adjacentLocation = randomSequenceGenerator.GetRandomElement(element.Value);

            _regionConnections.Add(adjacentRegion.Id, new RegionConnection<T>()
            {
                AdjacentRegionId = adjacentRegion.Id,
                AdjacentLocation = adjacentLocation,
                Location = location,
                Distance = distance
            });

            // Set adjacent region's connection
            adjacentRegion.SetConnection(this, adjacentLocation, location, distance);
        }

        public void SetConnection(Region<T> adjacentRegion, T location, T adjacentLocation, double distance)
        {
            if (!_regionConnections.ContainsKey(adjacentRegion.Id))
            {
                _regionConnections.Add(adjacentRegion.Id, new RegionConnection<T>()
                {
                    AdjacentRegionId = adjacentRegion.Id,
                    AdjacentLocation = adjacentLocation,
                    Location = location,
                    Distance = distance
                });
            }
            else
            {
                _regionConnections[adjacentRegion.Id].AdjacentLocation = adjacentLocation;
                _regionConnections[adjacentRegion.Id].Location = location;
                _regionConnections[adjacentRegion.Id].Distance = distance;
            }
        }

        public RegionConnection<T> GetConnection(Region<T> adjacentRegion)
        {
            if (!_regionConnections.ContainsKey(adjacentRegion.Id))
                throw new Exception("Trying to get connection point for adjacent region that hasn't been calculated (in the graph)");

            return _regionConnections[adjacentRegion.Id];
        }
        #endregion
    }
}
