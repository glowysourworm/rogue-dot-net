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

        public Region<GridLocation> this[int column, int row] { get; }

        public Region<GridLocation> this[IGridLocator location] { get; }

        public IEnumerable<Region<GridLocation>> Regions { get; }

        public IEnumerable<GridLocation> GetLocations();

        public IEnumerable<GridLocation> GetNonOccupiedLocations();

        public bool IsOccupied(IGridLocator location);

        public void SetOccupied(IGridLocator location, bool occupied);
    }
}
