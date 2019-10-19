using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Common;
using Rogue.NET.Core.Model.ScenarioConfiguration.Extension;
using Rogue.NET.Core.Processing.Service.Validation.Interface;
using System.Collections.Generic;
using System.Linq;

namespace Rogue.NET.Core.Processing.Service.Validation.ValidationRule.Alteration
{
    public class AlterationCategoryValidationRule : IScenarioValidationRule
    {
        public AlterationCategoryValidationRule() { }

        public IEnumerable<IScenarioValidationResult> Validate(ScenarioConfigurationContainer configuration)
        {
            var alterations = configuration.GetAllAlterations();

            // TODO: Use Alteration Processing Container to split this one off
            var failedBlockAlterations = alterations.Where(x => x.Alteration.Effect is BlockAlterationAlterationEffectTemplate)
                                                    .Where(x => !configuration.AlterationCategories.Contains((x.Alteration.Effect as BlockAlterationAlterationEffectTemplate).AlterationCategory))
                                                    .Actualize();

            var failedAlterations = alterations.Where(x => x.Alteration.AlterationCategory == null ||
                                                          !configuration.AlterationCategories.Contains(x.Alteration.AlterationCategory))
                                               .Union(failedBlockAlterations);

            return failedAlterations.Select(x =>
                new ScenarioValidationResult()
                {
                    Asset = x.Asset,
                    Message = "All Effects must have a valid Effect Category",
                    Severity = ValidationSeverity.Error,
                    Passed = false,
                    InnerMessage = x.Asset.Name + " -> " + x.Alteration.Name + " has no valid Alteration Category (OR Block Alteration Category is not properly set)"

                }).Actualize();
        }
    }
}
