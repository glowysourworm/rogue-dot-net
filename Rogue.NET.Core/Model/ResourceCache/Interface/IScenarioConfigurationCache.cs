using Rogue.NET.Core.Model.ScenarioConfiguration;

using System.Collections.Generic;

namespace Rogue.NET.Core.Model.ResourceCache.Interface
{
    public interface IScenarioConfigurationCache
    {
        IEnumerable<ScenarioConfigurationContainer> EmbeddedConfigurations { get; }
    }
}
