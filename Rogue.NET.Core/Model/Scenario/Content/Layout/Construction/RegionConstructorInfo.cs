using Rogue.NET.Core.Model.Scenario.Content.Layout.Interface;

namespace Rogue.NET.Core.Model.Scenario.Content.Layout.Construction
{
    public class RegionConstructorInfo<T> where T : class, IGridLocator
    {
        public T[] Locations { get; private set; }
        public T[] EdgeLocations { get; private set; }
        public RegionBoundary Boundary { get; private set; }
        public RegionBoundary ParentBoundary { get; private set; }

        public RegionConstructorInfo(T[] locations, T[] edgeLocations, RegionBoundary parentBoundary, RegionBoundary boundary)
        {
            this.Locations = locations;
            this.EdgeLocations = edgeLocations;
            this.ParentBoundary = parentBoundary;
            this.Boundary = boundary;
        }
    }
}
