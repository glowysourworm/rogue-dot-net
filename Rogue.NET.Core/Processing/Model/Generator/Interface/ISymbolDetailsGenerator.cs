using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;

namespace Rogue.NET.Core.Processing.Model.Generator.Interface
{
    public interface ISymbolDetailsGenerator
    {
        /// <summary>
        /// Copies symbol details to destination 
        /// </summary>
        void MapSymbolDetails(SymbolDetailsTemplate source, ScenarioImage dest);
    }
}
