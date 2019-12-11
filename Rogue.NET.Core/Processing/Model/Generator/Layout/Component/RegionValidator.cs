using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Processing.Model.Generator.Layout.Component.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Processing.Model.Generator.Layout.Component
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(IRegionValidator))]
    public class RegionValidator : IRegionValidator
    {
        // Constraints may depend on the layout parameters - but, for now, just make them "big enough"
        const int ROOM_MIN_SIZE = 4;
        const int TERRAIN_MIN_SIZE = 4;

        public RegionValidator()
        {

        }

        public int MinimumRoomSize
        { 
            get { return ROOM_MIN_SIZE; }
        }

        public bool ValidateRoomRegion(Region region)
        {
            return region.Cells.Length >= ROOM_MIN_SIZE;
        }

        public bool ValidateTerrainRegion(Region region)
        {
            return region.Cells.Length >= TERRAIN_MIN_SIZE;
        }
    }
}
