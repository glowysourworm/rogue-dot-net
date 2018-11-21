using ExpressMapper;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Model.Scenario.Content.Item;
using Rogue.NET.Core.Model.Scenario.Content.Skill;
using Rogue.NET.Core.Model.ScenarioConfiguration;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using Rogue.NET.Core.Model.ScenarioConfiguration.Animation;
using Rogue.NET.Core.Model.ScenarioConfiguration.Content;
using Rogue.NET.Scenario.Content.ViewModel.Content;
using Rogue.NET.Scenario.ViewModel.ItemGrid;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Animation;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Utility
{
    public static class MapperInit
    {
        static MapperInit()
        {
            Mapper.Register<Enemy, ScenarioImage>();
            Mapper.Register<Player, ScenarioImage>();
            Mapper.Register<SymbolDetailsTemplate, ScenarioImage>();

            Mapper.Register<Equipment, EquipmentViewModel>()
                  .Member(x => x.RogueName, x => x.RogueName)
                  .Member(x => x.Id, x => x.Id);

            Mapper.Register<SkillSet, SkillSetViewModel>();

            Mapper.Register<ScenarioConfigurationContainer, ScenarioConfigurationContainerViewModel>();
            Mapper.Register<ScenarioConfigurationContainerViewModel, ScenarioConfigurationContainer>();

            Mapper.Register<ScenarioImageViewModel, ScenarioImage>();
            Mapper.Register<ItemGridRowViewModel, ScenarioImage>();
            Mapper.Register<SymbolDetailsTemplateViewModel, SymbolDetailsTemplate>();
            Mapper.Register<AnimationTemplateViewModel, AnimationTemplate>();

            // Performance Problem - 1-2 minutes to compile... Maybe try the Emit Mapper?
            // Mapper.Compile();
        }

        public static void Initialize() { }
    }
}
