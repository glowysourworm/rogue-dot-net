using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Model.Scenario.Content.Item;
using Rogue.NET.Core.Model.ScenarioConfiguration.Content;
using System.Collections.Generic;

namespace Rogue.NET.Core.Model.Generator.Interface
{
    public interface IItemGenerator
    {
        Equipment GenerateEquipment(EquipmentTemplate equipmentTemplate, IEnumerable<Religion> religions);

        Consumable GenerateConsumable(ConsumableTemplate consumableTemplate, IEnumerable<Religion> religions);

        /// <summary>
        /// Generates a single consumable for a given probability 0 < x < 1
        /// </summary>
        Consumable GenerateProbabilityConsumable(ProbabilityConsumableTemplate probabilityTemplate, IEnumerable<Religion> religions);

        /// <summary>
        /// Generates a single consumable for a given probability 0 < x < 1
        /// </summary>
        Equipment GenerateProbabilityEquipment(ProbabilityEquipmentTemplate probabilityTemplate, IEnumerable<Religion> religions, bool equipOnStartup = false);
    }
}
