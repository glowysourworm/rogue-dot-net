using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Math.Algorithm.Interface;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.Scenario.Content.Layout.Interface;

using System;
using System.Collections.Generic;

namespace Rogue.NET.Core.Processing.Model.Generator.Layout.Construction
{
    /// <summary>
    /// (IMMUTABLE) Serializable data structure to store calculated room information. NOTE*** GridLocation is the best choice
    ///             for IMMUTABLE region (see GetHashCode())
    /// </summary>
    public struct RegionInfo<T> : IGraphNode where T : class, IGridLocator
    {
        /// <summary>
        /// Should represent an empty region. Use IsEmpty() to verify
        /// </summary>
        public static RegionInfo<T> Empty = new RegionInfo<T>();

        public string Id { get; private set; }
        public T[] Locations { get; private set; }
        public T[] EdgeLocations { get; private set; }
        public T[] LeftEdgeExposedLocations { get; private set; }
        public T[] RightEdgeExposedLocations { get; private set; }
        public T[] TopEdgeExposedLocations { get; private set; }
        public T[] BottomEdgeExposedLocations { get; private set; }
        public RegionBoundary Boundary { get; private set; }
        public RegionBoundary ParentBoundary { get; private set; }

        // IGraphNode
        public int Hash { get { return GetHashCode(); } }

        // 2D Arrays for region locations and edges - NOT SERIALIZED
        Grid<T> _gridLocations;
        Grid<bool> _edgeLocations;

        // Calcualte the hash value once for comparison
        int _calculatedHash;

        // USE FOR COMPARISON!  (default(RegionInfo<T>).. or region != null or RegionInfo<T>.Empty)
        bool _isInitialized;

        public bool IsEmpty()
        {
            return !_isInitialized;
        }

        public override bool Equals(object obj)
        {
            var otherRegion = (RegionInfo<T>)obj;

            // No way to check for "null" reference to obj that is supposed to be a struct..(?)
            return otherRegion.GetHashCode() == this.GetHashCode();
        }

        public override int GetHashCode()
        {
            if (_calculatedHash == default(int)) 
            {
                _calculatedHash = this.CreateHashCode(this.Id,
                                                      this.Boundary,
                                                      this.ParentBoundary);

                // (PERFORMANCE!!) CREATE GRID<T> HASH CODE USING CALLS FOR SPECIFIC TYPES
                _calculatedHash = ExtendHashCodeForGrid(_gridLocations, _calculatedHash);
            }

            return _calculatedHash;
        }

        // COULD OVERRIDE THE HASH CODE ON THE GRIDCELLINFO - BUT DIDN'T WANT THAT BEHAVIOR
        private int ExtendHashCodeForGrid(Grid<T> grid, int hashToExtend)
        {
            var gridBounds = grid.GetBoundary();

            for (int column = gridBounds.Left; column <= gridBounds.Right; column++)
            {
                for (int row = gridBounds.Top; row <= gridBounds.Bottom; row++)
                {
                    if (grid[column, row] != null)
                    {
                        if (grid[column, row] is GridLocation)
                        {
                            var location = grid[column, row] as GridLocation;

                            hashToExtend = hashToExtend.ExtendHashCode(location);       // HAS ITS OWN STRUCT-LIKE HASH CODE
                        }
                        else if (grid[column, row] is GridCellInfo)
                        {
                            var cell = grid[column, row] as GridCellInfo;

                            // Do collections first
                            foreach (var element in cell.TerrainLights)
                            {
                                hashToExtend = hashToExtend.ExtendHashCode(element.Key);
                                hashToExtend = hashToExtend.ExtendHashCode(element.Value);  // LIGHT HAS ITS OWN HASH CODE
                            }

                            hashToExtend = hashToExtend.ExtendHashCode(cell.AccentLight,    // LIGHT HAS ITS OWN HASH CODE
                                                                       cell.AmbientLight,
                                                                       cell.Column,
                                                                       cell.DoorSearchCounter,
                                                                       // cell.IsConnectionSupport,
                                                                       cell.IsCorridor,
                                                                       cell.IsDoor,
                                                                       cell.IsTerrainSupport,
                                                                       cell.IsWall,
                                                                       cell.IsWallLight,
                                                                       cell.Location,       // LOCATION HAS ITS OWN HASH CODE
                                                                       cell.Row,
                                                                       cell.Type,
                                                                       cell.WallLight);
                        }
                        else
                            throw new Exception("Unhandled Grid<T> Type:  RegionInfo.GetGridHashCode");
                    }
                }
            }

            return hashToExtend;
        }

        #region (public) Indexers for grid locations and edges
        public T this[int column, int row]
        {
            get { return _gridLocations[column, row]; }
        }
        public T this[IGridLocator location]
        {
            get { return _gridLocations[location.Column, location.Row]; }
        }
        public bool IsEdge(int column, int row)
        {
            return _edgeLocations[column, row];
        }
        #endregion

        public RegionInfo(string regionId, T[] locations, T[] edgeLocations, RegionBoundary boundary, RegionBoundary parentBoundary)
        {
            this.Id = regionId;
            this.Locations = locations;
            this.EdgeLocations = edgeLocations;
            this.Boundary = boundary;
            this.ParentBoundary = parentBoundary;

            _gridLocations = new Grid<T>(parentBoundary, boundary);
            _edgeLocations = new Grid<bool>(parentBoundary, boundary);

            _calculatedHash = default(int);

            // Non-serialized collections
            var leftEdgeLocations = new List<T>();
            var rightEdgeLocations = new List<T>();
            var topEdgeLocations = new List<T>();
            var bottomEdgeLocations = new List<T>();

            // Setup grid locations
            foreach (var location in locations)
            {
                // Validate location inside boundary
                if (!boundary.Contains(location))
                    throw new Exception("Invalid location for the region boundary Region.cs");

                _gridLocations[location.Column, location.Row] = location;
            }

            // Setup edge locations
            foreach (var location in edgeLocations)
            {
                // Validate location inside boundary
                if (!boundary.Contains(location))
                    throw new Exception("Invalid edge location for the region boundary Region.cs");

                _edgeLocations[location.Column, location.Row] = true;

                // Left Edge Exposed
                if (_gridLocations.IsDefined(location.Column - 1, location.Row) &&
                    _gridLocations[location.Column - 1, location.Row] == null)
                    leftEdgeLocations.Add(location);

                // Right Edge
                if (_gridLocations.IsDefined(location.Column + 1, location.Row) &&
                    _gridLocations[location.Column + 1, location.Row] == null)
                    rightEdgeLocations.Add(location);

                // Bottom Edge
                if (_gridLocations.IsDefined(location.Column, location.Row + 1) &&
                    _gridLocations[location.Column, location.Row + 1] == null)
                    bottomEdgeLocations.Add(location);

                // Top Edge
                if (_gridLocations.IsDefined(location.Column, location.Row - 1) &&
                    _gridLocations[location.Column, location.Row - 1] == null)
                    topEdgeLocations.Add(location);
            }

            this.LeftEdgeExposedLocations = leftEdgeLocations.ToArray();
            this.RightEdgeExposedLocations = rightEdgeLocations.ToArray();
            this.TopEdgeExposedLocations = topEdgeLocations.ToArray();
            this.BottomEdgeExposedLocations = bottomEdgeLocations.ToArray();

            _isInitialized = true;
        }

        public override string ToString()
        {
            return string.Format("Id={0} Locations[{1}], EdgeLocations[{2}], Boundary=[{3}]",
                                  this.Id, this.Locations.Length, this.EdgeLocations.Length, this.Boundary.ToString());
        }
    }
}
