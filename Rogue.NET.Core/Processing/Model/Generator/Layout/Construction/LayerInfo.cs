using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.Scenario.Content.Layout.Interface;
using System.Collections.Generic;
using System.Linq;

namespace Rogue.NET.Core.Processing.Model.Generator.Layout.Construction
{
    /// <summary>
    /// Class that represents a layer (collection of regions) for generating a layer map in the level grid
    /// </summary>
    public class LayerInfo<T> where T : class, IGridLocator
    {
        public string LayerName { get; private set; }
        public bool IsPassable { get; private set; }
        public virtual IEnumerable<Region<T>> Regions { get; private set; }

        public T this[int column, int row]
        {
            get
            {
                var vertex = this.Regions.FirstOrDefault(vertex => vertex[column, row] != null);

                return vertex != null ? vertex[column, row] : null;
            }
        }

        public LayerInfo(string layerName, IEnumerable<Region<T>> regions, bool isPassable)
        {
            this.Regions = regions;
            this.LayerName = layerName;
            this.IsPassable = isPassable;
        }
    }
}
