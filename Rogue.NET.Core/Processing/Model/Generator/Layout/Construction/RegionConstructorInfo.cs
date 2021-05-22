using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.Scenario.Content.Layout.Interface;

namespace Rogue.NET.Core.Processing.Model.Generator.Layout.Construction
{
    public class RegionConstructorInfo<T> where T : class, IGridLocator
    {
        public string Id { get; private set; }
        public T[] Locations { get; private set; }
        public T[] EdgeLocations { get; private set; }
        public RegionBoundary Boundary { get; private set; }
        public RegionBoundary ParentBoundary { get; private set; }

        public RegionConstructorInfo(string id, T[] locations, T[] edgeLocations, RegionBoundary boundary, RegionBoundary parentBoundary)
        {
            this.Id = id;
            this.Locations = locations;
            this.EdgeLocations = edgeLocations;
            this.ParentBoundary = parentBoundary;
            this.Boundary = boundary;
        }
    }
}
