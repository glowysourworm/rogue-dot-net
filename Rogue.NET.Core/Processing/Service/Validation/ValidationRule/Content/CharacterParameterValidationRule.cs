using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration;
using Rogue.NET.Core.Model.ScenarioConfiguration.Content;
using Rogue.NET.Core.Processing.Service.Validation.Interface;
using System.Collections.Generic;
using System.Linq;

namespace Rogue.NET.Core.Processing.Service.Validation.ValidationRule.Content
{
    public class CharacterParameterValidationRule : IScenarioValidationRule
    {
        public CharacterParameterValidationRule() { }

        public IEnumerable<IScenarioValidationResult> Validate(ScenarioConfigurationContainer configuration)
        {
            return configuration.EnemyTemplates
                                .Cast<CharacterTemplate>()
                                .Union(configuration.FriendlyTemplates)
                                .Union(configuration.PlayerTemplates)
                                .Where(x => !x.Hp.IsSet() ||
                                            !x.Agility.IsSet() ||
                                            !x.Intelligence.IsSet() ||
                                            !x.LightRadius.IsSet() ||
                                            !x.Speed.IsSet() ||
                                            !x.Strength.IsSet())
                                .Select(x =>
                new ScenarioValidationResult()
                {
                    Asset = x,
                    Message = "Characters Must Have Certain Parameters Set to Non-Zero Value",
                    Passed = false,
                    Severity = ValidationSeverity.Error,
                    InnerMessage = x.Name + " must have parameters greater than zero for Hp, Agility, Intelligence, Light Radius, Speed, and Strength"

                });
        }
    }
}
