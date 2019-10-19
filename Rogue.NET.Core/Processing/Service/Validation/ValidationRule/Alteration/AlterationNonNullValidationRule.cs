using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration;
using Rogue.NET.Core.Model.ScenarioConfiguration.Extension;
using Rogue.NET.Core.Processing.Service.Validation.Interface;
using System.Collections.Generic;
using System.Linq;

namespace Rogue.NET.Core.Processing.Service.Validation.ValidationRule.Alteration
{
    public class AlterationNonNullValidationRule : IScenarioValidationRule
    {
        public AlterationNonNullValidationRule() { }

        public IEnumerable<IScenarioValidationResult> Validate(ScenarioConfigurationContainer configuration)
        {
            var alterations = configuration.GetAllAlterations();

            return alterations.Where(x => x.Alteration == null)
                              .Select(x =>
                                {
                                    return new ScenarioValidationResult()
                                    {
                                        Asset = x.Asset,
                                        Message = "Alteration Effects (and / or) Learned Skills have to be set for all configured assets",
                                        Passed = false,
                                        Severity = ValidationSeverity.Error,
                                        InnerMessage = x.Asset.Name + " has an un-set alteration effect (or) learned skill property"
                                    };
                                });
        }
    }
}
