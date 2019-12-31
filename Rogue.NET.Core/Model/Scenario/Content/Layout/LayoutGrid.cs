using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Math;
using Rogue.NET.Core.Math.Geometry;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content.Layout.Construction;
using Rogue.NET.Core.Model.Scenario.Content.Layout.Interface;
using Rogue.NET.Core.Processing.Model.Extension;
using Rogue.NET.Core.Processing.Model.Generator.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Rogue.NET.Core.Model.Scenario.Content.Layout
{

    [Serializable]
    public class LayoutGrid : ISerializable
    {
        private GridCell[,] _grid;

        public enum LayoutLayer
        {
            /// <summary>
            /// Contains all cell regions for the entire map that are non-empty
            /// </summary>
            Full,

            /// <summary>
            /// Contains the connected walkable regions + the connection graph
            /// </summary>
            Connection,

            /// <summary>
            /// Contains the non-connected walkable cell regions for the entire map
            /// </summary>
            Walkable,

            /// <summary>
            /// Contains the cell regions for placing any level content
            /// </summary>
            Placement,

            /// <summary>
            /// Contains the original room locations - prior to terrain placement - with the
            /// impassable terrain cells removed.
            /// </summary>
            Room,

            /// <summary>
            /// Contains cell regions marked "Corridor"
            /// </summary>
            Corridor,

            /// <summary>
            /// Contains cell regions marked "Door"
            /// </summary>
            Door,

            /// <summary>
            /// Contains cell regions marked "Wall"
            /// </summary>
            Wall,

            /// <summary>
            /// Contains cell regions marked "Wall Light"
            /// </summary>
            WallLight,

            /// <summary>
            /// Contains cell regions of impassable terrain
            /// </summary>
            ImpassableTerrain
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
            get { return _grid.Get(column, row); }
        }
        public virtual GridCell this[IGridLocator location]
        {
            //NOTE*** Returns null as a default
            get { return _grid.Get(location.Column, location.Row); }
        }
        public RegionBoundary Bounds { get; private set; }

        // Connected Layers - Maintain Dijkstra Graph
        public ConnectedLayerMap ConnectionMap { get; private set; }

        // Non-connected Layers
        public LayerMap FullMap { get; private set; }
        public LayerMap WalkableMap { get; private set; }
        public LayerMap PlacementMap { get; private set; }
        public LayerMap RoomMap { get; private set; }
        public LayerMap CorridorMap { get; private set; }
        public LayerMap DoorMap { get; private set; }
        public LayerMap WallMap { get; private set; }
        public LayerMap WallLightMap { get; private set; }
        public LayerMap ImpassableTerrainMap { get; private set; }
        public IEnumerable<LayerMap> TerrainMaps { get; private set; }

        /// <summary>
        /// Sets up cell occupation data in EACH of the regions for ALL layers
        /// </summary>
        public void SetOccupied(IGridLocator location, bool occupied)
        {
            // Connected layers
            this.ConnectionMap.SetOccupied(location, occupied);

            // Non-connected layers
            this.FullMap.SetOccupied(location, occupied);
            this.WalkableMap.SetOccupied(location, occupied);
            this.PlacementMap.SetOccupied(location, occupied);
            this.RoomMap.SetOccupied(location, occupied);
            this.CorridorMap.SetOccupied(location, occupied);
            this.DoorMap.SetOccupied(location, occupied);
            this.WallMap.SetOccupied(location, occupied);
            this.WallLightMap.SetOccupied(location, occupied);
            this.ImpassableTerrainMap.SetOccupied(location, occupied);

            foreach (var layer in this.TerrainMaps)
                layer.SetOccupied(location, occupied);
        }

        public bool IsOccupied(IGridLocator location)
        {
            return this.ConnectionMap.IsOccupied(location) ||
                   this.FullMap.IsOccupied(location) ||
                   this.WalkableMap.IsOccupied(location) ||
                   this.PlacementMap.IsOccupied(location) ||
                   this.RoomMap.IsOccupied(location) ||
                   this.DoorMap.IsOccupied(location) ||
                   this.WallMap.IsOccupied(location) ||
                   this.WallLightMap.IsOccupied(location) ||
                   this.CorridorMap.IsOccupied(location) ||
                   this.ImpassableTerrainMap.IsOccupied(location) ||
                   this.TerrainMaps.Any(map => map.IsOccupied(location));
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
                          LayerInfo fullLayer,
                          ConnectedLayerInfo connectionLayer,
                          LayerInfo walkableLayer,
                          LayerInfo placementLayer,
                          LayerInfo roomLayer,
                          LayerInfo corridorLayer,
                          LayerInfo doorLayer,
                          LayerInfo wallLayer,
                          LayerInfo wallLightLayer,
                          IEnumerable<LayerInfo> terrainLayers)
        {
            // Primary 2D array
            _grid = new GridCell[grid.GetLength(0), grid.GetLength(1)];

            // Boundary
            this.Bounds = new RegionBoundary(new GridLocation(0, 0), grid.GetLength(0), grid.GetLength(1));

            // Select distinct, impassable regions for the impassable terrain map
            var impassableTerrainRegions = terrainLayers.Where(layer => !layer.IsPassable)
                                                        .SelectMany(layer => layer.Regions);

            // Connected Layers
            this.ConnectionMap = new ConnectedLayerMap(connectionLayer.LayerName, connectionLayer.RegionGraph, connectionLayer.Regions, this.Bounds.Width, this.Bounds.Height);

            // Non-connected Layers
            this.FullMap = new LayerMap(fullLayer.LayerName, fullLayer.Regions, this.Bounds.Width, this.Bounds.Height);
            this.WalkableMap = new LayerMap(walkableLayer.LayerName, walkableLayer.Regions, this.Bounds.Width, this.Bounds.Height);
            this.PlacementMap = new LayerMap(placementLayer.LayerName, placementLayer.Regions, this.Bounds.Width, this.Bounds.Height);
            this.RoomMap = new LayerMap(roomLayer.LayerName, roomLayer.Regions, this.Bounds.Width, this.Bounds.Height);
            this.CorridorMap = new LayerMap(corridorLayer.LayerName, corridorLayer.Regions, this.Bounds.Width, this.Bounds.Height);
            this.DoorMap = new LayerMap(doorLayer.LayerName, doorLayer.Regions, this.Bounds.Width, this.Bounds.Height);
            this.WallMap = new LayerMap(wallLayer.LayerName, wallLayer.Regions, this.Bounds.Width, this.Bounds.Height);
            this.WallLightMap = new LayerMap(wallLightLayer.LayerName, wallLightLayer.Regions, this.Bounds.Width, this.Bounds.Height);
            this.ImpassableTerrainMap = new LayerMap("Impassable Terrain", impassableTerrainRegions, this.Bounds.Width, this.Bounds.Height);
            this.TerrainMaps = terrainLayers.Select(layer => new LayerMap(layer.LayerName, layer.Regions, this.Bounds.Width, this.Bounds.Height))
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
                                                   grid[i, j].IsDoor, grid[i, j].DoorSearchCounter, grid[i, j].BaseLight,
                                                   grid[i, j].WallLight);
                    }
                }
            }
        }

        #region ISerializable
        public LayoutGrid(SerializationInfo info, StreamingContext context)
        {
            var width = info.GetInt32("Width");
            var height = info.GetInt32("Height");
            var count = info.GetInt32("Count");
            var terrainCount = info.GetInt32("TerrainMapCount");

            var connectionMap = (ConnectedLayerMap)info.GetValue("ConnectionMap", typeof(ConnectedLayerMap));
            var roomMap = (LayerMap)info.GetValue("RoomMap", typeof(LayerMap));
            var fullMap = (LayerMap)info.GetValue("FullMap", typeof(LayerMap));
            var walkableMap = (LayerMap)info.GetValue("WalkableMap", typeof(LayerMap));
            var placementMap = (LayerMap)info.GetValue("PlacementMap", typeof(LayerMap));
            var corridorMap = (LayerMap)info.GetValue("CorridorMap", typeof(LayerMap));
            var doorMap = (LayerMap)info.GetValue("DoorMap", typeof(LayerMap));
            var wallMap = (LayerMap)info.GetValue("WallMap", typeof(LayerMap));
            var wallLightMap = (LayerMap)info.GetValue("WallLightMap", typeof(LayerMap));
            var impassableMap = (LayerMap)info.GetValue("ImpassableTerrainMap", typeof(LayerMap));

            _grid = new GridCell[width, height];
            this.Bounds = new RegionBoundary(new GridLocation(0, 0), width, height);

            var terrainData = new List<LayerMap>();

            // Populate cell grid
            for (int i = 0; i < count; i++)
            {
                var cell = (GridCell)info.GetValue("Cell" + i.ToString(), typeof(GridCell));

                _grid[cell.Location.Column, cell.Location.Row] = cell;
            }

            // Populate terrain
            for (int i = 0; i < terrainCount; i++)
            {
                var terrain = (LayerMap)info.GetValue("TerrainMap" + i.ToString(), typeof(LayerMap));

                terrainData.Add(terrain);
            }

            this.ConnectionMap = connectionMap;
            
            this.FullMap = fullMap;
            this.WalkableMap = walkableMap;
            this.PlacementMap = placementMap;
            this.RoomMap = roomMap;
            this.DoorMap = doorMap;
            this.CorridorMap = corridorMap;
            this.WallMap = wallMap;
            this.WallLightMap = wallLightMap;
            this.ImpassableTerrainMap = impassableMap;
            this.TerrainMaps = terrainData;

            // DESERIALIZATION CALLBACK - SET GRIDLOCATION REFERENCES
            this.ConnectionMap.OnDeserialization(_grid);
            this.FullMap.OnDeserialization(_grid);
            this.WalkableMap.OnDeserialization(_grid);
            this.PlacementMap.OnDeserialization(_grid);
            this.RoomMap.OnDeserialization(_grid);
            this.DoorMap.OnDeserialization(_grid);
            this.CorridorMap.OnDeserialization(_grid);
            this.WallMap.OnDeserialization(_grid);
            this.WallLightMap.OnDeserialization(_grid);
            this.ImpassableTerrainMap.OnDeserialization(_grid);
            this.TerrainMaps.ForEach(map => map.OnDeserialization(_grid));
        }
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            var cells = new List<GridCell>();

            // Collect the grid cells in a list
            for (int i = 0; i < _grid.GetLength(0); i++)
            {
                for (int j = 0; j < _grid.GetLength(1); j++)
                {
                    if (_grid[i, j] != null)
                        cells.Add(_grid[i, j]);
                }
            }

            info.AddValue("Width", _grid.GetLength(0));
            info.AddValue("Height", _grid.GetLength(1));
            info.AddValue("Count", cells.Count);
            info.AddValue("TerrainMapCount", this.TerrainMaps.Count());
            info.AddValue("ConnectionMap", this.ConnectionMap);
            info.AddValue("RoomMap", this.RoomMap);
            info.AddValue("FullMap", this.FullMap);
            info.AddValue("WalkableMap", this.WalkableMap);
            info.AddValue("PlacementMap", this.PlacementMap);
            info.AddValue("CorridorMap", this.CorridorMap);
            info.AddValue("DoorMap", this.DoorMap);
            info.AddValue("WallMap", this.WallMap);
            info.AddValue("WallLightMap", this.WallLightMap);
            info.AddValue("ImpassableTerrainMap", this.ImpassableTerrainMap);

            for (int i = 0; i < cells.Count; i++)
                info.AddValue("Cell" + i.ToString(), cells[i]);

            for (int i = 0; i < this.TerrainMaps.Count(); i++)
                info.AddValue("TerrainMap" + i.ToString(), this.TerrainMaps.ElementAt(i));
        }
        #endregion

        #region (public) Extension Method Hooks (These prevent exposing the Cell[,])
        public IEnumerable<GridLocation> GetAdjacentLocations(GridLocation location)
        {
            return _grid.GetAdjacentElements(location.Column, location.Row).Select(cell => cell.Location);
        }
        public IEnumerable<GridLocation> GetCardinarlAdjacentLocations(GridLocation location)
        {
            return _grid.GetCardinalAdjacentElements(location.Column, location.Row).Select(cell => cell.Location);
        }
        public IEnumerable<GridCell> GetAdjacentCells(GridCell cell)
        {
            return _grid.GetAdjacentElements(cell.Location.Column, cell.Location.Row);
        }
        public IEnumerable<GridCell> GetCardinarlAdjacentCells(GridCell cell)
        {
            return _grid.GetCardinalAdjacentElements(cell.Location.Column, cell.Location.Row);
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
        public GridLocation GetNonOccupiedLocation(LayoutLayer layer, IRandomSequenceGenerator randomSequenceGenerator, IEnumerable<GridLocation> excludedLocations)
        {
            var map = SelectLayer(layer);

            return randomSequenceGenerator.GetRandomElement(map.GetNonOccupiedLocations().Except(excludedLocations));
        }

        /// <summary>
        /// Returns random group of non-occupied locations from the specified layer map - FROM A SINGLE REGION. Will return empty if there are no such
        /// regions available. 
        /// </summary>
        public IEnumerable<GridLocation> GetNonOccupiedRegionLocations(LayoutLayer layer, int locationCount, IRandomSequenceGenerator randomSequenceGenerator, IEnumerable<GridLocation> excludedLocations)
        {
            var map = SelectLayer(layer);

            var qualifiedRegions = map.Regions
                                      .Where(region => region.NonOccupiedLocations.Except(excludedLocations).Count() >= locationCount);

            if (qualifiedRegions.Count() == 0)
                return new GridLocation[] { };

            // Select random region from the qualified regions
            var region = randomSequenceGenerator.GetRandomElement(qualifiedRegions);

            // Return randomly chosen non-occupied locations
            return randomSequenceGenerator.GetDistinctRandomElements(region.NonOccupiedLocations.Except(excludedLocations), locationCount);
        }

        /// <summary>
        /// Returns a rectangle of non-occupied contiguous locations from the specified layer map. Will return null if there are no such rectangles are available. 
        /// </summary>
        public Region<GridLocation> GetNonOccupiedRegionLocationGroup(LayoutLayer layer, int width, int height, IRandomSequenceGenerator randomSequenceGenerator, IEnumerable<GridLocation> excludedLocations)
        {
            var map = SelectLayer(layer);

            // Search for sub-regions that have a contiguous square of non-occupied cells
            var qualifiedRegions = _grid.ScanRectanglarRegions(width, height, location =>
            {
                return map[location.Column, location.Row] != null &&
                      !excludedLocations.Any(otherLocation => otherLocation.Equals(location));
            });

            if (qualifiedRegions.Count() == 0)
                return null;

            // Select random region from the qualified regions
            var region = randomSequenceGenerator.GetRandomElement(qualifiedRegions);

            // Convert the region to grid locations
            var result = new Region<GridLocation>(region.Locations.Select(cell => cell.Location).ToArray(),
                                                  region.EdgeLocations.Select(cell => cell.Location).ToArray(),
                                                  region.Boundary,
                                                  new RegionBoundary(0, 0, this.Bounds.Width, this.Bounds.Height));

            return result;
        }

        /// <summary>
        /// Returns a location that is near the edge of the specified layer map
        /// </summary>
        public GridLocation GetNonOccupiedEdgeLocation(LayoutLayer layer, IRandomSequenceGenerator randomSequenceGenerator, IEnumerable<GridLocation> excludedLocations)
        {
            // Have to create a PDF based on edge distance - with a peak at SIMULATION_EDGE_NEARNESS (in the dimension of the
            // closest edge). Then, use this PDF to create weighted random draw over the space of non-occupied locations.

            var map = SelectLayer(layer);

            var edgePdf = GetDistancePdf(map.GetNonOccupiedLocations().Except(excludedLocations), map.Boundary);

            return randomSequenceGenerator.GetWeightedRandom(edgePdf.Keys, location => edgePdf[location]);
        }

        /// <summary>
        /// Returns a location that is "maximally distant" from the specified source location - using the sepcified map
        /// </summary>
        public GridLocation GetNonOccupiedDistantLocation(LayoutLayer layer, GridLocation sourceLocation, IRandomSequenceGenerator randomSequenceGenerator, IEnumerable<GridLocation> excludedLocations)
        {
            // Have to create a PDF based on distance from a source location - with a peak at SIMULATION_DISTANT_PARAMETER (in distance). 
            // Then, use this PDF to create weighted random draw over the space of non-occupied locations.

            var map = SelectLayer(layer);

            var edgePdf = GetDistancePdf(map.GetNonOccupiedLocations().Except(excludedLocations), new GridLocation[] { sourceLocation });

            return randomSequenceGenerator.GetWeightedRandom(edgePdf.Keys, location => edgePdf[location]);
        }

        /// <summary>
        /// Returns a location that is "maximally distant" from the specified source locations - using the specified layer map
        /// </summary>
        public GridLocation GetNonOccupiedDistantLocations(LayoutLayer layer, IEnumerable<GridLocation> sourceLocations, IRandomSequenceGenerator randomSequenceGenerator, IEnumerable<GridLocation> excludedLocations)
        {
            // Have to create a PDF based on distance from a source location - with a peak at SIMULATION_DISTANT_PARAMETER (in distance). 
            // Then, use this PDF to create weighted random draw over the space of non-occupied locations.

            var map = SelectLayer(layer);

            var edgePdf = GetDistancePdf(map.GetNonOccupiedLocations().Except(excludedLocations), sourceLocations);

            return randomSequenceGenerator.GetWeightedRandom(edgePdf.Keys, location => edgePdf[location]);
        }

        /// <summary>
        /// Selects the proper layer for the specified enumeration
        /// </summary>
        private LayerMap SelectLayer(LayoutLayer layer)
        {
            switch (layer)
            {
                case LayoutLayer.Full:
                    return this.FullMap;
                case LayoutLayer.Connection:
                    return this.ConnectionMap;
                case LayoutLayer.Walkable:
                    return this.WalkableMap;
                case LayoutLayer.Placement:
                    return this.PlacementMap;
                case LayoutLayer.Room:
                    return this.RoomMap;
                case LayoutLayer.Corridor:
                    return this.ConnectionMap;
                case LayoutLayer.Door:
                    return this.DoorMap;
                case LayoutLayer.Wall:
                    return this.WallMap;
                case LayoutLayer.WallLight:
                    return this.WallLightMap;
                case LayoutLayer.ImpassableTerrain:
                    return this.ImpassableTerrainMap;
                default:
                    throw new Exception("Unhandled LayoutLayer GetNonOccupiedLocation");
            }
        }

        /// <summary>
        /// Generates a distance pdf to simulate a "Nearness" draw for edge locations based on the provided location set and
        /// the boundary.
        /// </summary>
        private Dictionary<GridLocation, double> GetDistancePdf(IEnumerable<GridLocation> locations, RegionBoundary boundary)
        {
            var meanLeft = boundary.Left + (SIMULATION_EDGE_NEARNESS * boundary.Width);
            var meanRight = boundary.Right - (SIMULATION_EDGE_NEARNESS * boundary.Width);
            var meanTop = boundary.Top + (SIMULATION_EDGE_NEARNESS * boundary.Height);
            var meanBottom = boundary.Bottom - (SIMULATION_EDGE_NEARNESS * boundary.Height);
            var center = boundary.GetCenter();

            return locations.ToDictionary(location => location, location =>
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
        private Dictionary<GridLocation, double> GetDistancePdf(IEnumerable<GridLocation> locations, IEnumerable<GridLocation> sourceLocations)
        {
            // Want a PDF based on max distance from the source location
            //
            var maxDistance = locations.Max(location => sourceLocations.Max(source => Metric.EuclideanDistance(location, source)));

            return locations.ToDictionary(location => location, location =>
            {
                // Select the minimum distance between this location and each of the source locations as a weight
                var minDistance = sourceLocations.Min(source => Metric.EuclideanDistance(location, source));

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
