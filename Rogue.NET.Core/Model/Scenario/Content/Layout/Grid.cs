using Rogue.NET.Common.Serialization.Interface;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content.Layout.Interface;
using Rogue.NET.Core.Processing.Model.Algorithm;
using Rogue.NET.Core.Processing.Model.Extension;

using System;
using System.Collections.Generic;
using System.Linq;

using static Rogue.NET.Core.Processing.Model.Extension.ArrayExtension;

namespace Rogue.NET.Core.Model.Scenario.Content.Layout
{
    /// <summary>
    /// 2D array component built on a parent 2D array with minimized storage and offset capability. Does 
    /// automatic index math using its indexer; and serializes offsets from the parent 2D array. Also, 
    /// serializes the data.
    /// </summary>
    [Serializable]
    public class Grid<T> : IRecursiveSerializable
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
        /// Returns the region boundary for the sub-grid that this Grid instance represents
        /// </summary>
        public RegionBoundary GetBoundary()
        {
            return new RegionBoundary(_offsetColumn, _offsetRow, _width, _height);
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

        public Grid(IPropertyReader reader)
        {
            _offsetColumn = reader.Read<int>("OffsetColumn");
            _offsetRow = reader.Read<int>("OffsetRow");
            _width = reader.Read<int>("Width");
            _height = reader.Read<int>("Height");
            _parentWidth = reader.Read<int>("ParentWidth");
            _parentHeight = reader.Read<int>("ParentHeight");
            _grid = new T[_width, _height];

            var count = reader.Read<int>("Count");

            for (int i = 0; i < count; i++)
            {
                var item = reader.Read<T>("Item" + i);
                var location = reader.Read<GridLocation>("ItemLocation" + i);

                _grid[location.Column, location.Row] = item;
            }
        }

        public void GetProperties(IPropertyWriter writer)
        {
            writer.Write("OffsetColumn", _offsetColumn);
            writer.Write("OffsetRow", _offsetRow);
            writer.Write("Width", _width);
            writer.Write("Height", _height);
            writer.Write("ParentWidth", _parentWidth);
            writer.Write("ParentHeight", _parentHeight);

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
                if (ReferenceEquals(_grid[column, row], null))
                    return;

                dict.Add(new GridLocation(column, row), _grid[column, row]);
            });

            // Serialize the item data
            writer.Write("Count", dict.Count);

            var counter = 0;

            foreach (var element in dict)
            {
                writer.Write("Item" + counter, element.Value);
                writer.Write("ItemLocation" + counter++, element.Key);
            }
        }

        // Array Extension
        public IEnumerable<T> GetAdjacentElements(int column, int row)
        {
            return _grid.GetAdjacentElements(column, row);
        }
        public IEnumerable<T> GetCardinalAdjacentElements(int column, int row)
        {
            return _grid.GetCardinalAdjacentElements(column, row);
        }
        public T GetOffDiagonalElement1(int column, int row, Compass direction, out Compass cardinalDirection1)
        {
            return _grid.GetOffDiagonalElement1(column, row, direction, out cardinalDirection1);
        }
        public T GetOffDiagonalElement2(int column, int row, Compass direction, out Compass cardinalDirection2)
        {
            return _grid.GetOffDiagonalElement2(column, row, direction, out cardinalDirection2);
        }
        public void IterateAround(int column, int row, int range, GridPredicate<T> continuationPredicate)
        {
            _grid.IterateAround(column, row, range, continuationPredicate);
        }


        // Grid Algorithm (T : class, IGridLocator)
        public IEnumerable<Region<TRegion>> ScanRectangularRegions<TRegion>(int rectangleWidth, int rectangleHeight, Func<T, bool> predicate, Func<T, TRegion> selector) where TRegion : class, IGridLocator
        {
            return _grid.ScanRectanglarRegions(rectangleWidth, rectangleHeight, predicate, selector);
        }
    }
}
