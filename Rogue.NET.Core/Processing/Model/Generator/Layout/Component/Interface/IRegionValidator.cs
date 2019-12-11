using Rogue.NET.Core.Model.Scenario.Content.Layout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Processing.Model.Generator.Layout.Component.Interface
{
    /// <summary>
    /// Validates regions (terrain or room) to make sure they fit the constraints for being rooms or terrain
    /// </summary>
    public interface IRegionValidator
    {
        int MinimumRoomSize { get; }

        bool ValidateRoomRegion(Region region);

        bool ValidateTerrainRegion(Region region);
    }
}
