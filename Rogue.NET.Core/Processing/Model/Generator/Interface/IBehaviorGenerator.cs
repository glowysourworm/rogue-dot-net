using Rogue.NET.Core.Model.Scenario.Character.Behavior;
using Rogue.NET.Core.Model.ScenarioConfiguration.Content;

namespace Rogue.NET.Core.Processing.Model.Generator.Interface
{
    public interface IBehaviorGenerator
    {
        EnemyBehavior GenerateBehavior(BehaviorTemplate behaviorTemplate);
    }
}
