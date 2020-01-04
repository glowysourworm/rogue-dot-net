using Rogue.NET.Core.Math.Geometry;
using Rogue.NET.Core.Math.Geometry.Interface;
using Rogue.NET.Core.Model.Scenario.Content.Layout.Interface;
using Rogue.NET.Core.Processing.Model.Generator.Interface;

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Rogue.NET.Core.Model.Scenario.Content.Layout
{
    [Serializable]
    public class ConnectedRegion<T> : Region<T>, IRegionGraphWeightProvider<T> where T : class, IGridLocator
    {
        readonly Dictionary<string, RegionConnection<T>> _connections;

        /// <summary>
        /// Region connection dictionary maintained by Region -> Id
        /// </summary>
        public Dictionary<string, RegionConnection<T>> Connections
        {
            get { return _connections; }
        }

        public ConnectedRegion(string connectedRegionId, T[] locations, T[] edgeLocations, RegionBoundary boundary, RegionBoundary parentBoundary)
                : base(connectedRegionId, locations, edgeLocations, boundary, parentBoundary)
        {
            _connections = new Dictionary<string, RegionConnection<T>>();
        }

        public ConnectedRegion(T[] locations, T[] edgeLocations, RegionBoundary boundary, RegionBoundary parentBoundary)
                : base(locations, edgeLocations, boundary, parentBoundary)
        {
            _connections = new Dictionary<string, RegionConnection<T>>();
        }

        public ConnectedRegion(SerializationInfo info, StreamingContext context)
                : base(info, context)
        {
            _connections = (Dictionary<string, RegionConnection<T>>)info.GetValue("Connections", typeof(Dictionary<string, RegionConnection<T>>));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("Connections", _connections);
        }

        public void CalculateConnection(ConnectedRegion<T> adjacentRegion, IRandomSequenceGenerator randomSequenceGenerator)
        {
            // Return previously calculated weight
            if (_connections.ContainsKey(adjacentRegion.Id))
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

            _connections.Add(adjacentRegion.Id, new RegionConnection<T>()
            {
                AdjacentRegionId = adjacentRegion.Id,
                AdjacentLocation = adjacentLocation,
                Location = location,
                Distance = distance
            });

            // Set adjacent region's connection
            adjacentRegion.SetConnection(this, adjacentLocation, location, distance);
        }

        public void SetConnection(ConnectedRegion<T> adjacentRegion, T location, T adjacentLocation, double distance)
        {
            if (!_connections.ContainsKey(adjacentRegion.Id))
            {
                _connections.Add(adjacentRegion.Id, new RegionConnection<T>()
                {
                    AdjacentRegionId = adjacentRegion.Id,
                    AdjacentLocation = adjacentLocation,
                    Location = location,
                    Distance = distance
                });
            }
            else
            {
                _connections[adjacentRegion.Id].AdjacentLocation = adjacentLocation;
                _connections[adjacentRegion.Id].Location = location;
                _connections[adjacentRegion.Id].Distance = distance;
            }
        }

        public RegionConnection<T> GetConnection(ConnectedRegion<T> adjacentRegion)
        {
            if (!_connections.ContainsKey(adjacentRegion.Id))
                throw new Exception("Trying to get connection point for adjacent region that hasn't been calculated (in the graph)");

            return _connections[adjacentRegion.Id];
        }
    }
}
