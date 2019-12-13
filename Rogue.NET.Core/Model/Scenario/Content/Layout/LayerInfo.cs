using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Model.Scenario.Content.Layout
{
    /// <summary>
    /// Class that represents a layer (collection of regions) for generating a layer map in the level grid
    /// </summary>
    public class LayerInfo
    {
        List<Region<GridLocation>> _regions;

        public string LayerName { get; private set; }
        public IEnumerable<Region<GridLocation>> Regions { get { return _regions; } }

        public void AddRegion(Region<GridLocation> region)
        {
            _regions.Add(region);
        }

        public LayerInfo(string layerName)
        {
            _regions = new List<Region<GridLocation>>();

            this.LayerName = layerName;
        }
        public LayerInfo(string layerName, IEnumerable<Region<GridLocation>> regions)
        {
            _regions = new List<Region<GridLocation>>(regions);

            this.LayerName = layerName;
        }
    }
}
