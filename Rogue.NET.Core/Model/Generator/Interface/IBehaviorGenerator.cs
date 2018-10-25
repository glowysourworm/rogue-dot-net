using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.ScenarioConfiguration.Content;

namespace Rogue.NET.Core.Model.Generator.Interface
{
    public interface IBehaviorGenerator
    {
        Behavior GenerateBehavior(BehaviorTemplate behaviorTemplate);
    }
}
