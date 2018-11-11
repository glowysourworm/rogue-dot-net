using ExpressMapper;
using Rogue.NET.Core.Model.ScenarioConfiguration;
using Rogue.NET.Core.Model.ScenarioConfiguration.Animation;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Animation;

namespace Rogue.NET.ScenarioEditor.Utility
{
    /// <summary>
    /// Assembly definition for Express Mapper (one per assembly to create mapper definitions)
    /// </summary>
    public static class MapperInit
    {
        static MapperInit()
        {
            Mapper.Register<ScenarioConfigurationContainer, ScenarioConfigurationContainerViewModel>();
            Mapper.Register<ScenarioConfigurationContainerViewModel, ScenarioConfigurationContainer>();

            Mapper.Register<AnimationTemplateViewModel, AnimationTemplate>();

            Mapper.Compile();
        }
    }
}
