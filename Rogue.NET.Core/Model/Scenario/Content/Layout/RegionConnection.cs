using Rogue.NET.Common.Serialization.Component.Interface;
using Rogue.NET.Common.Serialization.Interface;
using Rogue.NET.Core.Math.Algorithm.Interface;

using System;

namespace Rogue.NET.Core.Model.Scenario.Content.Layout
{
    [Serializable]
    public class RegionConnection : IGraphEdge<Region<GridLocation>>, IRecursiveSerializable
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

        public RegionConnection(IPropertyReader reader)
        {
            this.Node = reader.Read<Region<GridLocation>>("Node");
            this.AdjacentNode = reader.Read<Region<GridLocation>>("AdjacentNode");
            this.Location = reader.Read<GridLocation>("Location");
            this.AdjacentLocation = reader.Read<GridLocation>("AdjacentLocation");
            this.Weight = reader.Read<double>("Weight");
        }

        public void GetProperties(IPropertyWriter writer)
        {
            writer.Write("Node", this.Node);
            writer.Write("AdjacentNode", this.AdjacentNode);
            writer.Write("Location", this.Location);
            writer.Write("AdjacentLocation", this.AdjacentLocation);
            writer.Write("Weight", this.Weight);
        }
    }
}
