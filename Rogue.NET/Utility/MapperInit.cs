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
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration;
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

            // Probability Consumable -> Probability Consumable View Model
            Mapper.RegisterCustom<ProbabilityConsumableTemplate, ProbabilityConsumableTemplateViewModel>(model =>
            {
                return new ProbabilityConsumableTemplateViewModel()
                {
                    GenerationProbability = model.GenerationProbability,
                    Guid = model.Guid,
                    Name = model.Name,
                    TheTemplate = Mapper.Map<ConsumableTemplate, ConsumableTemplateViewModel>(model.TheTemplate as ConsumableTemplate)
                };
            });
            // Probability Equipment -> Probability Equipment View Model
            Mapper.RegisterCustom<ProbabilityEquipmentTemplate, ProbabilityEquipmentTemplateViewModel>(model =>
            {
                return new ProbabilityEquipmentTemplateViewModel()
                {
                    GenerationProbability = model.GenerationProbability,
                    Guid = model.Guid,
                    Name = model.Name,
                    TheTemplate = Mapper.Map<EquipmentTemplate, EquipmentTemplateViewModel>(model.TheTemplate as EquipmentTemplate)
                };
            });
            // Probability Consumable View Model -> Probability Consumable
            Mapper.RegisterCustom<ProbabilityConsumableTemplateViewModel, ProbabilityConsumableTemplate>(viewModel =>
            {
                return new ProbabilityConsumableTemplate()
                {
                    GenerationProbability = viewModel.GenerationProbability,
                    Guid = viewModel.Guid,
                    Name = viewModel.Name,
                    TheTemplate = Mapper.Map<ConsumableTemplateViewModel, ConsumableTemplate>(viewModel.TheTemplate as ConsumableTemplateViewModel)
                };
            });
            // Probability Equipment View Model -> Probability Equipment
            Mapper.RegisterCustom<ProbabilityEquipmentTemplateViewModel, ProbabilityEquipmentTemplate>(viewModel =>
            {
                return new ProbabilityEquipmentTemplate()
                {
                    GenerationProbability = viewModel.GenerationProbability,
                    Guid = viewModel.Guid,
                    Name = viewModel.Name,
                    TheTemplate = Mapper.Map<EquipmentTemplateViewModel, EquipmentTemplate>(viewModel.TheTemplate as EquipmentTemplateViewModel)
                };
            });


            Mapper.Register<AnimationTemplateViewModel, AnimationTemplate>();

            // Performance Problem - 1-2 minutes to compile... Maybe try the Emit Mapper?
            // Mapper.Compile();
        }

        public static void Initialize() { }
    }
}
