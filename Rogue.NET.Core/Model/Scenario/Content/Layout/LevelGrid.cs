using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;
using Rogue.NET.Core.Model.Enums;

namespace Rogue.NET.Core.Model.Scenario.Content.Layout
{

    [Serializable]
    public class LevelGrid : ISerializable
    {
        private Cell[,] _grid;
        private CellRectangle _bounds;

        private Cell[] _doorArray;
        private Cell[] _cellArray;
        private Room[] _roomArray;

        #region Properties / Indexers
        public Cell this[int column, int row]
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
            set
            {
                // Allow out-of-bounds exceptions
                _grid[column, row] = value;

                // Invalidate cell arrays
                _cellArray = null;
                _doorArray = null;
            }
        }
        public CellRectangle Bounds
        {
            get { return _bounds; }
        }
        public IEnumerable<Room> Rooms { get { return _roomArray; } }
        #endregion

        /// <summary>
        /// Constructs LevelGrid from the provided 2D cell array and the room data array. The cells
        /// in the cell array are by reference; and are not re-created. The room data array contains
        /// cell points that are treated as a value type. These are recreated during serialization (not
        /// unique) apart from the cell reference objects.
        /// 
        /// USAGE:  Create Cell[,] first with room cells already in it. Also, create Room[] first with
        ///         all data prepared. Corridors may be created afterwards using the public indexer.
        /// </summary>
        public LevelGrid(Cell[,] grid, Room[] rooms)
        {
            _grid = grid;
            _bounds = new CellRectangle(new CellPoint(0, 0), grid.GetLength(0), grid.GetLength(1));
            _roomArray = rooms;
        }

        #region ISerializable
        public LevelGrid(SerializationInfo info, StreamingContext context)
        {
            var width = info.GetInt32("Width");
            var height = info.GetInt32("Height");
            var count = info.GetInt32("Count");
            var roomCount = info.GetInt32("RoomCount");

            _grid = new Cell[width, height];
            _bounds = new CellRectangle(new CellPoint(0, 0), width, height);

            var roomData = new List<Room>();

            // Populate cell grid
            for (int i=0;i<count;i++)
            {
                var cell = (Cell)info.GetValue("Cell" + i.ToString(), typeof(Cell));

                _grid[cell.Location.Column, cell.Location.Row] = cell;
            }

            // Populate rooms
            for (int i = 0; i < roomCount; i++)
            {
                var room = (Room)info.GetValue("Room" + i.ToString(), typeof(Room));

                roomData.Add(room);
            }

            _roomArray = roomData.ToArray();

            // Leave these invalid until iteration is necessary
            _doorArray = null;
            _cellArray = null;
        }
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // Have to use collections to serialize data
            if (_cellArray == null)
                RebuildArrays();

            info.AddValue("Width", _grid.GetLength(0));
            info.AddValue("Height", _grid.GetLength(1));
            info.AddValue("Count", _cellArray.Length);
            info.AddValue("RoomCount", _roomArray.Length);

            for (int i = 0; i < _cellArray.Length; i++)
                info.AddValue("Cell" + i.ToString(), _cellArray[i]);

            for (int i = 0; i < _roomArray.Length; i++)
                info.AddValue("Room" + i.ToString(), _roomArray[i]);
        }
        #endregion

        #region Array Access
        /// <summary>
        /// For efficiency, maintain arrays of elements in arrays for calling methods. NOTE*** If this is a performance
        /// problem during the generation process then will have to defer building these until it's completed.
        /// </summary>
        private void RebuildArrays()
        {
            var cells = new List<Cell>();
            var doorCells = new List<Cell>();

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
        public Cell[] GetDoors()
        {
            if (_cellArray == null)
                RebuildArrays();

            return _doorArray;
        }
        public Cell[] GetCells()
        {
            if (_cellArray == null)
                RebuildArrays();

            return _cellArray;
        }
        #endregion
    }
}
