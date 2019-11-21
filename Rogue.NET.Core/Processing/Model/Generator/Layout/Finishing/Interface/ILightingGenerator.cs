using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.ScenarioConfiguration.Layout;
using System.Collections.Generic;

namespace Rogue.NET.Core.Processing.Model.Generator.Layout.Finishing.Interface
{
    public interface ILightingGenerator
    {
        void CreateLightThreshold(GridCellInfo[,] grid, LayoutTemplate template);
        void CreateLightedRooms(GridCellInfo[,] grid, IEnumerable<Region> regions, LightAmbientTemplate template);
        void CreateWhiteNoiseLighting(GridCellInfo[,] grid, LightAmbientTemplate template);
        void CreatePerlinNoiseLighting(GridCellInfo[,] grid, LightAmbientTemplate template);
        void CreateWallLighting(GridCellInfo[,] grid, LightAmbientTemplate template);
    }
}
