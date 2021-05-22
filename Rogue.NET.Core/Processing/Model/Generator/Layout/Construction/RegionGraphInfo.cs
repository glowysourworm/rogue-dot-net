using Rogue.NET.Core.Math.Algorithm.Interface;
using Rogue.NET.Core.Math.Geometry;
using Rogue.NET.Core.Model.Scenario.Content.Layout.Interface;

using System.Collections.Generic;

namespace Rogue.NET.Core.Processing.Model.Generator.Layout.Construction
{
    /// <summary>
    /// Simple graph info class to store connection points along with the graph data
    /// </summary>
    public class RegionGraphInfo<T> : IGraph<RegionInfo<T>, RegionConnectionInfo<T>> where T : class, IGridLocator
    {
        // Supports "trivial graph" of a single region
        RegionInfo<T> _singleRegion;

        GraphEdgeCollection<RegionInfo<T>, RegionConnectionInfo<T>> _edges;

        public IEnumerable<RegionInfo<T>> Vertices
        {
            get
            {
                if (_edges != null)
                    return _edges.GetNodes();

                else
                    return new RegionInfo<T>[] { _singleRegion };
            }
        }

        public bool HasEdges()
        {
            return _edges != null;
        }

        public IEnumerable<RegionConnectionInfo<T>> GetConnections()
        {
            if (_edges == null)
                throw new System.Exception("Tryingn to call for connections on an un-connected graph");

            return _edges.GetEdges();
        }

        public IEnumerable<RegionConnectionInfo<T>> GetAdjacentEdges(RegionInfo<T> node)
        {
            if (_edges == null)
                throw new System.Exception("Tryingn to call for connections on an un-connected graph");

            return _edges.GetAdjacentEdges(node);
        }

        /// <summary>
        /// Removes the existing connection in favor of the new one
        /// </summary>
        public void Modify(RegionConnectionInfo<T> existingConnection, RegionConnectionInfo<T> newConnection)
        {
            if (_edges == null)
                throw new System.Exception("Tryingn to call for connections on an un-connected graph");

            _edges.Remove(existingConnection);
            _edges.Add(newConnection);
        }

        public RegionConnectionInfo<T> FindEdge(RegionInfo<T> node1, RegionInfo<T> node2)
        {
            if (_edges == null)
                throw new System.Exception("Tryingn to call for connections on an un-connected graph");

            return _edges.Find(node1, node2);
        }

        public bool HasEdge(RegionInfo<T> node1, RegionInfo<T> node2)
        {
            if (_edges == null)
                throw new System.Exception("Tryingn to call for connections on an un-connected graph");

            return _edges.HasEdge(node1, node2);
        }

        public RegionGraphInfo(RegionInfo<T> singleRegion)
        {
            _singleRegion = singleRegion;
        }

        public RegionGraphInfo(IEnumerable<RegionConnectionInfo<T>> connections)
        {
            _edges = new GraphEdgeCollection<RegionInfo<T>, RegionConnectionInfo<T>>(connections, false);
        }
    }
}
