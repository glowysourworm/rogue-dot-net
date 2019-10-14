using Rogue.NET.Core.Model.Scenario;
using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.Core.Model.Scenario.Alteration.Consumable;
using Rogue.NET.Core.Model.Scenario.Alteration.Doodad;
using Rogue.NET.Core.Model.Scenario.Alteration.Enemy;
using Rogue.NET.Core.Model.Scenario.Alteration.Equipment;
using Rogue.NET.Core.Model.Scenario.Alteration.Friendly;
using Rogue.NET.Core.Model.Scenario.Alteration.Skill;
using Rogue.NET.Core.Model.Scenario.Alteration.TemporaryCharacter;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Consumable;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Doodad;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Enemy;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Equipment;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Friendly;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Skill;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.TemporaryCharacter;
using System.Collections.Generic;

namespace Rogue.NET.Core.Processing.Model.Generator.Interface
{
    /// <summary>
    /// Generates alteration containers (cost, effect, parameters)
    /// </summary>
    public interface IAlterationGenerator
    {
        AlterationCost GenerateAlterationCost(AlterationCostTemplate template);

        AlterationContainer GenerateAlteration(AlterationTemplate template);
        ConsumableAlteration GenerateAlteration(ConsumableAlterationTemplate template);
        ConsumableProjectileAlteration GenerateAlteration(ConsumableProjectileAlterationTemplate template);
        DoodadAlteration GenerateAlteration(DoodadAlterationTemplate template);
        EnemyAlteration GenerateAlteration(EnemyAlterationTemplate template);
        FriendlyAlteration GenerateAlteration(FriendlyAlterationTemplate template);
        EquipmentAttackAlteration GenerateAlteration(EquipmentAttackAlterationTemplate template);
        EquipmentCurseAlteration GenerateAlteration(EquipmentCurseAlterationTemplate template);
        EquipmentEquipAlteration GenerateAlteration(EquipmentEquipAlterationTemplate template);
        TemporaryCharacterAlteration GenerateAlteration(TemporaryCharacterAlterationTemplate template);
        SkillAlteration GenerateAlteration(SkillAlterationTemplate template);
    }
}
