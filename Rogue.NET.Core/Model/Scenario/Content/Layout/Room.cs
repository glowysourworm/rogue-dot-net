using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Rogue.NET.Core.Model.Scenario.Content.Layout
{
    [Serializable]
    public class Room : ISerializable
    {
        // Internally, a room is a 2D grid the size of the LevelGrid - making access simple. It only
        // contains data for those cells in the room.
        Cell[,] _grid;

        // Maintains cells as indexable array. This is re-created when it becomes invalid
        Cell[] _cellArray;

        // Maintains edge cells as indexable array. This is re-created when it becomes invalid
        Cell[] _edgeCellArray;

        // Maintains bounding rectangle for the room
        CellRectangle _bounds;
        
        /// <summary>
        /// Accessor to get cell at (absolute) point relative to parent grid.
        /// </summary>
        public Cell this[int column, int row]
        {
            get { return _grid[column, row]; }
        }

        public void SetCell(LevelGrid parentGrid, Cell cell)
        {
            _grid[cell.Location.Column, cell.Location.Row] = cell;

            // Invalidate the cell arrays and bounding rectangle because they needs to be re-created
            _cellArray = null;
            _edgeCellArray = null;
            _bounds = null;
        }
        public Room(int gridWidth, int gridHeight)
        {
            _grid = new Cell[gridWidth, gridHeight];
        }
        public Room(int gridWidth, int gridHeight, IEnumerable<Cell> connectedCells)
        {
            _grid = new Cell[gridWidth, gridHeight];

            // Set up the room grid with appropriate cells
            foreach (var cell in connectedCells)
                _grid[cell.Location.Column, cell.Location.Row] = cell;

            // Search for edges to initialize edge array
            InitializeEdges();

            // Room cells are already found - so initialize array here
            _cellArray = connectedCells.ToArray();
        }
        public Room(int gridWidth, int gridHeight, Room room)
        {
            _grid = new Cell[gridWidth, gridHeight];

            foreach (var cell in room.GetCells())
                _grid[cell.Location.Column, cell.Location.Row] = cell;

            // Search for edges to initialize edge array
            InitializeEdges();

            // Room cells are already found - so initialize array here
            _cellArray = room.GetCells();
        }
        public Room(SerializationInfo info, StreamingContext context)
        {            
            var width = info.GetInt32("GridWidth");
            var height = info.GetInt32("GridHeight");
            var count = info.GetInt32("Count");

            _grid = new Cell[width, height];

            var cells = new List<Cell>(count);

            // Populate cells
            for (int i = 0; i < count; i++)
            {
                var cell = (Cell)info.GetValue("Cell" + i.ToString(), typeof(Cell));

                _grid[cell.Location.Column, cell.Location.Row] = cell;

                cells.Add(cell);
            }

            // Search for edges to initialize edge array
            InitializeEdges();

            // Room cells are already found - so initialize array here
            _cellArray = cells.ToArray();
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            var cells = GetCells();

            info.AddValue("GridWidth", _grid.GetLength(0));
            info.AddValue("GridHeight", _grid.GetLength(1));
            info.AddValue("Count", cells.Length);

            for (int i = 0; i < cells.Length; i++)
                info.AddValue("Cell" + i.ToString(), cells[i]);
        }

        /// <summary>
        /// Sets references to cells from parent grid. 
        /// NOTE*** This is required because of custom serialization - will copy cell data instead of references.
        /// TODO: Find a way around this problem...
        /// </summary>
        public void Initialize(LevelGrid parentGrid)
        {
            var bounds = parentGrid.Bounds;
            var cells = new List<Cell>();
            var edgeCells = new List<Cell>();

            var top = int.MaxValue;
            var bottom = 0;
            var right = 0;
            var left = int.MaxValue;

            // Copy references from parent grid where this room contains cells that overlap. Also,
            // searches for edges and bounding rectangle during the loop to avoid re-iterating
            for (int i = 0; i < bounds.CellWidth; i++)
            {
                for (int j = 0; j < bounds.CellHeight; j++)
                {
                    // Cell is non-null. Add to the cell array and match references
                    if (_grid[i, j] != null)
                    {
                        // Set reference
                        _grid[i, j] = parentGrid[i, j];

                        // Add to room cells
                        cells.Add(_grid[i, j]);

                        // Check bounding box
                        if (i < left)   left = i;
                        if (i > right)  right = i;
                        if (j < top)    top = j;
                        if (j > bottom) bottom = j;
                    }

                    // Edge of grid
                    if (i == 0 ||
                        j == 0 ||
                        i == _grid.GetLength(0) - 1 ||
                        j == _grid.GetLength(1) - 1)
                        edgeCells.Add(_grid[i, j]);

                    // Adjacent cell is null
                    else if (_grid[i - 1, j - 1] == null ||
                             _grid[i - 1, j]     == null ||
                             _grid[i - 1, j + 1] == null ||
                             _grid[i, j - 1]     == null ||
                             _grid[i, j + 1]     == null ||
                             _grid[i + 1, j - 1] == null ||
                             _grid[i + 1, j]     == null ||
                             _grid[i + 1, j + 1] == null)
                             edgeCells.Add(_grid[i, j]);
                }
            }

            _cellArray = cells.ToArray();
            _edgeCellArray = edgeCells.ToArray();
            _bounds = new CellRectangle(left, top, right, bottom);
        }

        private void InitializeEdges()
        {
            var edgeCells = new List<Cell>();

            var top = int.MaxValue;
            var bottom = 0;
            var right = 0;
            var left = int.MaxValue;

            for (int i = 0; i < _grid.GetLength(0); i++)
            {
                for (int j = 0; j < _grid.GetLength(1); j++)
                {
                    if (_grid[i, j] == null)
                        continue;

                    // Edge of grid
                    else if (i == 0 ||
                             j == 0 ||
                             i == _grid.GetLength(0) - 1 ||
                             j == _grid.GetLength(1) - 1)
                        edgeCells.Add(_grid[i, j]);

                    // Adjacent cell is null
                    else if (_grid[i - 1, j - 1] == null ||
                             _grid[i - 1, j]     == null ||
                             _grid[i - 1, j + 1] == null ||
                             _grid[i, j - 1]     == null ||
                             _grid[i, j + 1]     == null ||
                             _grid[i + 1, j - 1] == null ||
                             _grid[i + 1, j]     == null ||
                             _grid[i + 1, j + 1] == null)
                        edgeCells.Add(_grid[i, j]);

                    // Check bounding box
                    if (i < left) left = i;
                    if (i > right) right = i;
                    if (j < top) top = j;
                    if (j > bottom) bottom = j;
                }
            }

            _edgeCellArray = edgeCells.ToArray();
            _bounds = new CellRectangle(left, top, right, bottom);
        }
        private void InitializeArrays()
        {
            var cells = new List<Cell>();
            var edgeCells = new List<Cell>();

            var top = int.MaxValue;
            var bottom = 0;
            var right = 0;
            var left = int.MaxValue;

            for (int i = 0; i < _grid.GetLength(0); i++)
            {
                for (int j = 0; j < _grid.GetLength(1); j++)
                {
                    // Found cell in the room
                    if (_grid[i, j] != null)
                    {
                        // Add cell to room
                        cells.Add(_grid[i, j]);

                        // Check bounding box
                        if (i < left) left = i;
                        if (i > right) right = i;
                        if (j < top) top = j;
                        if (j > bottom) bottom = j;
                    }

                    // Found edge of grid
                    else if (i == 0 ||
                             j == 0 ||
                             i == _grid.GetLength(0) - 1 ||
                             j == _grid.GetLength(1) - 1)
                        edgeCells.Add(_grid[i, j]);

                    // Adjacent cell is null - edge of room
                    else if (_grid[i - 1, j - 1] == null ||
                             _grid[i - 1, j] == null ||
                             _grid[i - 1, j + 1] == null ||
                             _grid[i, j - 1] == null ||
                             _grid[i, j + 1] == null ||
                             _grid[i + 1, j - 1] == null ||
                             _grid[i + 1, j] == null ||
                             _grid[i + 1, j + 1] == null)
                             edgeCells.Add(_grid[i, j]);
                }
            }

            _cellArray = cells.ToArray();
            _edgeCellArray = edgeCells.ToArray();
            _bounds = new CellRectangle(left, top, right, bottom);
        }

        /// <summary>
        /// Inefficient (?) use of ToArray()
        /// </summary>
        public Cell[] GetCells()
        {
            // Adding a catch here because trying to avoid iterating during creation cycle
            if (_cellArray == null)
                InitializeArrays();

            return _cellArray;
        }
        public Cell[] GetEdges()
        {
            // Adding a catch here because trying to avoid iterating during creation cycle
            if (_cellArray == null)
                InitializeArrays();

            return _edgeCellArray;
        }
        public CellRectangle GetBounds()
        {
            // Adding a catch here because trying to avoid iterating during creation cycle
            if (_cellArray == null)
                InitializeArrays();

            return _bounds;
        }
    }
}
