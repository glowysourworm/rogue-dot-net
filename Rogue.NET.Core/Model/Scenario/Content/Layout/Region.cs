using Rogue.NET.Core.Math.Geometry;
using Rogue.NET.Core.Math.Geometry.Interface;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using static Rogue.NET.Core.Math.Geometry.Metric;

namespace Rogue.NET.Core.Model.Scenario.Content.Layout
{
    /// <summary>
    /// Serializable data structure to store calculated room information
    /// </summary>
    [Serializable]
    public class Region : ISerializable, IRegionGraphWeightProvider
    {
        public GridLocation[] Cells { get; private set; }
        public GridLocation[] EdgeCells { get; private set; }
        public RegionBoundary Bounds { get; private set; }

        /// <summary>
        /// Calculated flag to specify whether region is rectangular (or amorphous)
        /// </summary>
        public bool IsRectangular { get; private set; }

        // Used during layout generation to store calculated nearest neighbors
        Dictionary<Region, GraphConnection> _graphConnections;

        // 2D Arrays for region cells and edges
        GridLocation[,] _gridLocations;
        bool[,] _edgeLocations;

        // Indexers for grid locations and edges
        public GridLocation this[int column, int row]
        {
            get { return _gridLocations[column, row]; }
        }
        public bool IsEdge(int column, int row)
        {
            return _edgeLocations[column, row];
        }

        protected struct GraphConnection
        {
            public GridLocation Location { get; set; }
            public GridLocation AdjacentLocation { get; set; }
            public Region AdjacentRegion { get; set; }
            public MetricType Metric { get; set; }
            public double Distance { get; set; }
        }

        public Region(GridLocation[] cells, GridLocation[] edgeCells, RegionBoundary bounds, RegionBoundary parentBoundary)
        {
            this.Cells = cells;
            this.EdgeCells = edgeCells;
            this.Bounds = bounds;
            this.IsRectangular = cells.Length == (bounds.CellWidth * bounds.CellHeight);

            _graphConnections = new Dictionary<Region, GraphConnection>();
            _gridLocations = new GridLocation[parentBoundary.CellWidth, parentBoundary.CellHeight];
            _edgeLocations = new bool[parentBoundary.CellWidth, parentBoundary.CellHeight];

            // Setup grid locations
            foreach (var cell in cells)
                _gridLocations[cell.Column, cell.Row] = cell;

            // Setup edge locations
            foreach (var cell in edgeCells)
                _edgeLocations[cell.Column, cell.Row] = true;
        }
        public Region(SerializationInfo info, StreamingContext context)
        {
            _graphConnections = new Dictionary<Region, GraphConnection>();

            this.Cells = new GridLocation[info.GetInt32("CellsLength")];
            this.EdgeCells = new GridLocation[info.GetInt32("EdgeCellsLength")];
            this.Bounds = (RegionBoundary)info.GetValue("Bounds", typeof(RegionBoundary));
            this.IsRectangular = info.GetBoolean("IsRectangular");

            var parentBoundary = (RegionBoundary)info.GetValue("ParentBoundary", typeof(RegionBoundary));

            _gridLocations = new GridLocation[parentBoundary.CellWidth, parentBoundary.CellHeight];
            _edgeLocations = new bool[parentBoundary.CellWidth, parentBoundary.CellHeight];

            for (int i = 0; i < this.Cells.Length; i++)
            {
                var cell = (GridLocation)info.GetValue("Cell" + i.ToString(), typeof(GridLocation));

                // Add to cell array
                this.Cells[i] = cell;

                // Add to 2D array
                _gridLocations[cell.Column, cell.Row] = cell;
            }

            for (int i = 0; i < this.EdgeCells.Length; i++)
            {
                var edgeCell = (GridLocation)info.GetValue("EdgeCell" + i.ToString(), typeof(GridLocation));

                // Add to edge cell array
                this.EdgeCells[i] = edgeCell;

                // Add to 2D edge array
                _edgeLocations[edgeCell.Column, edgeCell.Row] = true;
            }
        }
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("CellsLength", this.Cells.Length);
            info.AddValue("EdgeCellsLength", this.EdgeCells.Length);
            info.AddValue("Bounds", this.Bounds);
            info.AddValue("IsRectangular", this.IsRectangular);
            info.AddValue("ParentBoundary", new RegionBoundary(new GridLocation(0,0), _gridLocations.GetLength(0), _gridLocations.GetLength(1)));

            var counter = 0;

            foreach (var cell in this.Cells)
                info.AddValue("Cell" + counter++.ToString(), cell);

            counter = 0;

            foreach (var cell in this.EdgeCells)
                info.AddValue("EdgeCell" + counter++.ToString(), cell);
        }

        #region IRegionGraphWeightProvider

        public double CalculateWeight(Region adjacentNode, Metric.MetricType metricType)
        {
            // Return previously calculated weight
            if (_graphConnections.ContainsKey(adjacentNode))
                return _graphConnections[adjacentNode].Distance;

            // Use the centers to calculate the weight
            if (this.IsRectangular && adjacentNode.IsRectangular)
            {
                var distance = double.MaxValue;

                switch (metricType)
                {
                    case Metric.MetricType.Roguian:
                        distance = Metric.RoguianDistance(this.Bounds.Center, adjacentNode.Bounds.Center);
                        break;
                    case Metric.MetricType.Euclidean:
                        distance = Metric.EuclideanDistance(this.Bounds.Center, adjacentNode.Bounds.Center);
                        break;
                    default:
                        throw new Exception("Unhandled metric type Region.CalculateWeight");
                }

                _graphConnections.Add(adjacentNode, new GraphConnection()
                {
                    AdjacentLocation = adjacentNode.Bounds.Center,
                    AdjacentRegion = adjacentNode,
                    Distance = distance,
                    Metric = metricType,
                    Location = this.Bounds.Center
                });
            }
            // Use a brute force O(n x m) search
            else
            {
                GridLocation location = null;
                GridLocation adjacentLocation = null;
                double distance = double.MaxValue;

                foreach (var edgeLocation1 in this.EdgeCells)
                {
                    foreach (var edgeLocation2 in adjacentNode.EdgeCells)
                    {
                        var nextDistance = Metric.RoguianDistance(edgeLocation1, edgeLocation2);

                        // Reset candidates
                        if (nextDistance < distance)
                        {
                            distance = nextDistance;
                            location = edgeLocation1;
                            adjacentLocation = edgeLocation2;
                        }
                    }
                }

                if (distance == double.MaxValue)
                    throw new Exception("No adjacent node connection found Region.CalculateWeight");

                _graphConnections.Add(adjacentNode, new GraphConnection()
                {
                    AdjacentLocation = adjacentLocation,
                    AdjacentRegion = adjacentNode,
                    Distance = distance,
                    Metric = metricType,
                    Location = location
                });
            }

            return _graphConnections[adjacentNode].Distance;
        }

        public GridLocation GetConnectionPoint(Region adjacentRegion, Metric.MetricType metricType)
        {
            if (!_graphConnections.ContainsKey(adjacentRegion))
                throw new Exception("Trying to get connection point for adjacent region that hasn't been calculated (in the graph)");

            return _graphConnections[adjacentRegion].Location;
        }

        public GridLocation GetAdjacentConnectionPoint(Region adjacentRegion, Metric.MetricType metricType)
        {
            if (!_graphConnections.ContainsKey(adjacentRegion))
                throw new Exception("Trying to get connection point for adjacent region that hasn't been calculated (in the graph)");

            return _graphConnections[adjacentRegion].AdjacentLocation;
        }
        #endregion
    }
}
