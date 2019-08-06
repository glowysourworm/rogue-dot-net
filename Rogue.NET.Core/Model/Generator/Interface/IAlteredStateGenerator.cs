using Rogue.NET.Core.Model.Scenario.Alteration;
using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration;

namespace Rogue.NET.Core.Model.Generator.Interface
{
    public interface IAlteredStateGenerator
    {
        AlteredCharacterState GenerateAlteredState(AlteredCharacterStateTemplate template);
    }
}
