using Rogue.NET.Core.Math.Algorithm.Interface;
using Rogue.NET.Core.Math.Geometry;
using Rogue.NET.Core.Model.Scenario.Content.Layout.Interface;

using System.Collections.Generic;

namespace Rogue.NET.Core.Processing.Model.Generator.Layout.Construction
{
    /// <summary>
    /// Supports corridor and connection creation by coordinating region connection points. Also, stores
    /// graph data to accomodate triangulation routines and correlate region connections to graph edges.
    /// </summary>
    public class RegionConnectionInfo<T> : IGraphEdge<RegionInfo<T>>
                                           where T : class, IGridLocator
    {
        #region IGraphEdge
        public RegionInfo<T> Node { get; private set; }

        public RegionInfo<T> AdjacentNode { get; private set; }

        public double Weight
        {
            get { return this.EuclideanRenderedDistance; }
        }
        #endregion

        /// <summary>
        /// Location of the connection point in cell coordinates
        /// </summary>
        public T Location { get; private set; }

        /// <summary>
        /// Location of adjacent connection point in cell coordinates
        /// </summary>
        public T AdjacentLocation { get; private set; }

        /// <summary>
        /// Locations of potential door calculated by Dijkstra's algorithm during the connection process
        /// </summary>
        public T DoorLocation { get; set; }

        /// <summary>
        /// Locations of potential door calculated by Dijkstra's algorithm during the connection process
        /// </summary>
        public T AdjacentDoorLocation { get; set; }

        /// <summary>
        /// Locations of corridors calculated by Dijkstra's algorithm during the connection process
        /// </summary>
        public IEnumerable<T> CorridorLocations { get; set; }

        /// <summary>
        /// Location of connection point in rendered euclidean coordinates
        /// </summary>
        public GraphVertex Vertex { get; private set; }

        /// <summary>
        /// Location of connection point in rendered euclidean coordinates
        /// </summary>
        public GraphVertex AdjacentVertex { get; private set; }

        /// <summary>
        /// Calculated distance between the two VERTICES
        /// </summary>
        public double EuclideanRenderedDistance { get; private set; }

        /// <summary>
        /// Returns true if the connection represents the SAME connection to the other region. The vertices could be
        /// the same or reversed.
        /// </summary>
        public bool IsEquivalent(RegionConnectionInfo<T> otherConnection)
        {
            if (this.Vertex.Equals(otherConnection.Vertex) &&
                this.AdjacentVertex.Equals(otherConnection.AdjacentVertex))
                return true;

            if (this.Vertex.Equals(otherConnection.AdjacentVertex) &&
                this.AdjacentVertex.Equals(otherConnection.Vertex))
                return true;

            return false;
        }

        public RegionConnectionInfo(RegionInfo<T> region,
                                    RegionInfo<T> adjacentRegion,
                                    T location,
                                    T adjacentLocation,
                                    GraphVertex vertex,
                                    GraphVertex adjacentVertex,
                                    double euclideanRenderedDistance)
        {
            this.Node = region;
            this.AdjacentNode = adjacentRegion;
            this.Location = location;
            this.AdjacentLocation = adjacentLocation;
            this.Vertex = vertex;
            this.AdjacentVertex = adjacentVertex;
            this.EuclideanRenderedDistance = euclideanRenderedDistance;
        }

        public RegionConnectionInfo(RegionConnectionInfo<T> copy)
        {
            this.AdjacentDoorLocation = copy.AdjacentDoorLocation;
            this.AdjacentLocation = copy.AdjacentLocation;
            this.AdjacentNode = copy.AdjacentNode;
            this.AdjacentVertex = copy.AdjacentVertex;
            this.CorridorLocations = copy.CorridorLocations;
            this.DoorLocation = copy.DoorLocation;
            this.EuclideanRenderedDistance = copy.EuclideanRenderedDistance;
            this.Location = copy.Location;
            this.Node = copy.Node;
            this.Vertex = copy.Vertex;
        }

        public override string ToString()
        {
            return "(" + this.Node.Id + ") -> (" + this.AdjacentNode.Id + ")";
        }
    }
}
