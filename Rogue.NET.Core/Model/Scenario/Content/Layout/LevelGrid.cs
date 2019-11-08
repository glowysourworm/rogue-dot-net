using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Processing.Model.Extension;

namespace Rogue.NET.Core.Model.Scenario.Content.Layout
{

    [Serializable]
    public class LevelGrid : ISerializable
    {
        private GridCell[,] _grid;
        private RegionBoundary _bounds;

        private GridCell[] _doorArray;
        private GridCell[] _cellArray;
        private RegionMap _roomMap;
        private RegionMap _terrainMap;

        #region Properties / Indexers
        public GridCell this[int column, int row]
        {
            //NOTE*** Returns null as a convention
            get
            {
                if (column >= 0 &&
                    row >= 0 &&
                    column < _grid.GetLength(0) &&
                    row < _grid.GetLength(1))
                    return _grid[column, row];

                return null;
            }
            //set
            //{
            //    // Allow out-of-bounds exceptions
            //    _grid[column, row] = value;

            //    // Invalidate cell arrays
            //    _cellArray = null;
            //    _doorArray = null;
            //}
        }
        public RegionBoundary Bounds
        {
            get { return _bounds; }
        }
        public RegionMap RoomMap
        {
            get { return _roomMap; }
        }
        public RegionMap TerrainMap
        {
            get { return _terrainMap; }
        }
        #endregion

        /// <summary>
        /// Constructs LevelGrid from the provided 2D cell arrays and the region arrays. The cells
        /// in the cell array are by reference; and are not re-created. The region data arrays contain
        /// cell points that are treated as a value type. These are recreated during serialization (not
        /// unique) apart from the cell reference objects. The terrain array follows the same pattern.
        /// 
        /// USAGE:  Create Cell[,] first with room cells already in it. Also, create Room[] first with
        ///         all data prepared. Also, create the terrain array with all data prepared. Corridors 
        ///         may be created afterwards using the public indexer.
        /// </summary>
        public LevelGrid(GridCellInfo[,] grid, Region[] roomRegions, Region[] terrainRegions)
        {
            _grid = new GridCell[grid.GetLength(0), grid.GetLength(1)];
            _bounds = new RegionBoundary(new GridLocation(0, 0), grid.GetLength(0), grid.GetLength(1));
            _roomMap = new RegionMap(roomRegions, _bounds.CellWidth, _bounds.CellHeight);
            _terrainMap = new RegionMap(terrainRegions, _bounds.CellWidth, _bounds.CellHeight);

            // Initialize the grid
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    if (grid[i, j] != null)
                        _grid[i, j] = new GridCell(i, j, grid[i, j].IsWall, grid[i, j].IsDoor, grid[i, j].DoorSearchCounter, grid[i, j].IsCorridor);
                }
            }
        }

        #region ISerializable
        public LevelGrid(SerializationInfo info, StreamingContext context)
        {
            var width = info.GetInt32("Width");
            var height = info.GetInt32("Height");
            var count = info.GetInt32("Count");
            var roomCount = info.GetInt32("RoomCount");
            var terrainCount = info.GetInt32("TerrainCount");

            _grid = new GridCell[width, height];
            _bounds = new RegionBoundary(new GridLocation(0, 0), width, height);

            var roomData = new List<Region>();
            var terrainData = new List<Region>();

            // Populate cell grid
            for (int i=0;i<count;i++)
            {
                var cell = (GridCell)info.GetValue("Cell" + i.ToString(), typeof(GridCell));

                _grid[cell.Location.Column, cell.Location.Row] = cell;
            }

            // Populate rooms
            for (int i = 0; i < roomCount; i++)
            {
                var room = (Region)info.GetValue("Room" + i.ToString(), typeof(Region));

                roomData.Add(room);
            }

            // Populate terrain
            for (int i = 0; i < terrainCount; i++)
            {
                var terrain = (Region)info.GetValue("Terrain" + i.ToString(), typeof(Region));

                terrainData.Add(terrain);
            }

            _roomMap = new RegionMap(roomData, width, height);
            _terrainMap = new RegionMap(terrainData, width, height);

            // Leave these invalid until iteration is necessary
            _doorArray = null;
            _cellArray = null;
        }
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // Have to use collections to serialize data
            if (_cellArray == null)
                RebuildArrays();

            var rooms = _roomMap.GetRegions().ToArray();
            var terrain = _terrainMap.GetRegions().ToArray();

            info.AddValue("Width", _grid.GetLength(0));
            info.AddValue("Height", _grid.GetLength(1));
            info.AddValue("Count", _cellArray.Length);
            info.AddValue("RoomCount", rooms.Length);
            info.AddValue("TerrainCount", terrain.Length);

            for (int i = 0; i < _cellArray.Length; i++)
                info.AddValue("Cell" + i.ToString(), _cellArray[i]);

            for (int i = 0; i < rooms.Length; i++)
                info.AddValue("Room" + i.ToString(), rooms[i]);

            for (int i = 0; i < terrain.Length; i++)
                info.AddValue("Terrain" + i.ToString(), terrain[i]);
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

            for (int i=0;i<_grid.GetLength(0);i++)
            {
                for (int j=0;j<_grid.GetLength(1);j++)
                {
                    if (_grid[i, j] == null)
                        continue;

                    cells.Add(_grid[i, j]);

                    if (_grid[i, j].IsDoor)
                        doorCells.Add(_grid[i, j]);
                }
            }

            _cellArray = cells.ToArray();
            _doorArray = doorCells.ToArray();
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
