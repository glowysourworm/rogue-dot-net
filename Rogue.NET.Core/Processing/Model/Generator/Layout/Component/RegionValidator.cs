using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.Scenario.Content.Layout.Interface;
using Rogue.NET.Core.Processing.Model.Generator.Layout.Component.Interface;
using System.ComponentModel.Composition;

namespace Rogue.NET.Core.Processing.Model.Generator.Layout.Component
{
    public static class RegionValidator
    {
        // Constraints may depend on the layout parameters - but, for now, just make them "big enough"
        const int ROOM_MIN_SIZE = 4;
        const int TERRAIN_MIN_SIZE = 4;

        public static int MinimumRoomSize
        {
            get { return ROOM_MIN_SIZE; }
        }

        public static bool ValidateRoomRegion<T>(Region<T> region) where T : class, IGridLocator
        {
            return region.Locations.Length >= ROOM_MIN_SIZE;
        }

        public static bool ValidateTerrainRegion<T>(Region<T> region) where T : class, IGridLocator
        {
            return region.Locations.Length >= TERRAIN_MIN_SIZE;
        }
    }
}
