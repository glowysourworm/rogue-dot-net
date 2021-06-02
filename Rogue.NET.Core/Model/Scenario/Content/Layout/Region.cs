using Rogue.NET.Common.Extension;
using Rogue.NET.Common.Serialization.Interface;
using Rogue.NET.Core.Math.Algorithm.Interface;
using Rogue.NET.Core.Model.Scenario.Content.Layout.Interface;

using System;
using System.Collections.Generic;

namespace Rogue.NET.Core.Model.Scenario.Content.Layout
{
    /// <summary>
    /// Serializable data structure to store regions of the layout that are related
    /// </summary>
    [Serializable]
    public class Region<T> : IRecursiveSerializable, IGraphNode where T : class, IGridLocator
    {
        public string Id { get; private set; }
        public T[] Locations { get; private set; }
        public T[] EdgeLocations { get; private set; }
        public RegionBoundary Boundary { get; private set; }
        public RegionBoundary ParentBoundary { get; private set; }

        // IGraphNode
        public int Hash { get { return base.GetHashCode(); } }

        // Store calculated hash for efficiency
        int _calculatedHash;

        // 2D Arrays for region locations and edges - NOT SERIALIZED
        Grid<T> _grid;
        Grid<bool> _edgeGrid;

        #region (public) Indexers for grid locations and edges
        public T this[int column, int row]
        {
            get { return _grid[column, row]; }
        }
        public T this[IGridLocator location]
        {
            get { return _grid[location.Column, location.Row]; }
        }
        public bool IsEdge(int column, int row)
        {
            return _edgeGrid[column, row];
        }
        #endregion

        #region STRUCT-LIKE EQUALS / HASH BEHAVIOR
        public static bool operator ==(Region<T> region1, Region<T> region2)
        {
            if (ReferenceEquals(region1, region2))
                return true;

            else if (ReferenceEquals(region1, null))
                return ReferenceEquals(region2, null);

            else if (ReferenceEquals(region2, null))
                return false;

            else
                return region1.Equals(region2);
        }

        public static bool operator !=(Region<T> region1, Region<T> region2)
        {
            if (ReferenceEquals(region1, region2))
                return false;

            else if (ReferenceEquals(region1, null))
                return !ReferenceEquals(region2, null);

            else if (ReferenceEquals(region2, null))
                return true;

            else
                return !region1.Equals(region2);
        }

        public override bool Equals(object obj)
        {
            var otherRegion = (Region<T>)obj;

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
                _calculatedHash = ExtendHashCodeForGrid(_grid, _calculatedHash);
            }

            return _calculatedHash;
        }

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
                        else
                            throw new Exception("Unhandled Grid<T> Type:  Region.GetGridHashCode");
                    }
                }
            }

            return hashToExtend;
        }
        #endregion

        public Region(string regionId, T[] locations, T[] edgeLocations, RegionBoundary boundary, RegionBoundary parentBoundary)
        {
            if (string.IsNullOrEmpty(regionId) || locations.Length == 0 || edgeLocations.Length == 0)
                throw new Exception("Invalid Region parameters - Region.cs");

            this.Id = regionId;
            this.Locations = locations;
            this.EdgeLocations = edgeLocations;
            this.Boundary = boundary;
            this.ParentBoundary = parentBoundary;

            _calculatedHash = default(int);

            _grid = new Grid<T>(parentBoundary, boundary);
            _edgeGrid = new Grid<bool>(parentBoundary, boundary);

            // Setup grid locations
            foreach (var location in locations)
            {
                // Validate location inside boundary
                if (!boundary.Contains(location))
                    throw new Exception("Invalid location for the region boundary Region.cs");

                _grid[location.Column, location.Row] = location;
            }

            // Setup edge locations
            foreach (var location in edgeLocations)
            {
                // Validate location inside boundary
                if (!boundary.Contains(location))
                    throw new Exception("Invalid edge location for the region boundary Region.cs");

                _edgeGrid[location.Column, location.Row] = true;
            }
        }

        public Region(IPropertyReader reader)
        {
            var regionId = reader.Read<string>("Id");
            var grid = reader.Read<Grid<T>>("Grid");
            var edgeGrid = reader.Read<Grid<bool>>("EdgeGrid");
            var boundary = reader.Read<RegionBoundary>("Boundary");
            var parentBoundary = reader.Read<RegionBoundary>("ParentBoundary");

            _calculatedHash = default(int);

            _grid = grid;
            _edgeGrid = edgeGrid;

            var locations = new List<T>();
            var edgeLocations = new List<T>();

            this.Id = regionId;
            this.Boundary = boundary;
            this.ParentBoundary = parentBoundary;

            for (int column = this.ParentBoundary.Left; column <= this.ParentBoundary.Right; column++)
            {
                for (int row = this.ParentBoundary.Top; row <= this.ParentBoundary.Bottom; row++)
                {
                    if (_grid[column, row] != null)
                    {
                        locations.Add(_grid[column, row]);

                        if (_edgeGrid[column, row])
                            edgeLocations.Add(_grid[column, row]);
                    }
                }
            }

            this.Locations = locations.ToArray();
            this.EdgeLocations = edgeLocations.ToArray();
        }

        public void GetProperties(IPropertyWriter writer)
        {
            writer.Write("Id", this.Id);
            writer.Write("Grid", _grid);
            writer.Write("EdgeGrid", _edgeGrid);
            writer.Write("Boundary", this.Boundary);
            writer.Write("ParentBoundary", this.ParentBoundary);
        }

        public override string ToString()
        {
            return string.Format("Id={0} Locations[{1}], EdgeLocations[{2}], Boundary=[{3}]",
                                  this.Id, this.Locations.Length, this.EdgeLocations.Length, this.Boundary.ToString());
        }
    }
}
