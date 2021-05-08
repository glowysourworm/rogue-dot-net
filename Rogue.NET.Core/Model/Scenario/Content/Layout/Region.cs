using Rogue.NET.Core.Model.Scenario.Content.Layout.Interface;

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Rogue.NET.Core.Model.Scenario.Content.Layout
{
    /// <summary>
    /// Serializable data structure to store calculated room information
    /// </summary>
    [Serializable]
    public class Region<T> : ISerializable, IDeserializationCallback where T : class, IGridLocator
    {
        public string Id { get; private set; }
        public T[] Locations { get; private set; }
        public T[] EdgeLocations { get; private set; }
        public T[] LeftEdgeExposedLocations { get; private set; }
        public T[] RightEdgeExposedLocations { get; private set; }
        public T[] TopEdgeExposedLocations { get; private set; }
        public T[] BottomEdgeExposedLocations { get; private set; }
        public IEnumerable<T> OccupiedLocations { get { return _occupiedLocations; } }
        public IEnumerable<T> NonOccupiedLocations { get { return _nonOccupiedLocations; } }
        public RegionBoundary Boundary { get; private set; }
        public RegionBoundary ParentBoundary { get; private set; }

        // Occupied Location Collections
        List<T> _occupiedLocations;
        List<T> _nonOccupiedLocations;

        // 2D Arrays for region locations and edges - NOT SERIALIZED
        Grid<T> _gridLocations;
        Grid<bool> _edgeLocations;
        Grid<bool> _occupiedLocationGrid;

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
        public bool IsOccupied(IGridLocator location)
        {
            return _occupiedLocationGrid[location.Column, location.Row];
        }
        public void SetOccupied(IGridLocator location, bool occupied)
        {
            var column = location.Column;
            var row = location.Row;

            // Occupied
            if (_occupiedLocationGrid[column, row])
            {
                if (!occupied)
                {
                    _occupiedLocations.Remove(_gridLocations[column, row]);
                    _nonOccupiedLocations.Add(_gridLocations[column, row]);
                }
            }

            // Non-Occupied
            else
            {
                if (occupied)
                {
                    _nonOccupiedLocations.Remove(_gridLocations[column, row]);
                    _occupiedLocations.Add(_gridLocations[column, row]);
                }
            }

            _occupiedLocationGrid[column, row] = occupied;
        }
        #endregion

        public Region(string regionId, T[] locations, T[] edgeLocations, RegionBoundary boundary, RegionBoundary parentBoundary)
        {
            Initialize(regionId, locations, edgeLocations, boundary, parentBoundary);
        }

        public Region(T[] locations, T[] edgeLocations, RegionBoundary boundary, RegionBoundary parentBoundary)
        {
            Initialize(Guid.NewGuid().ToString(), locations, edgeLocations, boundary, parentBoundary);
        }

        private void Initialize(string regionId, T[] locations, T[] edgeLocations, RegionBoundary boundary, RegionBoundary parentBoundary)
        {
            this.Id = regionId;
            this.Locations = locations;
            this.EdgeLocations = edgeLocations;
            this.Boundary = boundary;
            this.ParentBoundary = parentBoundary;

            _gridLocations = new Grid<T>(parentBoundary, boundary);
            _edgeLocations = new Grid<bool>(parentBoundary, boundary);
            _occupiedLocationGrid = new Grid<bool>(parentBoundary, boundary);

            _occupiedLocations = new List<T>();
            _nonOccupiedLocations = new List<T>(locations);

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
        }

        public Region(SerializationInfo info, StreamingContext context)
        {
            var regionId = info.GetString("Id");
            var locations = new T[info.GetInt32("LocationsLength")];
            var edgeLocations = new T[info.GetInt32("EdgeLocationsLength")];
            var boundary = (RegionBoundary)info.GetValue("Boundary", typeof(RegionBoundary));
            var parentBoundary = (RegionBoundary)info.GetValue("ParentBoundary", typeof(RegionBoundary));

            for (int i = 0; i < this.Locations.Length; i++)
                locations[i] = (T)info.GetValue("Location" + i.ToString(), typeof(T));

            for (int i = 0; i < this.EdgeLocations.Length; i++)
                edgeLocations[i] = (T)info.GetValue("EdgeLocation" + i.ToString(), typeof(T));

            Initialize(regionId, locations, edgeLocations, boundary, parentBoundary);
        }

        // TODO: REMOVE THIS - ALSO, REMOVE SERIALIZATION OF EDGES
        public void OnDeserialization(object sender)
        {
            if (sender == null)
                return;

            var grid = sender as GridCell[,];

            if (grid == null)
                throw new Exception("Improper use of OnDeserialization()  LayerMap");

            // Setup grid locations
            for (int i = 0; i < this.Locations.Length; i++)
            {
                var location = this.Locations[i] as GridLocation;

                if (location == null)
                    throw new Exception("Improper use of Region<T> - Should be set up for GridLocation for storage");

                var referenceLocation = grid[location.Column, location.Row].Location as T;

                // SET REFERENCES FROM THE PRIMARY GRID
                this.Locations[i] = referenceLocation;

                // INITIALIZED IN PARALLEL TO LOCATIONS
                _nonOccupiedLocations[i] = referenceLocation;

                // SET REFERENCES FROM THE PRIMARY GRID
                _gridLocations[location.Column, location.Row] = referenceLocation;
            }

            // Setup edge locations
            for (int i = 0; i < this.EdgeLocations.Length; i++)
            {
                var location = this.EdgeLocations[i] as GridLocation;

                if (location == null)
                    throw new Exception("Improper use of Region<T> - Should be set up for GridLocation for storage");

                var referenceLocation = grid[location.Column, location.Row].Location as T;

                // SET REFERENCES FROM THE PRIMARY GRID
                this.EdgeLocations[i] = referenceLocation;

                // SET REFERENCES FROM THE PRIMARY GRID
                _gridLocations[location.Column, location.Row] = referenceLocation;
            }

            // Non-serialized collections
            var leftEdgeLocations = new List<T>();
            var rightEdgeLocations = new List<T>();
            var topEdgeLocations = new List<T>();
            var bottomEdgeLocations = new List<T>();

            // Setup exposed edge locations
            foreach (var location in this.EdgeLocations)
            {
                // Validate location inside boundary
                if (!this.Boundary.Contains(location))
                    throw new Exception("Invalid edge location for the region boundary Region.cs");

                _edgeLocations[location.Column, location.Row] = true;

                // Left Edge Exposed
                if (_gridLocations[location.Column - 1, location.Row] == null)
                    leftEdgeLocations.Add(location);

                // Right Edge
                if (_gridLocations[location.Column + 1, location.Row] == null)
                    rightEdgeLocations.Add(location);

                // Bottom Edge
                if (_gridLocations[location.Column, location.Row + 1] == null)
                    bottomEdgeLocations.Add(location);

                // Top Edge
                if (_gridLocations[location.Column, location.Row - 1] == null)
                    topEdgeLocations.Add(location);
            }

            this.LeftEdgeExposedLocations = leftEdgeLocations.ToArray();
            this.RightEdgeExposedLocations = rightEdgeLocations.ToArray();
            this.TopEdgeExposedLocations = topEdgeLocations.ToArray();
            this.BottomEdgeExposedLocations = bottomEdgeLocations.ToArray();
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Id", this.Id);
            info.AddValue("LocationsLength", this.Locations.Length);
            info.AddValue("EdgeLocationsLength", this.EdgeLocations.Length);
            info.AddValue("Boundary", this.Boundary);
            info.AddValue("ParentBoundary", this.ParentBoundary);

            var counter = 0;

            foreach (var location in this.Locations)
            {
                // VALIDATE TYPE USED AS IGridLocator
                if (location.GetType() != typeof(GridLocation))
                    throw new SerializationException("Unsupported IGridLocator type during serialization Region.cs");

                info.AddValue("Location" + counter++.ToString(), location);
            }

            counter = 0;

            foreach (var location in this.EdgeLocations)
            {
                // VALIDATE TYPE USED AS IGridLocator
                if (location.GetType() != typeof(GridLocation))
                    throw new SerializationException("Unsupported IGridLocator type during serialization Region.cs");

                info.AddValue("EdgeLocation" + counter++.ToString(), location);
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is Region<T>)
            {
                var region = obj as Region<T>;

                // Check that the number of cells matches
                if (region.Locations.Length != this.Locations.Length)
                    return false;

                // Check that the boundary matches
                if (!region.Boundary.Equals(this.Boundary))
                    return false;

                // Iterate until a mis-match is found
                foreach (var otherLocation in region.Locations)
                {
                    if (this[otherLocation.Column, otherLocation.Row] == null)
                        return false;
                }

                // Found match for each cell
                return true;
            }

            else
                return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("Locations[{0}], EdgeLocations[{1}], Boundary=[{2}]", this.Locations.Length, this.EdgeLocations.Length, this.Boundary.ToString());
        }
    }
}
