using Rogue.NET.Core.Math.Geometry;
using System.Collections.Generic;
using System.Linq;

namespace Rogue.NET.Core.Model.Scenario.Content.Layout
{
    /// <summary>
    /// Class that represents a layer (collection of regions) for generating a layer map in the level grid
    /// </summary>
    public class LayerInfo
    {
        public string LayerName { get; private set; }
        public bool IsPassable { get; private set; }
        public IEnumerable<Region<GridLocation>> Regions { get; private set; }

        public GridLocation this[int column, int row]
        {
            get
            {
                var vertex = this.Regions.FirstOrDefault(vertex => vertex[column, row] != null);

                return vertex != null ? vertex[column, row] : null;
            }
        }

        public LayerInfo(string layerName, IEnumerable<Region<GridLocation>> regions, bool isPassable)
        {
            this.Regions = regions;
            this.LayerName = layerName;
            this.IsPassable = isPassable;
        }
    }
}
