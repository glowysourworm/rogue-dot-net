using Rogue.NET.Core.Math.Algorithm.Interface;

using System;
using System.Runtime.Serialization;

namespace Rogue.NET.Core.Model.Scenario.Content.Layout
{
    [Serializable]
    public class RegionConnection : IGraphEdge<Region<GridLocation>>, ISerializable
    {
        public Region<GridLocation> Node { get; private set; }
        public Region<GridLocation> AdjacentNode { get; private set; }

        public GridLocation Location { get; private set; }
        public GridLocation AdjacentLocation { get; private set; }

        /// <summary>
        /// Represents the euclidean rendered distance between the two region's connection points
        /// </summary>
        public double Weight { get; private set; }

        public RegionConnection(Region<GridLocation> node, Region<GridLocation> adjacentNode, GridLocation location, GridLocation adjacentLocation, double weight)
        {
            this.Node = node;
            this.AdjacentNode = adjacentNode;
            this.Location = location;
            this.AdjacentLocation = adjacentLocation;
            this.Weight = weight;
        }

        public RegionConnection(SerializationInfo info, StreamingContext context)
        {
            this.Node = (Region<GridLocation>)info.GetValue("Node", typeof(Region<GridLocation>));
            this.AdjacentNode = (Region<GridLocation>)info.GetValue("AdjacentNode", typeof(Region<GridLocation>));
            this.Location = (GridLocation)info.GetValue("Location", typeof(GridLocation));
            this.AdjacentLocation = (GridLocation)info.GetValue("AdjacentLocation", typeof(GridLocation));
            this.Weight = info.GetDouble("Weight");
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Node", this.Node);
            info.AddValue("AdjacentNode", this.AdjacentNode);
            info.AddValue("Location", this.Location);
            info.AddValue("AdjacentLocation", this.AdjacentLocation);
            info.AddValue("Weight", this.Weight);
        }
    }
}
