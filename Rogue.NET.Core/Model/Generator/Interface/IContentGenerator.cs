using Rogue.NET.Core.Model.Scenario;
using Rogue.NET.Core.Model.ScenarioConfiguration;
using System.Collections.Generic;

namespace Rogue.NET.Engine.Model.Generator.Interface
{
    public interface IContentGenerator
    {
        IEnumerable<Level> CreateContents(
                IEnumerable<Level> levels,
                ScenarioConfigurationContainer configurationContainer,
                bool survivorMode);
    }
}
