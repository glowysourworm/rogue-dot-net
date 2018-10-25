using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.ScenarioConfiguration.Content;

namespace Rogue.NET.Engine.Model.Generator.Interface
{
    public interface IBehaviorGenerator
    {
        Behavior GenerateBehavior(BehaviorTemplate behaviorTemplate);
    }
}
