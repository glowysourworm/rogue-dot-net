using Rogue.NET.Common.Collection;
using Rogue.NET.Common.Extension;
using Rogue.NET.Common.Serialization.Component.Interface;
using Rogue.NET.Common.Serialization.Interface;
using Rogue.NET.Core.Math.Algorithm.Interface;
using Rogue.NET.Core.Math.Geometry;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Rogue.NET.Core.Model.Scenario.Content.Layout
{
    [Serializable]
    public class RegionGraph : IGraph<Region<GridLocation>, RegionConnection>, IRecursiveSerializable
    {
        // TRIVIAL GRAPH
        Region<GridLocation> _singleRegion;

        // PRIMARY EDGES
        GraphEdgeCollection<Region<GridLocation>, RegionConnection> _edges;

        // Connection points (non-serialized)
        SimpleDictionary<Region<GridLocation>, List<GridLocation>> _connectionPoints;

        // Mode setting
        bool _isSingleRegion;

        public RegionGraph(Region<GridLocation> singleRegion)
        {
            Initialize(true, singleRegion, null);
        }

        public RegionGraph(IEnumerable<RegionConnection> connections)
        {
            Initialize(false, null, connections);
        }

        public RegionGraph(IPropertyReader reader)
        {
            var isSingleRegion = reader.Read<bool>("IsSingleRegion");

            Region<GridLocation> singleRegion = null;
            List<RegionConnection> edges = null;

            if (isSingleRegion)
            {
                singleRegion = reader.Read<Region<GridLocation>>("SingleRegion");
            }
            else
            {
                edges = reader.Read<List<RegionConnection>>("Edges");
            }

            Initialize(isSingleRegion, singleRegion, edges);
        }

        public void GetProperties(IPropertyWriter writer)
        {
            writer.Write("IsSingleRegion", _isSingleRegion);

            if (_isSingleRegion)
            {
                writer.Write("SingleRegion", _singleRegion);
            }
            else
            {
                writer.Write("Edges", _edges.GetEdges().ToList());
            }
        }


        private void Initialize(bool isSingleRegion, Region<GridLocation> singleRegion, IEnumerable<RegionConnection> connections)
        {
            _isSingleRegion = isSingleRegion;

            if (isSingleRegion)
            {
                _singleRegion = singleRegion;
            }
            else
            {
                _edges = new GraphEdgeCollection<Region<GridLocation>, RegionConnection>(connections, false);
                _connectionPoints = new SimpleDictionary<Region<GridLocation>, List<GridLocation>>();

                // Pre-calculate the connection points
                foreach (var connection in connections)
                {
                    // Node -> Connection Point
                    if (_connectionPoints.ContainsKey(connection.Node))
                    {
                        if (!_connectionPoints[connection.Node].Contains(connection.Location))
                            _connectionPoints[connection.Node].Add(connection.Location);
                    }
                    else
                    {
                        _connectionPoints.Add(connection.Node, new List<GridLocation>() { connection.Location });
                    }

                    // Adjacent Node -> Adjacent Connection Point
                    if (_connectionPoints.ContainsKey(connection.AdjacentNode))
                    {
                        if (!_connectionPoints[connection.AdjacentNode].Contains(connection.AdjacentLocation))
                            _connectionPoints[connection.AdjacentNode].Add(connection.AdjacentLocation);
                    }
                    else
                    {
                        _connectionPoints.Add(connection.AdjacentNode, new List<GridLocation>() { connection.AdjacentLocation });
                    }
                }
            }
        }

        public IEnumerable<Region<GridLocation>> Vertices
        {
            get 
            {
                if (!_isSingleRegion)
                    return _edges.GetNodes();

                return new Region<GridLocation>[] { _singleRegion };
            }
        }

        public IEnumerable<GridLocation> GetConnectionPoints(Region<GridLocation> node)
        {
            if (_isSingleRegion)
                return new GridLocation[] { };

            if (!_connectionPoints.ContainsKey(node))
                throw new Exception("Region not found in RegionGraph:  " + node.Id);

            return _connectionPoints[node];
        }

        public IEnumerable<RegionConnection> GetAdjacentEdges(Region<GridLocation> node)
        {
            if (!_isSingleRegion)
                return _edges.GetAdjacentEdges(node);

            return Array.Empty<RegionConnection>();
        }

        public RegionConnection FindEdge(Region<GridLocation> node1, Region<GridLocation> node2)
        {
            if (!_isSingleRegion)
                return _edges.Find(node1, node2);

            return null;
        }
    }
}
