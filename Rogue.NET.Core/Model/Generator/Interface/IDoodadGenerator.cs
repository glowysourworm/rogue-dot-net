using Rogue.NET.Core.Model.Scenario.Content.Doodad;
using Rogue.NET.Core.Model.ScenarioConfiguration.Content;

namespace Rogue.NET.Core.Model.Generator.Interface
{
    public interface IDoodadGenerator
    {
        DoodadMagic GenerateDoodad(DoodadTemplate doodadTemplate);
    }
}
