using System.Collections.Generic;
using System.Linq;

namespace Rogue.NET.Core.Model.Scenario.Content.Layout
{
    /// <summary>
    /// Class that represents a layer (collection of regions) for generating a layer map in the level grid
    /// </summary>
    public class LayerInfo
    {
        List<Region<GridLocation>> _regions;

        public string LayerName { get; private set; }
        public bool IsPassable { get; private set; }
        public IEnumerable<Region<GridLocation>> Regions { get { return _regions; } }

        public GridLocation this[int column, int row]
        {
            get
            {
                var region = _regions.FirstOrDefault(region => region[column, row] != null);

                return region != null ? region[column, row] : null;
            }
        }

        public void AddRegion(Region<GridLocation> region)
        {
            _regions.Add(region);
        }

        public LayerInfo(string layerName, IEnumerable<Region<GridLocation>> regions, bool isPassable)
        {
            _regions = new List<Region<GridLocation>>(regions);

            this.LayerName = layerName;
            this.IsPassable = isPassable;
        }
    }
}
