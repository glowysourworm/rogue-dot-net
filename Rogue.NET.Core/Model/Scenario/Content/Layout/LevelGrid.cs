using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Processing.Model.Extension;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Rogue.NET.Core.Model.Scenario.Content.Layout
{

    [Serializable]
    public class LevelGrid : ISerializable
    {
        private GridCell[,] _grid;
        private GridCell[] _doorArray;
        private GridCell[] _cellArray;
        private GridCell[] _walkableArray;
        private GridCell[] _wallLightArray;

        #region Properties / Indexers
        public virtual GridCell this[int column, int row]
        {
            //NOTE*** Returns null as a default
            get { return _grid.Get(column, row); }
        }
        public RegionBoundary Bounds { get; private set; }
        public LayerMap RoomMap { get; private set; }
        public LayerMap CorridorMap { get; private set; }
        public LayerMap ImpassableTerrainMap { get; private set; }
        public IEnumerable<LayerMap> TerrainMaps { get; private set; }
        #endregion

        /// <summary>
        /// Constructs LevelGrid from the provided 2D cell arrays and the regions. The cells
        /// in the cell array are by reference; and are not re-created. The region data arrays contain
        /// cell points that are treated as a value type. These are recreated during serialization (not
        /// unique) apart from the cell reference objects. The terrain array follows the same pattern.
        /// 
        /// USAGE:  Create Cell[,] first with room cells already in it. Also, create Room[] first with
        ///         all data prepared. Also, create the terrain array with all data prepared. Corridors 
        ///         may be created afterwards using the public indexer.
        /// </summary>
        public LevelGrid(GridCellInfo[,] grid, LayerInfo roomLayer, LayerInfo corridorLayer, IEnumerable<LayerInfo> terrainLayers)
        {
            _grid = new GridCell[grid.GetLength(0), grid.GetLength(1)];

            this.Bounds = new RegionBoundary(new GridLocation(0, 0), grid.GetLength(0), grid.GetLength(1));
            this.RoomMap = new LayerMap(roomLayer.LayerName, roomLayer.Regions, this.Bounds.Width, this.Bounds.Height);
            this.CorridorMap = new LayerMap(corridorLayer.LayerName, corridorLayer.Regions, this.Bounds.Width, this.Bounds.Height);
            this.ImpassableTerrainMap = new LayerMap("Impassable Terrain", terrainLayers.Where(x => !x.IsPassable).SelectMany(layer => layer.Regions), this.Bounds.Width, this.Bounds.Height);
            this.TerrainMaps = terrainLayers.Select(layer => new LayerMap(layer.LayerName, layer.Regions, this.Bounds.Width, this.Bounds.Height)).Actualize();

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
        public LevelGrid(SerializationInfo info, StreamingContext context)
        {
            var width = info.GetInt32("Width");
            var height = info.GetInt32("Height");
            var count = info.GetInt32("Count");
            var terrainCount = info.GetInt32("TerrainMapCount");
            var roomMap = (LayerMap)info.GetValue("RoomMap", typeof(LayerMap));
            var corridorMap = (LayerMap)info.GetValue("CorridorMap", typeof(LayerMap));
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

            this.RoomMap = roomMap;
            this.CorridorMap = corridorMap;
            this.TerrainMaps = terrainData;
            this.ImpassableTerrainMap = impassableMap;

            // Leave these invalid until iteration is necessary
            _doorArray = null;
            _cellArray = null;
            _wallLightArray = null;
        }
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // Have to use collections to serialize data
            if (_cellArray == null)
                RebuildArrays();

            info.AddValue("Width", _grid.GetLength(0));
            info.AddValue("Height", _grid.GetLength(1));
            info.AddValue("Count", _cellArray.Length);
            info.AddValue("TerrainMapCount", this.TerrainMaps.Count());
            info.AddValue("RoomMap", this.RoomMap);
            info.AddValue("CorridorMap", this.CorridorMap);
            info.AddValue("ImpassableTerrainMap", this.ImpassableTerrainMap);

            for (int i = 0; i < _cellArray.Length; i++)
                info.AddValue("Cell" + i.ToString(), _cellArray[i]);

            for (int i = 0; i < this.TerrainMaps.Count(); i++)
                info.AddValue("TerrainMap" + i.ToString(), this.TerrainMaps.ElementAt(i));
        }
        #endregion

        #region Array Access
        /// <summary>
        /// For efficiency, maintain arrays of elements in arrays for calling methods. NOTE*** If this is a performance
        /// problem during the generation process then will have to defer building these until it's completed.
        /// </summary>
        private void RebuildArrays()
        {
            var cells = new List<GridCell>();
            var doorCells = new List<GridCell>();
            var walkableCells = new List<GridCell>();
            var wallLightCells = new List<GridCell>();

            for (int i = 0; i < _grid.GetLength(0); i++)
            {
                for (int j = 0; j < _grid.GetLength(1); j++)
                {
                    if (_grid[i, j] == null)
                        continue;

                    cells.Add(_grid[i, j]);

                    if (!_grid[i, j].IsWall &&
                         this.ImpassableTerrainMap[i, j] == null)
                        walkableCells.Add(_grid[i, j]);

                    if (_grid[i, j].IsDoor)
                        doorCells.Add(_grid[i, j]);

                    if (_grid[i, j].IsWallLight)
                        wallLightCells.Add(_grid[i, j]);
                }
            }

            _cellArray = cells.ToArray();
            _doorArray = doorCells.ToArray();
            _walkableArray = walkableCells.ToArray();
            _wallLightArray = wallLightCells.ToArray();
        }
        public GridCell[] GetDoors()
        {
            if (_cellArray == null)
                RebuildArrays();

            return _doorArray;
        }
        public GridCell[] GetCells()
        {
            if (_cellArray == null)
                RebuildArrays();

            return _cellArray;
        }
        public GridCell[] GetWalkableCells()
        {
            if (_cellArray == null)
                RebuildArrays();

            return _walkableArray;
        }
        public GridCell[] GetWallLightCells()
        {
            if (_wallLightArray == null)
                RebuildArrays();

            return _wallLightArray;
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
    }
}
