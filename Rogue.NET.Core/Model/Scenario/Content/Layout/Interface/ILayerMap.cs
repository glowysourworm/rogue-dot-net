using System.Collections.Generic;

namespace Rogue.NET.Core.Model.Scenario.Content.Layout.Interface
{
    /// <summary>
    /// Common members for LayerMapBase implementations built on Region of type GridLocation
    /// </summary>
    public interface ILayerMap
    {
        public string Name { get; }

        public RegionBoundary Boundary { get; }

        public RegionBoundary ParentBoundary { get; }

        // Avoids double-indexing the ILayerMap for the grid location. ALSO CHECKS FOR NULL
        // REGION TO AVOID EXCEPTIONS. Returns null if there is no region at the specified indices.
        public GridLocation Get(int column, int row);

        public IEnumerable<Region<GridLocation>> Regions { get; }

        public Region<GridLocation> this[int column, int row] { get; }

        public Region<GridLocation> this[IGridLocator location] { get; }

        public IEnumerable<GridLocation> GetLocations();
    }
}
