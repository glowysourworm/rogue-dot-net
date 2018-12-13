using Rogue.NET.ScenarioEditor.ViewModel.Difficulty.Interface;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration;
using System.Collections.Generic;

namespace Rogue.NET.ScenarioEditor.Service.Interface
{
    /// <summary>
    /// Service that calculates projections about the scenario. This does not take into account any Alteration data
    /// except for the required Food calculations. These will be based ONLY on consumable items of type Food - Permanent
    /// Alteration to Source.
    /// </summary>
    public interface IScenarioDifficultyCalculationService
    {
        /// <summary>
        /// Calculates food consumption minus food availability on the assumption that the player eats each 
        /// time their Hunger meter reaches 100.
        /// </summary>
        /// <param name="configuration">Scenario Configuration</param>
        /// <param name="includedAssets">Set of included assets in the calculation. This must include the food assets.</param>
        /// <returns>Projected hunger curve</returns>
        IEnumerable<IProjectedQuantityViewModel> CalculateHungerCurve(
            ScenarioConfigurationContainerViewModel configuration, 
            IEnumerable<IDifficultyAssetViewModel> includedAssets);

        /// <summary>
        /// Calculates food availability per level based on generated food assets and enemy drops.
        /// </summary>
        /// <param name="configuration">Scenario Configuration</param>
        /// <param name="includedAssets">Set of included assets in the calculation. This must include the food assets.</param>
        /// <returns>Projected food availability curve</returns>
        IEnumerable<IProjectedQuantityViewModel> CalculateFoodAvailability(
            ScenarioConfigurationContainerViewModel configuration,
            IEnumerable<IDifficultyAssetViewModel> includedAssets);

        /// <summary>
        /// Calculates food conumption per level based on layout parameters.
        /// </summary>
        /// <param name="configuration">Scenario Configuration</param>
        /// <param name="includedAssets">Set of included assets in the calculation</param>
        /// <returns>Projected food availability curve</returns>
        IEnumerable<IProjectedQuantityViewModel> CalculateFoodConsumption(
            ScenarioConfigurationContainerViewModel configuration,
            IEnumerable<IDifficultyAssetViewModel> includedAssets);

        /// <summary>
        /// Calculates Enemy Hp per level (absolute low, absolute high, and average)
        /// </summary>
        /// <param name="configuration">Scenario Configuration</param>
        /// <param name="includedAssets">Set of included assets in the calculation</param>
        /// <returns>Projected enemy Hp curve</returns>
        IEnumerable<IProjectedQuantityViewModel> CalculateEnemyHp(
            ScenarioConfigurationContainerViewModel configuration,
            IEnumerable<IDifficultyAssetViewModel> includedAssets);

        /// <summary>
        /// Calculates Player Hp per level (absolute low, absolute high, and average).
        /// </summary>
        /// <param name="configuration">Scenario Configuration</param>
        /// <param name="includedAssets">Set of included assets in the calculation</param>
        /// <param name="usePlayerStrengthAttributeEmphasis">Assume Player uses Strength Attribute Emphasis</param>
        /// <returns>Projected Player Hp curve</returns>
        IEnumerable<IProjectedQuantityViewModel> CalculatePlayerHp(
            ScenarioConfigurationContainerViewModel configuration,
            IEnumerable<IDifficultyAssetViewModel> includedAssets,
            bool usePlayerStrengthAttributeEmphasis);

        /// <summary>
        /// Calculates Player Projected Experience
        /// </summary>
        /// <param name="configuration">Scenario Configuration</param>
        /// <param name="includedAssets">Set of included assets in the calculation</param>
        /// <returns>Projected Player Experience curve</returns>
        IEnumerable<IProjectedQuantityViewModel> CalculatePlayerExperience(
            ScenarioConfigurationContainerViewModel configuration,
            IEnumerable<IDifficultyAssetViewModel> includedAssets);

        /// <summary>
        /// Calculates Player Projected Level
        /// </summary>
        /// <param name="configuration">Scenario Configuration</param>
        /// <param name="includedAssets">Set of included assets in the calculation</param>
        /// <returns>Projected Player Level curve</returns>
        IEnumerable<IProjectedQuantityViewModel> CalculatePlayerLevel(
            ScenarioConfigurationContainerViewModel configuration,
            IEnumerable<IDifficultyAssetViewModel> includedAssets);

        /// <summary>
        /// Calculates Player projected attack power = Player Attack - Enemy Defense
        /// </summary>
        /// <param name="configuration">Scenario Configuration</param>
        /// <param name="includedAssets">Set of included assets in the calculation</param>
        /// <returns>Projected Player Attack Power Curve</returns>
        IEnumerable<IProjectedQuantityViewModel> CalculatePlayerAttackPower(
            ScenarioConfigurationContainerViewModel configuration,
            IEnumerable<IDifficultyAssetViewModel> includedAssets,
            bool usePlayerStrengthAttributeEmphasis,
            bool includeAttackAttributes);

        /// <summary>
        /// Calculates Enemy projected attack power = Enemy Attack - Player Defense
        /// </summary>
        /// <param name="configuration">Scenario Configuration</param>
        /// <param name="includedAssets">Set of included assets in the calculation</param>
        /// <returns>Projected Player Attack Power Curve</returns>
        IEnumerable<IProjectedQuantityViewModel> CalculateEnemyAttackPower(
            ScenarioConfigurationContainerViewModel configuration,
            IEnumerable<IDifficultyAssetViewModel> includedAssets,
            bool usePlayerStrengthAttributeEmphasis,
            bool includeAttackAttributes);
    }
}
