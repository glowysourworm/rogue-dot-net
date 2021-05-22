using Rogue.NET.Core.Model.ScenarioConfiguration.Layout;
using Rogue.NET.Core.Processing.Model.Generator.Layout.Construction;

namespace Rogue.NET.Core.Processing.Model.Generator.Layout.Builder.Interface
{
    public interface ISymmetricBuilder
    {
        /// <summary>
        /// Creates a BASE and CONNECTION layer from the template
        /// </summary>
        GridCellInfo[,] CreateSymmetricLayout(LayoutTemplate template);
    }
}
