using Rogue.NET.Common.Collection;
using Rogue.NET.Common.Extension;
using Rogue.NET.Common.Serialization.Component.Interface;
using Rogue.NET.Common.Serialization.Interface;
using Rogue.NET.Core.Math;
using Rogue.NET.Core.Math.Geometry;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content.Layout.Interface;
using Rogue.NET.Core.Processing.Model.Algorithm;
using Rogue.NET.Core.Processing.Model.Extension;
using Rogue.NET.Core.Processing.Model.Generator.Interface;
using Rogue.NET.Core.Processing.Model.Generator.Layout.Construction;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Rogue.NET.Core.Model.Scenario.Content.Layout
{

    [Serializable]
    public class LayoutGrid : IRecursiveSerializable
    {
        private Grid<GridCell> _grid;

        [Flags]
        public enum LayoutLayer
        {
            /// <summary>
            /// Contains the walkable cell regions for the entire map (should be connected)
            /// </summary>
            Walkable = 1,

            /// <summary>
            /// Contains the cell regions for placing any level content
            /// </summary>
            Placement = 2,

            /// <summary>
            /// Contains the room regions
            /// </summary>
            Room = 4,

            /// <summary>
            /// Contains room regions - subdivided by impassable terrain. These must be connected in the 
            /// connection layer.
            /// </summary>
            ConnectionRoom = 8,

            /// <summary>
            /// Contains cell regions marked "Corridor"
            /// </summary>
            Corridor = 16,

            /// <summary>
            /// Contains cell regions marked "Wall"
            /// </summary>
            Wall = 32,

            /// <summary>
            /// Contains cells that are for terrain support - these MAY OR MAY NOT be impassable terrain cells
            /// </summary>
            TerrainSupport = 64,

            /// <summary>
            /// Contains the walkable map plus impassable terrain.
            /// </summary>
            FullNoTerrainSupport = 128
        }

        // Define "Near the edge" as being randomly distributed with a peak at distance = (X * Grid Dimension) from the 
        // edge of the grid or layer boundary
        //
        private const double SIMULATION_EDGE_NEARNESS = 0.1;

        // Define "Far from X" as being randomly distributed with a peak at distance = (X * Max Distance) from the
        // source location
        private const double SIMULATION_DISTANT_PARAMETER = 0.9;

        #region Properties / Indexers
        public virtual GridCell this[int column, int row]
        {
            //NOTE*** Returns null as a default
            get { return _grid[column, row]; }
        }
        public virtual GridCell this[IGridLocator location]
        {
            //NOTE*** Returns null as a default
            get { return _grid[location.Column, location.Row]; }
        }
        public RegionBoundary Bounds { get; private set; }

        // Connected Layers - Maintain Dijkstra Graph
        public ConnectedLayerMap ConnectionMap { get; private set; }

        // Non-connected Layers
        public LayerMap FullNoTerrainSupportMap { get; private set; }
        public LayerMap WalkableMap { get; private set; }
        public LayerMap PlacementMap { get; private set; }
        public LayerMap RoomMap { get; private set; }
        public LayerMap CorridorMap { get; private set; }
        public LayerMap WallMap { get; private set; }
        public LayerMap TerrainSupportMap { get; private set; }
        public IEnumerable<TerrainLayerMap> TerrainMaps { get; private set; }

        /// <summary>
        /// Sets up cell occupation data in EACH of the regions for ALL layers
        /// </summary>
        public void SetOccupied(IGridLocator location, bool occupied)
        {
            _grid[location.Column, location.Row].IsOccupied = occupied;
        }

        public bool IsOccupied(IGridLocator location)
        {
            return _grid[location.Column, location.Row].IsOccupied;
        }
        #endregion

        /// <summary>
        /// Constructs LayoutGrid from the provided 2D cell arrays and the regions. The cells
        /// in the cell array are by reference; and are not re-created. The region data arrays contain
        /// cell points that are treated as a value type. These are recreated during serialization (not
        /// unique) apart from the cell reference objects. The terrain array follows the same pattern.
        /// 
        /// USAGE:  Create Cell[,] first with room cells already in it. Also, create Room[] first with
        ///         all data prepared. Also, create the terrain array with all data prepared. Corridors 
        ///         may be created afterwards using the public indexer.
        /// </summary>
        public LayoutGrid(GridCellInfo[,] grid,
                          ConnectedLayerInfo<GridLocation> connectionLayer,
                          LayerInfo<GridLocation> fullNoTerrainSupportLayer,
                          LayerInfo<GridLocation> walkableLayer,
                          LayerInfo<GridLocation> placementLayer,
                          LayerInfo<GridLocation> roomLayer,
                          LayerInfo<GridLocation> corridorLayer,
                          LayerInfo<GridLocation> wallLayer,
                          LayerInfo<GridLocation> terrainSupportLayer,
                          IEnumerable<LayerInfo<GridLocation>> terrainLayers)
        {
            // Boundary
            this.Bounds = new RegionBoundary(new GridLocation(0, 0), grid.GetLength(0), grid.GetLength(1));

            // Primary 2D array
            _grid = new Grid<GridCell>(this.Bounds, this.Bounds);

            // Connected Layers
            this.ConnectionMap = new ConnectedLayerMap(connectionLayer.LayerName,
                                                       connectionLayer.RegionGraph,
                                                       connectionLayer.Regions,
                                                       this.Bounds.Width,
                                                       this.Bounds.Height);

            // Non-connected Layers
            this.FullNoTerrainSupportMap = new LayerMap(fullNoTerrainSupportLayer.LayerName, fullNoTerrainSupportLayer.Regions, this.Bounds.Width, this.Bounds.Height);
            this.WalkableMap = new LayerMap(walkableLayer.LayerName, walkableLayer.Regions, this.Bounds.Width, this.Bounds.Height);
            this.PlacementMap = new LayerMap(placementLayer.LayerName, placementLayer.Regions, this.Bounds.Width, this.Bounds.Height);
            this.RoomMap = new LayerMap(roomLayer.LayerName, roomLayer.Regions, this.Bounds.Width, this.Bounds.Height);
            this.CorridorMap = new LayerMap(corridorLayer.LayerName, corridorLayer.Regions, this.Bounds.Width, this.Bounds.Height);
            this.WallMap = new LayerMap(wallLayer.LayerName, wallLayer.Regions, this.Bounds.Width, this.Bounds.Height);
            this.TerrainSupportMap = new LayerMap(terrainSupportLayer.LayerName, terrainSupportLayer.Regions, this.Bounds.Width, this.Bounds.Height);
            this.TerrainMaps = terrainLayers.Select(layer => new TerrainLayerMap(layer.LayerName, layer.Regions, this.Bounds.Width, this.Bounds.Height, !layer.IsPassable))
                                            .Actualize();

            // Initialize the grid
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    if (grid[i, j] != null)
                    {
                        // Create the grid cell
                        _grid[i, j] = new GridCell(grid[i, j].Location, grid[i, j].IsWall, grid[i, j].IsWallLight,
                                                   grid[i, j].IsDoor, grid[i, j].DoorSearchCounter, grid[i, j].AmbientLight,
                                                   grid[i, j].WallLight, grid[i, j].AccentLight, grid[i, j].TerrainLights.Values);
                    }
                }
            }
        }

        #region IRecursiveSerializable
        public LayoutGrid(IPropertyReader reader)
        {
            _grid = reader.Read<Grid<GridCell>>("Grid");
            this.Bounds = reader.Read<RegionBoundary>("Bounds");
            this.ConnectionMap = reader.Read<ConnectedLayerMap>("ConnectionMap");
            this.RoomMap = reader.Read<LayerMap>("RoomMap");
            this.FullNoTerrainSupportMap = reader.Read<LayerMap>("FullNoTerrainSupportMap");
            this.WalkableMap = reader.Read<LayerMap>("WalkableMap");
            this.PlacementMap = reader.Read<LayerMap>("PlacementMap");
            this.CorridorMap = reader.Read<LayerMap>("CorridorMap");
            this.WallMap = reader.Read<LayerMap>("WallMap");
            this.TerrainSupportMap = reader.Read<LayerMap>("TerrainSupportMap");
            this.TerrainMaps = reader.Read<List<TerrainLayerMap>>("TerrainMaps");
        }

        public void GetProperties(IPropertyWriter writer)
        {
            writer.Write("Grid", _grid);
            writer.Write("Bounds", this.Bounds);
            writer.Write("ConnectionMap", this.ConnectionMap);
            writer.Write("RoomMap", this.RoomMap);
            writer.Write("FullNoTerrainSupportMap", this.FullNoTerrainSupportMap);
            writer.Write("WalkableMap", this.WalkableMap);
            writer.Write("PlacementMap", this.PlacementMap);
            writer.Write("CorridorMap", this.CorridorMap);
            writer.Write("WallMap", this.WallMap);
            writer.Write("TerrainSupportMap", this.TerrainSupportMap);
            writer.Write("TerrainMaps", this.TerrainMaps.ToList());
        }
        #endregion

        #region (public) Extension Method Hooks (These prevent exposing the Cell[,])
        public IEnumerable<GridLocation> GetAdjacentLocations(GridLocation location)
        {
            return _grid.GetAdjacentElements(location.Column, location.Row).Select(cell => cell.Location);
        }
        public IEnumerable<GridLocation> GetAdjacentLocations(int column, int row)
        {
            return _grid.GetAdjacentElements(column, row).Select(cell => cell.Location);
        }
        public IEnumerable<GridLocation> GetCardinarlAdjacentLocations(GridLocation location)
        {
            return _grid.GetCardinalAdjacentElements(location.Column, location.Row).Select(cell => cell.Location);
        }
        public IEnumerable<GridLocation> GetCardinarlAdjacentLocations(int column, int row)
        {
            return _grid.GetCardinalAdjacentElements(column, row).Select(cell => cell.Location);
        }
        public IEnumerable<GridCell> GetAdjacentCells(GridCell cell)
        {
            return _grid.GetAdjacentElements(cell.Location.Column, cell.Location.Row);
        }
        public IEnumerable<GridCell> GetAdjacentCells(int column, int row)
        {
            return _grid.GetAdjacentElements(column, row);
        }
        public IEnumerable<GridCell> GetCardinarlAdjacentCells(GridCell cell)
        {
            return _grid.GetCardinalAdjacentElements(cell.Location.Column, cell.Location.Row);
        }
        public IEnumerable<GridCell> GetCardinarlAdjacentCells(int column, int row)
        {
            return _grid.GetCardinalAdjacentElements(column, row);
        }
        public GridCell GetOffDiagonalCell1(GridLocation location, Compass direction, out Compass cardinalDirection1)
        {
            return _grid.GetOffDiagonalElement1(location.Column, location.Row, direction, out cardinalDirection1);
        }
        public GridCell GetOffDiagonalCell2(GridLocation location, Compass direction, out Compass cardinalDirection2)
        {
            return _grid.GetOffDiagonalElement2(location.Column, location.Row, direction, out cardinalDirection2);
        }
        #endregion

        #region (public) Queries

        public bool LayerContains(LayoutLayer layer, GridLocation location)
        {
            var map = SelectLayer(layer);

            return map[location.Column, location.Row] != null;
        }

        /// <summary>
        /// Returns random non-occupied location from the specified layer map
        /// </summary>
        public GridLocation GetNonOccupiedLocation(LayoutLayer layer, IRandomSequenceGenerator randomSequenceGenerator, IEnumerable<GridLocation> excludedLocations = null)
        {
            var map = SelectLayer(layer);

            // Have to create collection of non-occupied locations for this layer
            var locations = map.GetLocations()
                               .Where(location => !_grid[location.Column, location.Row].IsOccupied);

            if (excludedLocations != null)
                locations = locations.Except(excludedLocations);
            
            return randomSequenceGenerator.GetRandomElement(locations);
        }

        /// <summary>
        /// Returns a rectangle of non-occupied contiguous locations from the specified layer map. Will return null if there are no such rectangles are available. 
        /// </summary>
        public Region<GridLocation> GetNonOccupiedRegionLocationGroup(LayoutLayer layer, int width, int height, IRandomSequenceGenerator randomSequenceGenerator, IEnumerable<GridLocation> excludedLocations = null)
        {
            var map = SelectLayer(layer);

            // Search for sub-regions that have a contiguous square of non-occupied cells
            var qualifiedRegions = _grid.ScanRectangularRegions(width, height, location =>
            {
                if (excludedLocations != null)
                {
                    return  map[location] != null &&
                           !excludedLocations.Any(otherLocation => otherLocation.Equals(location));
                }
                else
                    return map[location] != null;
            }, (cell) => cell.Location);

            if (qualifiedRegions.Count() == 0)
                return null;

            // Select random region from the qualified regions
            var region = randomSequenceGenerator.GetRandomElement(qualifiedRegions);

            // Convert the region to grid locations
            var result = new Region<GridLocation>(System.Guid.NewGuid().ToString(),
                                                  region.Locations.ToArray(),
                                                  region.EdgeLocations.ToArray(),
                                                  region.Boundary,
                                                  new RegionBoundary(0, 0, this.Bounds.Width, this.Bounds.Height));

            return result;
        }

        /// <summary>
        /// Returns a location that is near the edge of the specified layer map
        /// </summary>
        public GridLocation GetNonOccupiedEdgeLocation(LayoutLayer layer, IRandomSequenceGenerator randomSequenceGenerator, IEnumerable<GridLocation> excludedLocations = null)
        {
            // Have to create a PDF based on edge distance - with a peak at SIMULATION_EDGE_NEARNESS (in the dimension of the
            // closest edge). Then, use this PDF to create weighted random draw over the space of non-occupied locations.

            var map = SelectLayer(layer);

            // Have to create collection of non-occupied locations for this layer
            var locations = map.GetLocations()
                               .Where(location => !_grid[location.Column, location.Row].IsOccupied);

            if (excludedLocations != null)
                locations = locations.Except(excludedLocations);

            var edgePdf = GetDistancePdf(locations, map.Boundary);

            return randomSequenceGenerator.GetWeightedRandom(edgePdf.Keys, location => edgePdf[location]);
        }

        /// <summary>
        /// Returns a location that is "maximally distant" from the specified source locations - using the specified layer map
        /// </summary>
        public GridLocation GetNonOccupiedDistantLocations(LayoutLayer layer, IEnumerable<GridLocation> sourceLocations, IRandomSequenceGenerator randomSequenceGenerator, IEnumerable<GridLocation> excludedLocations = null)
        {
            // Have to create a PDF based on distance from a source location - with a peak at SIMULATION_DISTANT_PARAMETER (in distance). 
            // Then, use this PDF to create weighted random draw over the space of non-occupied locations.

            var map = SelectLayer(layer);

            // Have to create collection of non-occupied locations for this layer
            var locations = map.GetLocations()
                               .Where(location => !_grid[location.Column, location.Row].IsOccupied);

            if (excludedLocations != null)
                locations = locations.Except(excludedLocations);

            var edgePdf = GetDistancePdf(locations, sourceLocations);

            return randomSequenceGenerator.GetWeightedRandom(edgePdf.Keys, location => edgePdf[location]);
        }

        /// <summary>
        /// Returns non-occupied locations in range of the specified location - using the specified layer map
        /// </summary>
        public IEnumerable<GridLocation> GetNonOccupiedLocationsNear(LayoutLayer layer, GridLocation location, int range)
        {
            var map = SelectLayer(layer);

            var locations = new List<GridLocation>();

            // Iterate around the location on the primary grid - checking the layer map
            _grid.IterateAround(location.Column, location.Row, range, (cell) =>
            {
                if (map[cell] != null &&
                   !_grid[cell.Column, cell.Row].IsOccupied)
                {
                    locations.Add(_grid[cell.Column, cell.Row].Location);
                }

                return true;
            });

            return locations;
        }

        /// <summary>
        /// Returns closest non-occupied location to the specified location - using the specified layer map
        /// </summary>
        public GridLocation GetClosestNonOccupiedLocation(LayoutLayer layer, GridLocation location, IEnumerable<GridLocation> excludedLocations = null)
        {
            var map = SelectLayer(layer);

            // Have to create collection of non-occupied locations for this layer
            var locations = map.GetLocations()
                               .Where(location => !_grid[location.Column, location.Row].IsOccupied);

            if (excludedLocations != null)
                locations = locations.Except(excludedLocations);

            if (excludedLocations == null)
                return locations
                          .Except(new GridLocation[] { location })
                          .MinBy(otherLocation => Metric.ForceDistance(location, otherLocation, Metric.MetricType.Euclidean));
            else
                return locations
                          .Except(excludedLocations)
                          .Except(new GridLocation[] { location })
                          .MinBy(otherLocation => Metric.ForceDistance(location, otherLocation, Metric.MetricType.Euclidean));
        }

        /// <summary>
        /// Searches for closest location in ANOTHER layer using CURRENT layer and location
        /// </summary>
        public GridLocation GetClosestLocationInLayer(GridLocation location, LayoutLayer currentLayer, LayoutLayer otherLayer)
        {
            var currentMap = SelectLayer(currentLayer);
            var otherMap = SelectLayer(otherLayer);

            if (currentMap[location] == null)
                throw new Exception("Trying to search location in layer that doesn't exist: LayoutGrid.GetClosestLocationInLayer");

            var maxRadius = MathFunctions.Max(this.Bounds.Right - location.Column,
                                              this.Bounds.Bottom - location.Row,
                                              location.Column - this.Bounds.Left,
                                              location.Row - this.Bounds.Top);

            GridLocation result = null;

            _grid.IterateAround(location.Column, location.Row, maxRadius, cell =>
            {
                // Found region in other layer
                if (otherMap[cell] != null)
                {
                    result = cell.Location;
                    return false;
                }

                return true;
            });

            return result;
        }

        /// <summary>
        /// Selects the proper layer for the specified enumeration
        /// </summary>
        public ILayerMap SelectLayer(LayoutLayer layer)
        {
            switch (layer)
            {
                case LayoutLayer.Walkable:
                    return this.WalkableMap;
                case LayoutLayer.Placement:
                    return this.PlacementMap;
                case LayoutLayer.Room:
                    return this.RoomMap;
                case LayoutLayer.ConnectionRoom:
                    return this.ConnectionMap;
                case LayoutLayer.Corridor:
                    return this.CorridorMap;
                case LayoutLayer.Wall:
                    return this.WallMap;
                case LayoutLayer.TerrainSupport:
                    return this.TerrainSupportMap;
                case LayoutLayer.FullNoTerrainSupport:
                    return this.FullNoTerrainSupportMap;
                default:
                    throw new Exception("Unhandled LayoutLayer GetNonOccupiedLocation");
            }
        }

        /// <summary>
        /// Generates a distance pdf to simulate a "Nearness" draw for edge locations based on the provided location set and
        /// the boundary.
        /// </summary>
        private SimpleDictionary<GridLocation, double> GetDistancePdf(IEnumerable<GridLocation> locations, RegionBoundary boundary)
        {
            var meanLeft = boundary.Left + (SIMULATION_EDGE_NEARNESS * boundary.Width);
            var meanRight = boundary.Right - (SIMULATION_EDGE_NEARNESS * boundary.Width);
            var meanTop = boundary.Top + (SIMULATION_EDGE_NEARNESS * boundary.Height);
            var meanBottom = boundary.Bottom - (SIMULATION_EDGE_NEARNESS * boundary.Height);
            var center = boundary.GetCenter();

            return locations.ToSimpleDictionary(location => location, location =>
            {
                var distanceLeft = location.Column - boundary.Left;
                var distanceRight = boundary.Right - location.Column;
                var distanceTop = location.Row - boundary.Top;
                var distanceBottom = boundary.Bottom - location.Row;

                var minDistance = MathFunctions.Min(distanceLeft, distanceRight, distanceTop, distanceBottom);

                // Closer to the Left
                if (distanceLeft == minDistance)
                    return GetTriangleFunction(location.Column, boundary.Left, meanLeft, center.Column);

                // Closer to the Right
                else if (distanceRight == minDistance)
                    return GetTriangleFunction(location.Column, center.Column, meanRight, boundary.Right);

                // Closer to the Top
                else if (distanceTop == minDistance)
                    return GetTriangleFunction(location.Row, boundary.Top, meanTop, center.Row);

                // Closer to the Bottom
                else
                    return GetTriangleFunction(location.Row, center.Row, meanBottom, boundary.Bottom);
            });
        }

        /// <summary>
        /// Generates a distance pdf to simulate a "Distant" draw for locations "far away from the specified location"
        /// </summary>
        private SimpleDictionary<GridLocation, double> GetDistancePdf(IEnumerable<GridLocation> locations, IEnumerable<GridLocation> sourceLocations)
        {
            // Want a PDF based on max distance from the source location
            //
            var maxDistance = locations.Max(location => sourceLocations.Max(source => Metric.ForceDistance(location, source, Metric.MetricType.Euclidean)));

            return locations.ToSimpleDictionary(location => location, location =>
            {
                // Select the minimum distance between this location and each of the source locations as a weight
                var minDistance = sourceLocations.Min(source => Metric.ForceDistance(location, source, Metric.MetricType.Euclidean));

                return GetTriangleFunction(minDistance, 0, SIMULATION_DISTANT_PARAMETER * maxDistance, maxDistance);
            });
        }

        // Generates a piece-wise triangle function with a peak of 1 at the specified "x" locations
        private double GetTriangleFunction(double x, double start, double peak, double end)
        {
            if (x < start || x > end)
                throw new ArgumentException("Trying to generate Triangle function outside the domain LayoutGrid");

            if (x < peak)
                return (1.0 / (peak - start)) - (start / (peak - start));

            else
                return (1.0 / (peak - end)) - (end / (peak - end));
        }

        #endregion
    }
}
