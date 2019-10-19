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
    public class ItemRandomSymbolValidationRule : IScenarioValidationRule
    {
        public ItemRandomSymbolValidationRule() { }

        public IEnumerable<IScenarioValidationResult> Validate(ScenarioConfigurationContainer configuration)
        {
            var symbolPoolCategories = configuration.SymbolPool.Select(x => x.SymbolPoolCategory).Actualize();

            // This only affects randomizable symbols: Consumables, Equipment, and Doodads
            var nonCategorizedSymbols = configuration.ConsumableTemplates.Cast<DungeonObjectTemplate>()
                                                     .Union(configuration.EquipmentTemplates)
                                                     .Union(configuration.DoodadTemplates)
                                                     .Where(x => x.SymbolDetails.Randomize)
                                                     .Where(x => !symbolPoolCategories.Contains(x.SymbolDetails.SymbolPoolCategory))
                                                     .Actualize();


            return nonCategorizedSymbols.Select(x =>
                new ScenarioValidationResult()
                {
                    Asset = x,
                    Message = "Randomized Symbols MUST have a valid Symbol Category",
                    Severity = ValidationSeverity.Error,
                    Passed = false,
                    InnerMessage = x.Name + " has no valid Symbol Category"

                }).Actualize();
        }
    }
}
