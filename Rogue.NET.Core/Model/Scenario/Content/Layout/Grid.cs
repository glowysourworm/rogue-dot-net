using Rogue.NET.Core.Processing.Model.Extension;

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Rogue.NET.Core.Model.Scenario.Content.Layout
{
    /// <summary>
    /// 2D array component built on a parent 2D array with minimized storage and offset capability. Does 
    /// automatic index math using its indexer; and serializes offsets from the parent 2D array. Also, 
    /// serializes the data.
    /// </summary>
    [Serializable]
    public class Grid<T> : ISerializable, IDeserializationCallback
    {
        readonly T[,] _grid;
        readonly int _offsetColumn;
        readonly int _offsetRow;
        readonly int _width;
        readonly int _height;
        readonly int _parentWidth;
        readonly int _parentHeight;

        /// <summary>
        /// Indexer to the 2D array based on the PARENT COLUMN AND ROW. Allows indexing over 
        /// full parent index space - but returns default(T) for indices out of bounds of the
        /// grid. For setting, throws an exception if the parent indexer is outside the bounds
        /// of the grid.
        /// </summary>
        public T this[int parentColumn, int parentRow]
        {
            get
            {
                if (parentColumn < 0 ||
                    parentColumn >= _parentWidth ||
                    parentRow < 0 ||
                    parentRow >= _parentHeight)
                    throw new Exception("Trying to index outside the PARENT boundary:  Grid.this[]");

                var column = parentColumn - _offsetColumn;
                var row = parentRow - _offsetRow;

                if (column < 0 ||
                    column >= _width ||
                    row < 0 ||
                    row >= _height)
                    return default(T);

                return _grid[column, row];
            }
            set
            {
                var column = parentColumn - _offsetColumn;
                var row = parentRow - _offsetRow;

                if (column < 0 ||
                    column >= _width ||
                    row < 0 ||
                    row >= _height)
                    throw new Exception("Trying to set Grid<> outside of its bounds");

                _grid[column, row] = value;
            }
        }

        /// <summary>
        /// Returns true if the grid is defined in the provided PARENT indices
        /// </summary>
        public bool IsDefined(int parentColumn, int parentRow)
        {
            if (parentColumn < 0 ||
                parentColumn >= _parentWidth ||
                parentRow < 0 ||
                parentRow >= _parentHeight)
                return false;

            return true;
        }

        /// <summary>
        /// Sets value for the grid at the provided parent coordinates. Throws an exception if
        /// the indicies are outside the bounds of the the grid's boundary.
        /// </summary>
        public void Set(int parentColumn, int parentRow, T value)
        {
            this[parentColumn, parentRow] = value;
        }

        public Grid(RegionBoundary parentBoundary, RegionBoundary boundary)
        {
            _offsetColumn = boundary.Left;
            _offsetRow = boundary.Top;
            _width = boundary.Width;
            _height = boundary.Height;
            _parentWidth = parentBoundary.Width;
            _parentHeight = parentBoundary.Height;

            _grid = new T[_width, _height];
        }

        public Grid(SerializationInfo info, StreamingContext context)
        {
            _offsetColumn = info.GetInt32("OffsetColumn");
            _offsetRow = info.GetInt32("OffsetRow");
            _width = info.GetInt32("Width");
            _height = info.GetInt32("Height");
            _parentWidth = info.GetInt32("ParentWidth");
            _parentHeight = info.GetInt32("ParentHeight");
            _grid = new T[_width, _height];

            var count = info.GetInt32("Count");

            for (int i = 0; i < count; i++)
            {
                var item = (T)info.GetValue("Item" + i, typeof(T));
                var location = (GridLocation)info.GetValue("ItemLocation" + i, typeof(GridLocation));

                _grid[location.Column, location.Row] = item;
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("OffsetColumn", _offsetColumn);
            info.AddValue("OffsetRow", _offsetRow);
            info.AddValue("Width", _width);
            info.AddValue("Height", _height);
            info.AddValue("ParentWidth", _parentWidth);
            info.AddValue("ParentHeight", _parentHeight);

            // NOTE*** TRYING TO MINIMIZE STORAGE BY GATHERING DATA FROM THE GRID
            //         .. BUT THE TRADEOFF IS THAT THE LOCATION ALSO HAS TO BE STORED.
            //
            //         THIS COULD BE OPTIMIZED BY FIRST CALCULATING WHETHER IT'S MORE
            //         STORAGE EFFICIENT TO STORE LOCATION+ITEM, OR ITEM (WITH THE GRID
            //         SCAN TO LOCATE IT'S COLUMN AND ROW).
            //
            var dict = new Dictionary<GridLocation, T>();

            // Gather the item data from the grid
            _grid.Iterate((column, row) =>
            {
                // T is a reference type -> check for null reference
                if (typeof(T).IsClass)
                {
                    if (_grid[column, row] != null)
                        dict.Add(new GridLocation(column, row), _grid[column, row]);
                }

                // T is a value type -> check for default(T)
                else
                {
                    if (!_grid[column, row].Equals(default(T)))
                        dict.Add(new GridLocation(column, row), _grid[column, row]);
                }
            });

            // Serialize the item data
            info.AddValue("Count", dict.Count);

            var counter = 0;

            foreach (var element in dict)
            {
                info.AddValue("Item" + counter, element.Value);
                info.AddValue("ItemLocation" + counter++, element.Key);
            }
        }

        /// <summary>
        /// Routine used to match parent references from the deserialized parent grid
        /// </summary>
        public void OnDeserialization(object sender)
        {
            var parentGrid = sender as T[,];

            if (parentGrid == null)
                return;

            // Iterate over PARENT grid - transferring data (or) references to THIS grid
            for (int i = _offsetColumn; i < _offsetColumn + _width; i++)
            {
                for (int j = _offsetRow; j < _offsetRow + _height; j++)
                {
                    // Use indexer to set data from parent indices
                    this[i, j] = parentGrid[i, j];
                }
            }
        }
    }
}
