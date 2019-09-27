using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content.Doodad;
using Rogue.NET.Core.Model.ScenarioConfiguration.Content;

namespace Rogue.NET.Core.Processing.Model.Generator.Interface
{
    public interface IDoodadGenerator
    {
        DoodadNormal GenerateNormalDoodad(string name, DoodadNormalType type);
        DoodadMagic GenerateMagicDoodad(DoodadTemplate doodadTemplate);
    }
}
