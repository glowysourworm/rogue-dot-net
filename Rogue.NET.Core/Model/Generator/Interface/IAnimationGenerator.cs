using Rogue.NET.Core.Model.Scenario.Animation;
using Rogue.NET.Core.Model.ScenarioConfiguration.Animation;

namespace Rogue.NET.Core.Model.Generator.Interface
{
    public interface IAnimationGenerator
    {
        AnimationGroup GenerateAnimationGroup(AnimationGroupTemplate template);
        AnimationData GenerateAnimation(AnimationTemplate template);
    }
}
