using Rogue.NET.Core.Model.ScenarioConfiguration.Layout;
using Rogue.NET.Core.Processing.Model.Generator.Layout.Construction;

namespace Rogue.NET.Core.Processing.Model.Generator.Layout.Finishing.Interface
{
    public interface ILightingFinisher
    {
        void CreateLighting(GridCellInfo[,] grid, LayoutTemplate template);

        void CreateDefaultLighting(GridCellInfo[,] grid);
    }
}
