using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.Core.Model.Scenario.Alteration.Consumable;
using Rogue.NET.Core.Model.Scenario.Alteration.Doodad;
using Rogue.NET.Core.Model.Scenario.Alteration.Enemy;
using Rogue.NET.Core.Model.Scenario.Alteration.Equipment;
using Rogue.NET.Core.Model.Scenario.Alteration.Skill;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Consumable;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Doodad;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Enemy;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Equipment;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Skill;

namespace Rogue.NET.Core.Model.Generator.Interface
{
    /// <summary>
    /// Generates alteration containers (cost, effect, parameters)
    /// </summary>
    public interface IAlterationGenerator
    {
        AlterationCost GenerateAlterationCost(AlterationCostTemplate template);

        ConsumableAlteration GenerateAlteration(ConsumableAlterationTemplate template);
        ConsumableProjectileAlteration GenerateAlteration(ConsumableProjectileAlterationTemplate template);
        DoodadAlteration GenerateAlteration(DoodadAlterationTemplate template);
        EnemyAlteration GenerateAlteration(EnemyAlterationTemplate template);
        EquipmentAttackAlteration GenerateAlteration(EquipmentAttackAlterationTemplate template);
        EquipmentCurseAlteration GenerateAlteration(EquipmentCurseAlterationTemplate template);
        EquipmentEquipAlteration GenerateAlteration(EquipmentEquipAlterationTemplate template);
        SkillAlteration GenerateAlteration(SkillAlterationTemplate template);
    }
}
