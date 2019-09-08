using Rogue.NET.Core.Processing.Model.Static;
using Rogue.NET.ScenarioEditor.Service.Interface;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Content;
using System.ComponentModel.Composition;
using System.Linq;

namespace Rogue.NET.ScenarioEditor.Service
{
    [Export(typeof(ICharacterCalculationService))]
    public class CharacterCalculationService : ICharacterCalculationService
    {
        public RangeViewModel<double> CalculateEnemyAttack(EnemyTemplateViewModel enemyTemplate)
        {
            // Low Equipment Value
            var lowAttackValue = enemyTemplate.StartingEquipment
                                              .Where(x => EquipmentCalculator.IsWeaponType(x.TheTemplate.Type) && 
                                                          x.EquipOnStartup && 
                                                          x.GenerationProbability >= 1)
                                              .Sum(x => EquipmentCalculator.GetAttackValue(
                                                         x.TheTemplate.Type,
                                                         x.TheTemplate.Class.Low,
                                                         x.TheTemplate.Quality.Low));

            // High Equipment Value
            var highAttackValue = enemyTemplate.StartingEquipment
                                               .Where(x => EquipmentCalculator.IsWeaponType(x.TheTemplate.Type) &&
                                                          x.EquipOnStartup &&
                                                          x.GenerationProbability > 0)
                                               .Sum(x => EquipmentCalculator.GetAttackValue(
                                                         x.TheTemplate.Type,
                                                         x.TheTemplate.Class.High,
                                                         x.TheTemplate.Quality.High));

            var lowBaseAttack = MeleeCalculator.GetAttackValue(lowAttackValue, enemyTemplate.Strength.Low);
            var highBaseAttack = MeleeCalculator.GetAttackValue(highAttackValue, enemyTemplate.Strength.High);

            return new RangeViewModel<double>(lowBaseAttack, highBaseAttack);
        }

        public RangeViewModel<double> CalculateEnemyDefense(EnemyTemplateViewModel enemyTemplate)
        {
            // Low Equipment Value
            var lowDefenseValue = enemyTemplate.StartingEquipment
                                               .Where(x => EquipmentCalculator.IsArmorType(x.TheTemplate.Type) &&
                                                           x.EquipOnStartup &&
                                                           x.GenerationProbability >= 1)
                                               .Sum(x => EquipmentCalculator.GetDefenseValue(
                                                           x.TheTemplate.Type,
                                                           x.TheTemplate.Class.Low,
                                                           x.TheTemplate.Quality.Low));

            // High Equipment Value
            var highDefenseValue = enemyTemplate.StartingEquipment
                                                .Where(x => EquipmentCalculator.IsArmorType(x.TheTemplate.Type) &&
                                                           x.EquipOnStartup &&
                                                           x.GenerationProbability > 0)
                                                .Sum(x => EquipmentCalculator.GetDefenseValue(
                                                           x.TheTemplate.Type,
                                                           x.TheTemplate.Class.High,
                                                           x.TheTemplate.Quality.High));

            var lowBaseDefense = MeleeCalculator.GetDefenseValue(lowDefenseValue, enemyTemplate.Strength.Low);
            var highBaseDefense = MeleeCalculator.GetDefenseValue(highDefenseValue, enemyTemplate.Strength.High);

            return new RangeViewModel<double>(lowBaseDefense, highBaseDefense);
        }
    }
}
