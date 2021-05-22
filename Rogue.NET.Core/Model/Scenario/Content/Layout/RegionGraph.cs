using Rogue.NET.Common.Collection;
using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Math.Algorithm.Interface;
using Rogue.NET.Core.Math.Geometry;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Rogue.NET.Core.Model.Scenario.Content.Layout
{
    [Serializable]
    public class RegionGraph : IGraph<Region<GridLocation>, RegionConnection>, ISerializable
    {
        // TRIVIAL GRAPH
        Region<GridLocation> _singleRegion;

        // PRIMARY EDGES
        GraphEdgeCollection<Region<GridLocation>, RegionConnection> _edges;

        // Connection points (non-serialized)
        Dictionary<Region<GridLocation>, List<GridLocation>> _connectionPoints;

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

        public RegionGraph(SerializationInfo info, StreamingContext context)
        {
            var isSingleRegion = info.GetBoolean("IsSingleRegion");

            Region<GridLocation> singleRegion = null;
            SimpleList<RegionConnection> edges = null;

            if (isSingleRegion)
            {
                singleRegion = (Region<GridLocation>)info.GetValue("SingleRegion", typeof(Region<GridLocation>));
            }
            else
            {
                edges = (SimpleList<RegionConnection>)info.GetValue("Edges", typeof(SimpleList<RegionConnection>));
            }

            Initialize(isSingleRegion, singleRegion, edges);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            var isSingleRegion = _singleRegion != null;

            info.AddValue("IsSingleRegion", isSingleRegion);

            if (isSingleRegion)
            {
                info.AddValue("SingleRegion", _singleRegion);
            }
            else
            {
                info.AddValue("Edges", _edges.GetEdges().ToSimpleList());
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
                _connectionPoints = new Dictionary<Region<GridLocation>, List<GridLocation>>();

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
