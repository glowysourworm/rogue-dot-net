using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.Scenario.Content.Layout.Construction;
using Rogue.NET.Core.Model.ScenarioConfiguration.Layout;

using System.Collections.Generic;

namespace Rogue.NET.Core.Processing.Model.Generator.Layout.Finishing.Interface
{
    public interface ILightingFinisher
    {
        void CreateLighting(GridCellInfo[,] grid, LayoutTemplate template);

        void CreateDefaultLighting(GridCellInfo[,] grid);
    }
}
