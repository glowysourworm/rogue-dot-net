using Rogue.NET.Core.Model.Scenario.Content.Item;
using Rogue.NET.Core.Model.ScenarioConfiguration.Content;

namespace Rogue.NET.Core.Model.Generator.Interface
{
    public interface IItemGenerator
    {
        Equipment GenerateEquipment(EquipmentTemplate equipmentTemplate);

        Consumable GenerateConsumable(ConsumableTemplate consumableTemplate);

        /// <summary>
        /// Generates a single consumable for a given probability 0 < x < 1
        /// </summary>
        Consumable GenerateProbabilityConsumable(ProbabilityConsumableTemplate probabilityTemplate);

        /// <summary>
        /// Generates a single consumable for a given probability 0 < x < 1
        /// </summary>
        Equipment GenerateProbabilityEquipment(ProbabilityEquipmentTemplate probabilityTemplate, bool equipOnStartup = false);
    }
}
