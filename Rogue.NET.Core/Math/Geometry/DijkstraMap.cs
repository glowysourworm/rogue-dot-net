using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Math.Geometry
{
    /// <summary>
    /// Provides shortest-distance methods based on Dijkstra's Algorithm. TODO:TERRAIN Support multiple targets.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DijkstraMap<T> where T : Region
    {
        public DijkstraMapNode<T> SourceNode { get; private set; }
        public DijkstraMapNode<T> TargetNode { get; private set; }

        public DijkstraMap(DijkstraMapNode<T> sourceNode, DijkstraMapNode<T> targetNode)
        {
            this.SourceNode = sourceNode;
            this.TargetNode = targetNode;
        }

        public IEnumerable<T> GetShortestPath()
        {
            var path = new List<T>();
            var currentNode = this.TargetNode;
            while (currentNode != this.SourceNode)
            {
                // Add current node's reference object
                path.Add(currentNode.Reference);

                // Follow the graph by the smallest weight
                currentNode = currentNode.ConnectedNodes.MinBy(node => node.DijkstraWeight);
            }

            // Add the start node
            path.Add(currentNode.Reference);

            // Reverse the order of the path
            path.Reverse();

            return path;
        }
    }
}
