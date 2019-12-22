using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.Scenario.Content.Layout.Interface;
using Rogue.NET.Core.Processing.Model.Extension;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Rogue.NET.Core.Math.Geometry
{
    public class Graph<T> where T : class
    {
        // Maintain edges by vertex
        Dictionary<GraphVertex<T>, GraphEdgeCollection<T>> _vertexDict;

        // Also, maintain edges separately
        GraphEdgeCollection<T> _edges;


        /// <summary>
        /// Returns distinct edges in the graph
        /// </summary>
        public IEnumerable<GraphEdge<T>> Edges
        {
            get { return _edges.Get(); }
        }

        /// <summary>
        /// Returns distinct set of vertices in the graph
        /// </summary>
        public IEnumerable<GraphVertex<T>> Vertices
        {
            get { return _vertexDict.Keys; }
        }

        public Graph()
        {
            _vertexDict = new Dictionary<GraphVertex<T>, GraphEdgeCollection<T>>();
            _edges = new GraphEdgeCollection<T>();
        }

        public Graph(IEnumerable<GraphEdge<T>> edges)
        {
            _vertexDict = new Dictionary<GraphVertex<T>, GraphEdgeCollection<T>>();
            _edges = new GraphEdgeCollection<T>();

            // Add all edges to the graph
            foreach (var edge in edges)
                AddEdge(edge);
        }

        public void AddVertex(GraphVertex<T> vertex)
        {
            _vertexDict.Add(vertex, new GraphEdgeCollection<T>());
        }

        public void AddEdge(GraphEdge<T> edge)
        {
            if (edge.Point1.Column == edge.Point2.Column &&
                edge.Point1.Row == edge.Point2.Row)
                throw new ArgumentException("Invalid graph edge - starts and ends at the same location");

            // Detects duplicate edges
            _edges.Add(edge);

            // Add Point1
            if (!_vertexDict.ContainsKey(edge.Point1))
                _vertexDict.Add(edge.Point1, new GraphEdgeCollection<T>(new GraphEdge<T>[] { edge }));

            else
                _vertexDict[edge.Point1].Add(edge);

            // Add Point2
            if (!_vertexDict.ContainsKey(edge.Point2))
                _vertexDict.Add(edge.Point2, new GraphEdgeCollection<T>(new GraphEdge<T>[] { edge }));

            else
                _vertexDict[edge.Point2].Add(edge);
        }

        /// <summary>
        /// O(1) Vertex lookup
        /// </summary>
        public bool Contains(GraphVertex<T> vertex)
        {
            return _vertexDict.ContainsKey(vertex);
        }

        public IEnumerable<GraphVertex<T>> Find(T reference)
        {
            return _vertexDict.Keys.Where(key => key.Reference == reference);
        }

        public IEnumerable<GraphEdge<T>> FindEdges(T reference1, T reference2)
        {
            return _edges.Get()
                         .Where(edge => (edge.Point1.Reference == reference1 &&
                                         edge.Point2.Reference == reference2) ||
                                        (edge.Point1.Reference == reference2 &&
                                         edge.Point2.Reference == reference1))
                         .Actualize();
        }

        public void Remove(GraphEdge<T> edge)
        {
            foreach (var key in _vertexDict.Keys)
            {
                if (_vertexDict[key].Contains(edge))
                    _vertexDict[key].Remove(edge);
            }

            _edges.Remove(edge);
        }

        /// <summary>
        /// Removes edges according to the supplied predicate
        /// </summary>
        public void FilterByEdge(Func<GraphEdge<T>, bool> predicate)
        {
            _edges.Filter(predicate);
            _vertexDict.ForEach(element => element.Value.Filter(predicate));
        }

        public void FilterByVertex(Func<GraphVertex<T>, bool> predicate)
        {
            // Remove vertices with edge collections
            var elements = _vertexDict.Filter(element => predicate(element.Key));

            // Remove edges with any of the filtered points
            _edges.Filter(edge => elements.ContainsKey(edge.Point1) || 
                                  elements.ContainsKey(edge.Point2));
        }

        /// <summary>
        /// Returns a set of connections for the vertex. The point of this is to be able to select edges from 
        /// the point of view of each vertex to work with triangulation.
        /// </summary>
        public IEnumerable<GraphEdge<T>> this[GraphVertex<T> vertex]
        {
            get { return _vertexDict[vertex].Get(); }
        }

        // Use to help debug
        public void OutputCSV(string directory, string filePrefix)
        {
            var width = this.Vertices.Max(vertex => vertex.Column) + 1;
            var height = this.Vertices.Max(vertex => vertex.Row) + 1;

            var map = new string[width, height];

            map.Iterate((column, row) =>
            {
                if (this.Vertices.Any(vertex => vertex.Column == column && vertex.Row == row))
                {
                    map[column, row] = "N";
                }

                else if (this.Vertices.Any(vertex =>
                {
                    return vertex.Reference != null &&
                          (vertex.Reference as Region<GridCellInfo>)[column, row] != null;
                }))
                {
                    map[column, row] = "R";
                }

                else
                    map[column, row] = "-";
            });

            OutputCSV(map, Path.Combine(directory, filePrefix + ".csv"));
        }

        private void OutputCSV(string[,] matrix, string fileName)
        {
            var builder = new StringBuilder();

            // Output by row CSV
            for (int j = 0; j < matrix.GetLength(1); j++)
            {
                for (int i = 0; i < matrix.GetLength(0); i++)
                    builder.Append(matrix[i, j] + ", ");

                // Remove trailing comma
                builder.Remove(builder.Length - 1, 1);

                // Append return carriage
                builder.Append("\r\n");
            }

            File.WriteAllText(fileName, builder.ToString());
        }
    }
}
