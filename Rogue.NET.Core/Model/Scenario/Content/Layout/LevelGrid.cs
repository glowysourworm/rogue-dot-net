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
        private CellRectangle[,] _roomGrid;

        private IList<Cell> _doors;
        private IList<Cell> _cells;
        private IList<CellRectangle> _rooms;

        private Cell[] _doorArray;
        private Cell[] _cellArray;
        private CellRectangle[] _roomArray;

        public LevelGrid(int width, int height, int roomWidth, int roomHeight)
        {
            _grid = new Cell[width, height];
            _roomGrid = new CellRectangle[roomWidth, roomHeight];
            _doors = new List<Cell>();
            _cells = new List<Cell>();
            _rooms = new List<CellRectangle>();

            RebuildArrays();
        }

        #region ISerializable
        public LevelGrid(SerializationInfo info, StreamingContext context)
        {
            var width = info.GetInt32("Width");
            var height = info.GetInt32("Height");
            var count = info.GetInt32("Count");
            var roomWidth = info.GetInt32("RoomWidth");
            var roomHeight = info.GetInt32("RoomHeight");

            _grid = new Cell[width, height];
            _roomGrid = new CellRectangle[roomWidth, roomHeight];
            _cells = new List<Cell>();
            _doors = new List<Cell>();
            _rooms = new List<CellRectangle>();

            // Populate cell grid
            for (int i=0;i<count;i++)
            {
                var cell = (Cell)info.GetValue("Cell" + i.ToString(), typeof(Cell));

                _grid[cell.Location.Column, cell.Location.Row] = cell;

                _cells.Add(cell);

                if (cell.IsDoor)
                    _doors.Add(cell);
            }

            // Populate room grid
            for (int i=0;i<roomWidth;i++)
            {
                for (int j=0; j<roomHeight;j++)
                {
                    var room = (CellRectangle)info.GetValue(string.Join(":", "Room", i, j), typeof(CellRectangle));

                    // New Room
                    if (!_rooms.Any(x => x.Equals(room)))
                    {
                        _roomGrid[i, j] = room;
                        _rooms.Add(room);
                    }

                    // Duplicate Room (used for BigRoom layout type)
                    else
                    {
                        var existingRoom = _rooms.First(x => x.Equals(room));
                        var found = false;

                        for (int n = 0;n < _roomGrid.GetLength(0) && !found;n++)
                        {
                            for (int m = 0;m < _roomGrid.GetLength(1) && !found;m++)
                            {
                                if (_roomGrid[n, m] == existingRoom)
                                {
                                    _roomGrid[i, j] = existingRoom;
                                    found = true;
                                }
                            }
                        }
                    }
                }
            }

            RebuildArrays();
        }
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Width", _grid.GetLength(0));
            info.AddValue("Height", _grid.GetLength(1));
            info.AddValue("Count", _cells.Count);
            info.AddValue("RoomWidth", _roomGrid.GetLength(0));
            info.AddValue("RoomHeight", _roomGrid.GetLength(1));

            for (int i = 0; i < _cells.Count; i++)
                info.AddValue("Cell" + i.ToString(), _cells[i]);

            for (int i=0;i<_roomGrid.GetLength(0);i++)
            {
                for (int j=0;j<_roomGrid.GetLength(1);j++)
                    info.AddValue(string.Join(":", "Room", i, j), _roomGrid[i, j]);
            }
        }
        #endregion

        public Cell this[int column, int row]
        {
            get { return GetCell(column, row); }
            set { AddCell(value); }
        }
        public void AddCell(Cell cell)
        {
            if (cell.Location.Column < 0 || cell.Location.Column >= _grid.GetLength(0))
                throw new Exception("Trying to add cell to out of range slot");

            if (cell.Location.Row < 0 || cell.Location.Row >= _grid.GetLength(1))
                throw new Exception("Trying to add cell to out of range slot");

            if (_grid[cell.Location.Column, cell.Location.Row] != null)
                return;

            _grid[cell.Location.Column, cell.Location.Row] = cell;

            _cells.Add(cell);

            if (cell.IsDoor)
                _doors.Insert(0, cell);

            RebuildArrays();
        }
        public Cell GetCell(int x, int y)
        {
            if (x < 0 || x >= this.GetBounds().Right)
                return null;

            if (y < 0 || y >= this.GetBounds().Bottom)
                return null;

            return _grid[x, y];
        }
        public Cell GetCell(CellPoint cp)
        {
            return GetCell(cp.Column, cp.Row);
        }

        /// <summary>
        /// Returns 1st of 2 off diagonal cells in the specified non-cardinal direction (Exapmle: NE -> N cell)
        /// </summary>
        /// <param name="direction">NE, NW, SE, SW</param>
        public Cell GetOffDiagonalCell1(CellPoint location, Compass direction, out Compass cardinalDirection1)
        {
            switch (direction)
            {
                case Compass.NE:
                case Compass.NW:
                    cardinalDirection1 = Compass.N;
                    return GetCell(location.Column, location.Row - 1);
                case Compass.SE:
                case Compass.SW:
                    cardinalDirection1 = Compass.S;
                    return GetCell(location.Column, location.Row + 1);
                default:
                    throw new Exception("Off-Diagonal directions don't include " + direction.ToString());
            }
        }

        /// <summary>
        /// Returns 2nd of 2 off diagonal cells in the specified non-cardinal direction (Exapmle: NE -> E cell)
        /// </summary>
        /// <param name="direction">NE, NW, SE, SW</param>
        public Cell GetOffDiagonalCell2(CellPoint location, Compass direction, out Compass cardinalDirection2)
        {
            switch (direction)
            {
                case Compass.NE:
                case Compass.SE:
                    cardinalDirection2 = Compass.E;
                    return GetCell(location.Column + 1, location.Row);
                case Compass.SW:
                case Compass.NW:
                    cardinalDirection2 = Compass.W;
                    return GetCell(location.Column - 1, location.Row);
                default:
                    throw new Exception("Off-Diagonal directions don't include " + direction.ToString());
            }
        }
        public CellRectangle GetRoom(int column, int row)
        {
            return _roomGrid[column, row];
        }
        public int GetRoomGridWidth()
        {
            return _roomGrid.GetLength(0);
        }
        public int GetRoomGridHeight()
        {
            return _roomGrid.GetLength(1);
        }

        public void SetRoom(CellRectangle cellRectangle, int roomColumn, int roomRow)
        {
            _roomGrid[roomColumn, roomRow] = cellRectangle;

            // Rebuild list (** NO DUPLICATES)
            _rooms.Clear();
            for (int i = 0; i < _roomGrid.GetLength(0); i++)
            {
                for (int j = 0; j < _roomGrid.GetLength(1); j++)
                {
                    if (!_rooms.Contains(_roomGrid[i, j]))
                        _rooms.Add(_roomGrid[i, j]);
                }
            }

            RebuildArrays();
        }

        public CellRectangle GetBounds()
        {
            return new CellRectangle(new CellPoint(0, 0), _grid.GetLength(0), _grid.GetLength(1));
        }


        /// <summary>
        /// For efficiency, maintain arrays of elements in arrays for calling methods. NOTE*** If this is a performance
        /// problem during the generation process then will have to defer building these until it's completed.
        /// </summary>
        private void RebuildArrays()
        {
            _cellArray = _cells.ToArray();
            _doorArray = _doors.ToArray();
            _roomArray = _rooms.ToArray();
        }
        public Cell[] GetDoors()
        {
            return _doorArray;
        }
        public CellRectangle[] GetRooms()
        {
            return _roomArray;
        }
        public Cell[] GetCells()
        {
            return _cellArray;
        }
    }
}
