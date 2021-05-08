using Rogue.NET.Core.Model.Scenario.Content.Layout.Interface;

using System;

namespace Rogue.NET.Core.Model.Scenario.Content.Layout
{
    [Serializable]
    public class RegionConnection<T> where T : class, IGridLocator
    {
        public T Location { get; set; }
        public T AdjacentLocation { get; set; }

        public string AdjacentRegionId { get; set; }
    }
}
