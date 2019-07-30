using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Model.Scenario.Content.Item;
using Rogue.NET.Core.Model.ScenarioConfiguration.Content;
using System.Collections.Generic;

namespace Rogue.NET.Core.Model.Generator.Interface
{
    public interface IItemGenerator
    {
        Equipment GenerateEquipment(EquipmentTemplate equipmentTemplate, IEnumerable<CharacterClass> characterClasses);

        Consumable GenerateConsumable(ConsumableTemplate consumableTemplate, IEnumerable<CharacterClass> characterClasses);

        /// <summary>
        /// Generates a single consumable for a given probability 0 < x < 1
        /// </summary>
        Consumable GenerateProbabilityConsumable(ProbabilityConsumableTemplate probabilityTemplate, IEnumerable<CharacterClass> characterClasses);

        /// <summary>
        /// Generates a single consumable for a given probability 0 < x < 1
        /// </summary>
        Equipment GenerateProbabilityEquipment(ProbabilityEquipmentTemplate probabilityTemplate, IEnumerable<CharacterClass> characterClasses, bool equipOnStartup = false);
    }
}
