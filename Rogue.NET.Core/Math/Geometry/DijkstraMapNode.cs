using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Math.Geometry
{
    /// <summary>
    /// Represents a node in the Dijkstra Map. The class T is used as a reference to the attached object
    /// that the node represents.
    /// </summary>
    public class DijkstraMapNode<T> where T : class
    {
        /// <summary>
        /// Connected nodes in the graph
        /// </summary>
        public IEnumerable<DijkstraMapNode<T>> ConnectedNodes { get; private set; }

        /// <summary>
        /// Reference to the node's attached object
        /// </summary>
        public T Reference { get; private set; }

        /// <summary>
        /// The weight for this node in the Dijkstra map. This is accumulated based on the starting and
        /// ending node(s) in the graph.
        /// </summary>
        public double DijkstraWeight { get; set; }

        public void AddNode(DijkstraMapNode<T> node)
        {
            var connectedNodes = new List<DijkstraMapNode<T>>(this.ConnectedNodes);

            connectedNodes.Add(node);

            this.ConnectedNodes = connectedNodes;
        }

        public DijkstraMapNode(T reference, double dijkstraWeight)
        {
            this.ConnectedNodes = new List<DijkstraMapNode<T>>();
            this.Reference = reference;
            this.DijkstraWeight = dijkstraWeight;
        }
    }
}
