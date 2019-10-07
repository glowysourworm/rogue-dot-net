using Rogue.NET.Core.Model.Scenario.Animation;
using Rogue.NET.Core.Model.ScenarioConfiguration.Animation;

namespace Rogue.NET.Core.Processing.Model.Generator.Interface
{
    public interface IAnimationGenerator
    {
        AnimationSequence GenerateAnimation(AnimationSequenceTemplate template);
    }
}
