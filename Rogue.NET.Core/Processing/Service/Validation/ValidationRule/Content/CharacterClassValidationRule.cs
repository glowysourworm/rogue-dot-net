using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using Rogue.NET.Core.Processing.Service.Validation.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Processing.Service.Validation.ValidationRule.Content
{
    public class CharacterClassValidationRule : IScenarioValidationRule
    {
        public CharacterClassValidationRule() { }

        public IEnumerable<IScenarioValidationResult> Validate(ScenarioConfigurationContainer configuration)
        {
            var characterClasses = configuration.PlayerTemplates.Select(x => x.Name).Actualize();

            var equipment = configuration.EquipmentTemplates
                                         .Where(x => x.HasCharacterClassRequirement)
                                         .Where(x => string.IsNullOrEmpty(x.CharacterClass) || !characterClasses.Contains(x.CharacterClass))
                                         .Actualize();

            var consumables = configuration.EquipmentTemplates
                                         .Where(x => x.HasCharacterClassRequirement)
                                         .Where(x => string.IsNullOrEmpty(x.CharacterClass) || !characterClasses.Contains(x.CharacterClass))
                                         .Actualize();

            var doodads = configuration.DoodadTemplates
                                         .Where(x => x.HasCharacterClassRequirement)
                                         .Where(x => string.IsNullOrEmpty(x.CharacterClass) || !characterClasses.Contains(x.CharacterClass))
                                         .Actualize();

            var skills = configuration.SkillTemplates
                                      .Where(skillSet =>
                                      {
                                          return skillSet.Skills
                                                         .Where(x => x.HasCharacterClassRequirement)
                                                         .Any(x => string.IsNullOrEmpty(x.CharacterClass) || !characterClasses.Contains(x.CharacterClass));
                                      })
                                      .Actualize();

            return equipment.Cast<DungeonObjectTemplate>()
                            .Union(consumables)
                            .Union(doodads)
                            .Union(skills)
                            .Select(x => new ScenarioValidationResult()
                            {
                                Asset = x,
                                Severity = ValidationSeverity.Error,
                                Message = "Character Class Requirement must have a valid Character Class Set",
                                InnerMessage = x.Name + " has a character class requirement with no valid character class set",
                                Passed = false
                            })
                            .Actualize();
        }
    }
}
