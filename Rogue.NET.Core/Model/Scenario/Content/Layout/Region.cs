﻿using Rogue.NET.Core.Math.Geometry;
using Rogue.NET.Core.Math.Geometry.Interface;
using Rogue.NET.Core.Model.Scenario.Content.Layout.Interface;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using static Rogue.NET.Core.Math.Geometry.Metric;

namespace Rogue.NET.Core.Model.Scenario.Content.Layout
{
    /// <summary>
    /// Serializable data structure to store calculated room information
    /// </summary>
    [Serializable]
    public class Region<T> : ISerializable, IRegionGraphWeightProvider<T> where T : class, IGridLocator
    {
        public T[] Locations { get; private set; }
        public T[] EdgeLocations { get; private set; }
        public RegionBoundary Boundary { get; private set; }

        // Used during layout generation to store calculated nearest neighbors
        Dictionary<Region<T>, GraphConnection> _graphConnections;

        // 2D Arrays for region locations and edges
        T[,] _gridLocations;
        bool[,] _edgeLocations;

        // Indexers for grid locations and edges
        public T this[int column, int row]
        {
            get { return _gridLocations[column, row]; }
        }
        public bool IsEdge(int column, int row)
        {
            return _edgeLocations[column, row];
        }

        protected struct GraphConnection
        {
            public T Location { get; set; }
            public T AdjacentLocation { get; set; }
            public Region<T> AdjacentRegion { get; set; }
            public MetricType Metric { get; set; }
            public double Distance { get; set; }
        }

        public Region(T[] locations, T[] edgeLocations, RegionBoundary boundary, RegionBoundary parentBoundary)
        {
            this.Locations = locations;
            this.EdgeLocations = edgeLocations;
            this.Boundary = boundary;

            _graphConnections = new Dictionary<Region<T>, GraphConnection>();
            _gridLocations = new T[parentBoundary.Width, parentBoundary.Height];
            _edgeLocations = new bool[parentBoundary.Width, parentBoundary.Height];

            // Setup grid locations
            foreach (var location in locations)
                _gridLocations[location.Column, location.Row] = location;

            // Setup edge locations
            foreach (var location in edgeLocations)
                _edgeLocations[location.Column, location.Row] = true;
        }
        public Region(SerializationInfo info, StreamingContext context)
        {
            _graphConnections = new Dictionary<Region<T>, GraphConnection>();

            this.Locations = new T[info.GetInt32("LocationsLength")];
            this.EdgeLocations = new T[info.GetInt32("EdgeLocationsLength")];
            this.Boundary = (RegionBoundary)info.GetValue("Boundary", typeof(RegionBoundary));

            var parentBoundary = (RegionBoundary)info.GetValue("ParentBoundary", typeof(RegionBoundary));

            _gridLocations = new T[parentBoundary.Width, parentBoundary.Height];
            _edgeLocations = new bool[parentBoundary.Width, parentBoundary.Height];

            for (int i = 0; i < this.Locations.Length; i++)
            {
                var location = (T)info.GetValue("Location" + i.ToString(), typeof(T));

                // Add to cell array
                this.Locations[i] = location;

                // Add to 2D array
                _gridLocations[location.Column, location.Row] = location;
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
            info.AddValue("LocationsLength", this.Locations.Length);
            info.AddValue("EdgeLocationsLength", this.EdgeLocations.Length);
            info.AddValue("Boundary", this.Boundary);
            info.AddValue("ParentBoundary", new RegionBoundary(0, 0, _gridLocations.GetLength(0), _gridLocations.GetLength(1)));

            var counter = 0;

            foreach (var location in this.Locations)
            {
                // VALIDATE TYPE USED AS IGridLocator
                if (location.GetType() != typeof(GridLocation))
                    throw new SerializationException("Unsupported IGridLocator type during serialization Region.cs");

                info.AddValue("Location" + counter++.ToString(), location);
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
                    var match = false;

                    foreach (var location in this.Locations)
                    {
                        if (location.Equals(otherLocation))
                        {
                            match = true;
                            break;
                        }
                    }

                    if (!match)
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

        public double CalculateWeight(Region<T> adjacentNode, Metric.MetricType metricType)
        {
            // Return previously calculated weight
            if (_graphConnections.ContainsKey(adjacentNode))
                return _graphConnections[adjacentNode].Distance;

            //// Use the centers to calculate the weight
            //if (this.IsRectangular && adjacentNode.IsRectangular)
            //{
            //    var distance = double.MaxValue;

            //    switch (metricType)
            //    {
            //        case Metric.MetricType.Roguian:
            //            distance = Metric.RoguianDistance(this.Bounds.Center, adjacentNode.Bounds.Center);
            //            break;
            //        case Metric.MetricType.Euclidean:
            //            distance = Metric.EuclideanDistance(this.Bounds.Center, adjacentNode.Bounds.Center);
            //            break;
            //        default:
            //            throw new Exception("Unhandled metric type Region.CalculateWeight");
            //    }

            //    _graphConnections.Add(adjacentNode, new GraphConnection()
            //    {
            //        AdjacentLocation = adjacentNode.Bounds.Center,
            //        AdjacentRegion = adjacentNode,
            //        Distance = distance,
            //        Metric = metricType,
            //        Location = this.Bounds.Center
            //    });
            //}
            //// Use a brute force O(n x m) search
            //else
            //{
            T location = null;
            T adjacentLocation = null;
            double distance = double.MaxValue;

            foreach (var edgeLocation1 in this.EdgeLocations)
            {
                foreach (var edgeLocation2 in adjacentNode.EdgeLocations)
                {
                    var nextDistance = metricType == MetricType.Roguian ? Metric.RoguianDistance(edgeLocation1, edgeLocation2)
                                                                        : Metric.EuclideanDistance(edgeLocation1, edgeLocation2);

                    // Reset candidates
                    if (nextDistance < distance)
                    {
                        distance = nextDistance;
                        location = edgeLocation1;
                        adjacentLocation = edgeLocation2;
                    }
                }
            }

            if (distance == double.MaxValue)
                throw new Exception("No adjacent node connection found Region.CalculateWeight");

            _graphConnections.Add(adjacentNode, new GraphConnection()
            {
                AdjacentLocation = adjacentLocation,
                AdjacentRegion = adjacentNode,
                Distance = distance,
                Metric = metricType,
                Location = location
            });
            //}

            return _graphConnections[adjacentNode].Distance;
        }

        public T GetConnectionPoint(Region<T> adjacentRegion, Metric.MetricType metricType)
        {
            if (!_graphConnections.ContainsKey(adjacentRegion))
                throw new Exception("Trying to get connection point for adjacent region that hasn't been calculated (in the graph)");

            return _graphConnections[adjacentRegion].Location;
        }

        public T GetAdjacentConnectionPoint(Region<T> adjacentRegion, Metric.MetricType metricType)
        {
            if (!_graphConnections.ContainsKey(adjacentRegion))
                throw new Exception("Trying to get connection point for adjacent region that hasn't been calculated (in the graph)");

            return _graphConnections[adjacentRegion].AdjacentLocation;
        }
        #endregion
    }
}
