using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Converter.Model.ScenarioConfiguration;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Common;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Interface;
using Rogue.NET.Core.Model.ScenarioConfiguration.Animation;
using Rogue.NET.Core.Model.ScenarioConfiguration.Content;
using Rogue.NET.Core.Model.ScenarioConfiguration.Extension;
using Rogue.NET.Core.Processing.Service.Validation.Interface;
using Rogue.NET.Core.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace Rogue.NET.Core.Processing.Service.Validation
{
    [Export(typeof(IScenarioValidationService))]
    public class ScenarioValidationService : IScenarioValidationService
    {
        [ImportingConstructor]
        public ScenarioValidationService()
        {

        }

        public IEnumerable<IScenarioValidationResult> Validate(ScenarioConfigurationContainer configuration)
        {
            var validationRules = CreateValidationRules();

            return validationRules.SelectMany(x => x.Validate(configuration))
                                  .Actualize();
        }

        public bool IsValid(ScenarioConfigurationContainer scenarioConfigurationContainer)
        {
            var validationRules = CreateValidationRules();

            return validationRules.SelectMany(x => x.Validate(scenarioConfigurationContainer))
                                  .Where(x => x.Severity == ValidationSeverity.Error)
                                  .All(x => x.Passed);
        }

        private IEnumerable<IScenarioValidationRule> CreateValidationRules()
        {
            var validationRuleTypes = typeof(ScenarioValidationService).Assembly
                                                                       .GetTypes()
                                                                       .Where(type => typeof(IScenarioValidationRule).IsAssignableFrom(type) && 
                                                                                     !type.IsInterface)
                                                                       .Actualize();

            return validationRuleTypes.Select(type => type.Construct())
                                      .Cast<IScenarioValidationRule>()
                                      .Actualize();
        }
    }
}
