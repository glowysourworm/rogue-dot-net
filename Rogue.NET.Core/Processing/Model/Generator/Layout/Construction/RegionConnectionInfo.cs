using Rogue.NET.Core.Math.Geometry;
using Rogue.NET.Core.Model.Scenario.Content.Layout.Interface;

using System.Collections.Generic;

namespace Rogue.NET.Core.Processing.Model.Generator.Layout.Construction
{
    public class RegionConnectionInfo<T> where T : class, IGridLocator
    {
        /// <summary>
        /// Location of the connection point in cell coordinates
        /// </summary>
        public T Location { get; set; }

        /// <summary>
        /// Location of adjacent connection point in cell coordinates
        /// </summary>
        public T AdjacentLocation { get; set; }

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
        public GraphVertex Vertex { get; set; }

        /// <summary>
        /// Location of connection point in rendered euclidean coordinates
        /// </summary>
        public GraphVertex AdjacentVertex { get; set; }

        /// <summary>
        /// Calculated distance between the two VERTICES
        /// </summary>
        public double EuclideanRenderedDistance { get; set; }

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

        public override string ToString()
        {
            return "(" + this.Vertex.ToString() + ") -> (" + this.AdjacentVertex.ToString() + ")";
        }
    }
}
