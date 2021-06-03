using Rogue.NET.Common.Collection;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.Core.Model.Scenario.Alteration.Consumable;
using Rogue.NET.Core.Model.Scenario.Alteration.Doodad;
using Rogue.NET.Core.Model.Scenario.Alteration.Effect;
using Rogue.NET.Core.Model.Scenario.Alteration.Enemy;
using Rogue.NET.Core.Model.Scenario.Alteration.Equipment;
using Rogue.NET.Core.Model.Scenario.Alteration.Interface;
using Rogue.NET.Core.Model.Scenario.Alteration.Skill;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Common;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Consumable;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Doodad;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Enemy;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Equipment;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Interface;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Skill;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Model.Static
{
    /// <summary>
    /// Static container for some type data on ALL ALTERATION TYPES. This was built because attribute
    /// specifiers got to be too much. This should handle lookups for Block Type and Cost Type.
    /// </summary>
    public static class AlterationSpecificationContainer
    {
        // Alteration Interface Type -> Alteration Effect Interface Type -> Alteration Cost Type
        readonly static SimpleDictionary<Type, SimpleDictionary<Type, AlterationCostType>> _alterationCostTypes;
        readonly static SimpleDictionary<Type, SimpleDictionary<Type, bool>> _alterationBlockingSupport;

        // (Duplicate is necessary for showing data on the UI...)
        readonly static SimpleDictionary<Type, SimpleDictionary<Type, AlterationCostType>> _alterationTemplateCostTypes;
        readonly static SimpleDictionary<Type, SimpleDictionary<Type, bool>> _alterationTemplateBlockingSupport;

        static AlterationSpecificationContainer()
        {
            _alterationCostTypes = new SimpleDictionary<Type, SimpleDictionary<Type, AlterationCostType>>();
            _alterationBlockingSupport = new SimpleDictionary<Type, SimpleDictionary<Type, bool>>();

            _alterationTemplateCostTypes = new SimpleDictionary<Type, SimpleDictionary<Type, AlterationCostType>>();
            _alterationTemplateBlockingSupport = new SimpleDictionary<Type, SimpleDictionary<Type, bool>>();

            // ISkillAlterationEffect -> AlterationCostType
            _alterationCostTypes.Add(
                typeof(ISkillAlterationEffect), new SimpleDictionary<Type, AlterationCostType>(){
                    { typeof(AttackAttributeAuraAlterationEffect), AlterationCostType.PerStep },
                    { typeof(AttackAttributeMeleeAlterationEffect), AlterationCostType.OneTime },
                    { typeof(AttackAttributePassiveAlterationEffect), AlterationCostType.PerStep },
                    { typeof(AttackAttributeTemporaryAlterationEffect), AlterationCostType.PerStep },
                    { typeof(AuraAlterationEffect), AlterationCostType.PerStep },
                    { typeof(BlockAlterationAlterationEffect), AlterationCostType.PerStep },
                    { typeof(ChangeLevelAlterationEffect), AlterationCostType.OneTime },
                    { typeof(CreateEnemyAlterationEffect), AlterationCostType.OneTime },
                    { typeof(CreateFriendlyAlterationEffect), AlterationCostType.OneTime },
                    { typeof(CreateTemporaryCharacterAlterationEffect), AlterationCostType.OneTime },
                    { typeof(DetectAlterationAlignmentAlterationEffect), AlterationCostType.OneTime },
                    { typeof(DetectAlterationAlterationEffect), AlterationCostType.OneTime },
                    { typeof(EquipmentEnhanceAlterationEffect), AlterationCostType.OneTime },
                    { typeof(IdentifyAlterationEffect), AlterationCostType.OneTime },
                    { typeof(PassiveAlterationEffect), AlterationCostType.PerStep },
                    { typeof(PermanentAlterationEffect), AlterationCostType.OneTime },
                    { typeof(RemedyAlterationEffect), AlterationCostType.OneTime },
                    { typeof(RevealAlterationEffect), AlterationCostType.OneTime },
                    { typeof(StealAlterationEffect), AlterationCostType.OneTime },
                    { typeof(TeleportManualAlterationEffect), AlterationCostType.OneTime },
                    { typeof(TeleportRandomAlterationEffect), AlterationCostType.OneTime },
                    { typeof(TemporaryAlterationEffect), AlterationCostType.OneTime },
                    { typeof(TransmuteAlterationEffect), AlterationCostType.OneTime },
                    { typeof(UncurseAlterationEffect), AlterationCostType.OneTime }
                });
            // IEnemyAlterationEffect -> AlterationCostType
            _alterationCostTypes.Add(
                typeof(IEnemyAlterationEffect), new SimpleDictionary<Type, AlterationCostType>(){
                    { typeof(AttackAttributeMeleeAlterationEffect), AlterationCostType.OneTime },
                    { typeof(AttackAttributeTemporaryAlterationEffect), AlterationCostType.OneTime },
                    { typeof(CreateEnemyAlterationEffect), AlterationCostType.OneTime },
                    { typeof(CreateFriendlyAlterationEffect), AlterationCostType.OneTime },
                    { typeof(CreateTemporaryCharacterAlterationEffect), AlterationCostType.OneTime },
                    { typeof(EquipmentDamageAlterationEffect), AlterationCostType.OneTime },
                    { typeof(PermanentAlterationEffect), AlterationCostType.OneTime },
                    { typeof(StealAlterationEffect), AlterationCostType.OneTime },
                    { typeof(RunAwayAlterationEffect), AlterationCostType.OneTime },
                    { typeof(TeleportRandomAlterationEffect), AlterationCostType.OneTime },
                    { typeof(TemporaryAlterationEffect), AlterationCostType.OneTime }
                });
            // IFriendlyAlterationEffect -> AlterationCostType
            _alterationCostTypes.Add(
                typeof(IFriendlyAlterationEffect), new SimpleDictionary<Type, AlterationCostType>(){
                    { typeof(AttackAttributeMeleeAlterationEffect), AlterationCostType.None },
                    { typeof(AttackAttributeTemporaryAlterationEffect), AlterationCostType.None },
                    { typeof(PermanentAlterationEffect), AlterationCostType.None },
                    { typeof(TemporaryAlterationEffect), AlterationCostType.None }
                });
            // ITemporaryCharacterAlterationEffect -> AlterationCostType
            _alterationCostTypes.Add(
                typeof(ITemporaryCharacterAlterationEffect), new SimpleDictionary<Type, AlterationCostType>(){
                    { typeof(AttackAttributeMeleeAlterationEffect), AlterationCostType.None },
                    { typeof(AttackAttributeTemporaryAlterationEffect), AlterationCostType.None },
                    { typeof(PermanentAlterationEffect), AlterationCostType.None },
                    { typeof(TemporaryAlterationEffect), AlterationCostType.None }
                });
            // IEquipmentAttackAlterationEffect -> AlterationCostType
            _alterationCostTypes.Add(
                typeof(IEquipmentAttackAlterationEffect), new SimpleDictionary<Type, AlterationCostType>(){
                    { typeof(AttackAttributeMeleeAlterationEffect), AlterationCostType.OneTime },
                    { typeof(DrainMeleeAlterationEffect), AlterationCostType.OneTime },
                    { typeof(PermanentAlterationEffect), AlterationCostType.OneTime }
                });
            // IEquipmentEquipAlterationEffect -> AlterationCostType
            _alterationCostTypes.Add(
                typeof(IEquipmentEquipAlterationEffect), new SimpleDictionary<Type, AlterationCostType>(){
                    { typeof(AttackAttributeAuraAlterationEffect), AlterationCostType.PerStep },
                    { typeof(AttackAttributePassiveAlterationEffect), AlterationCostType.PerStep },
                    { typeof(AuraAlterationEffect), AlterationCostType.PerStep },
                    { typeof(BlockAlterationAlterationEffect), AlterationCostType.PerStep },
                    { typeof(PassiveAlterationEffect), AlterationCostType.PerStep }
                });
            // IEquipmentCurseAlterationEffect -> AlterationCostType (NONE FOR CURSES)
            _alterationCostTypes.Add(
                typeof(IEquipmentCurseAlterationEffect), new SimpleDictionary<Type, AlterationCostType>(){
                    { typeof(AttackAttributeAuraAlterationEffect), AlterationCostType.None },
                    { typeof(AttackAttributePassiveAlterationEffect), AlterationCostType.None },
                    { typeof(AuraAlterationEffect), AlterationCostType.None },
                    { typeof(PassiveAlterationEffect), AlterationCostType.None }
                });
            // IDoodadAlterationEffect -> AlterationCostType
            _alterationCostTypes.Add(
                typeof(IDoodadAlterationEffect), new SimpleDictionary<Type, AlterationCostType>(){
                    { typeof(AttackAttributeMeleeAlterationEffect), AlterationCostType.OneTime },
                    { typeof(AttackAttributeTemporaryAlterationEffect), AlterationCostType.OneTime },
                    { typeof(ChangeLevelAlterationEffect), AlterationCostType.OneTime },
                    { typeof(CreateEnemyAlterationEffect), AlterationCostType.OneTime },
                    { typeof(CreateFriendlyAlterationEffect), AlterationCostType.OneTime },
                    { typeof(CreateTemporaryCharacterAlterationEffect), AlterationCostType.OneTime },
                    { typeof(DetectAlterationAlignmentAlterationEffect), AlterationCostType.OneTime },
                    { typeof(DetectAlterationAlterationEffect), AlterationCostType.OneTime },
                    { typeof(EquipmentDamageAlterationEffect), AlterationCostType.OneTime },
                    { typeof(EquipmentEnhanceAlterationEffect), AlterationCostType.OneTime },
                    { typeof(IdentifyAlterationEffect), AlterationCostType.OneTime },
                    { typeof(PermanentAlterationEffect), AlterationCostType.OneTime },
                    { typeof(RemedyAlterationEffect), AlterationCostType.OneTime },
                    { typeof(RevealAlterationEffect), AlterationCostType.OneTime },
                    { typeof(TeleportRandomAlterationEffect), AlterationCostType.OneTime },
                    { typeof(TemporaryAlterationEffect), AlterationCostType.OneTime },
                    { typeof(TransmuteAlterationEffect), AlterationCostType.OneTime },
                    { typeof(UncurseAlterationEffect), AlterationCostType.OneTime }
                });
            // IConsumableAlterationEffect -> AlterationCostType
            _alterationCostTypes.Add(
                typeof(IConsumableAlterationEffect), new SimpleDictionary<Type, AlterationCostType>(){
                    { typeof(AttackAttributeMeleeAlterationEffect), AlterationCostType.OneTime },
                    { typeof(AttackAttributeTemporaryAlterationEffect), AlterationCostType.OneTime },
                    { typeof(ChangeLevelAlterationEffect), AlterationCostType.OneTime },
                    { typeof(CreateEnemyAlterationEffect), AlterationCostType.OneTime },
                    { typeof(CreateFriendlyAlterationEffect), AlterationCostType.OneTime },
                    { typeof(CreateTemporaryCharacterAlterationEffect), AlterationCostType.OneTime },
                    { typeof(DetectAlterationAlignmentAlterationEffect), AlterationCostType.OneTime },
                    { typeof(DetectAlterationAlterationEffect), AlterationCostType.OneTime },
                    { typeof(EquipmentDamageAlterationEffect), AlterationCostType.OneTime },
                    { typeof(EquipmentEnhanceAlterationEffect), AlterationCostType.OneTime },
                    { typeof(IdentifyAlterationEffect), AlterationCostType.OneTime },
                    { typeof(PermanentAlterationEffect), AlterationCostType.OneTime },
                    { typeof(RemedyAlterationEffect), AlterationCostType.OneTime },
                    { typeof(RevealAlterationEffect), AlterationCostType.OneTime },
                    { typeof(TeleportRandomAlterationEffect), AlterationCostType.OneTime },
                    { typeof(TemporaryAlterationEffect), AlterationCostType.OneTime },
                    { typeof(TransmuteAlterationEffect), AlterationCostType.OneTime },
                    { typeof(UncurseAlterationEffect), AlterationCostType.OneTime }
                });

            // IConsumableProjectileAlterationEffect -> AlterationCostType
            _alterationCostTypes.Add(
                typeof(IConsumableProjectileAlterationEffect), new SimpleDictionary<Type, AlterationCostType>(){
                    { typeof(AttackAttributeMeleeAlterationEffect), AlterationCostType.None },
                    { typeof(AttackAttributeTemporaryAlterationEffect), AlterationCostType.None },
                    { typeof(PermanentAlterationEffect), AlterationCostType.None },
                    { typeof(TemporaryAlterationEffect), AlterationCostType.None }
                });


            // ISkillAlterationEffect -> Block Support
            _alterationBlockingSupport.Add(
                typeof(ISkillAlterationEffect), new SimpleDictionary<Type, bool>(){
                    { typeof(AttackAttributeAuraAlterationEffect), false },
                    { typeof(AttackAttributeMeleeAlterationEffect), true },
                    { typeof(AttackAttributePassiveAlterationEffect), false },
                    { typeof(AttackAttributeTemporaryAlterationEffect), true },
                    { typeof(AuraAlterationEffect), false },
                    { typeof(BlockAlterationAlterationEffect), false },
                    { typeof(ChangeLevelAlterationEffect), false },
                    { typeof(CreateEnemyAlterationEffect), false },
                    { typeof(CreateFriendlyAlterationEffect), false },
                    { typeof(CreateTemporaryCharacterAlterationEffect), false },
                    { typeof(DetectAlterationAlignmentAlterationEffect), false },
                    { typeof(DetectAlterationAlterationEffect), false },
                    { typeof(EquipmentEnhanceAlterationEffect), false },
                    { typeof(IdentifyAlterationEffect), false },
                    { typeof(PassiveAlterationEffect), false },
                    { typeof(PermanentAlterationEffect), true },
                    { typeof(RemedyAlterationEffect), false },
                    { typeof(RevealAlterationEffect), false },
                    { typeof(StealAlterationEffect), true },
                    { typeof(TeleportManualAlterationEffect), false },
                    { typeof(TeleportRandomAlterationEffect), false },
                    { typeof(TemporaryAlterationEffect), true },
                    { typeof(TransmuteAlterationEffect), false },
                    { typeof(UncurseAlterationEffect), false }
                });
            // IEnemyAlterationEffect -> Block Support
            _alterationBlockingSupport.Add(
                typeof(IEnemyAlterationEffect), new SimpleDictionary<Type, bool>(){
                    { typeof(AttackAttributeMeleeAlterationEffect), true },
                    { typeof(AttackAttributeTemporaryAlterationEffect), true },
                    { typeof(CreateEnemyAlterationEffect), false },
                    { typeof(CreateFriendlyAlterationEffect), false },
                    { typeof(CreateTemporaryCharacterAlterationEffect), false },
                    { typeof(EquipmentDamageAlterationEffect), true },
                    { typeof(PermanentAlterationEffect), true },
                    { typeof(StealAlterationEffect), true },
                    { typeof(RunAwayAlterationEffect), false },
                    { typeof(TeleportRandomAlterationEffect), false },
                    { typeof(TemporaryAlterationEffect), true }
                });
            // IFriendlyAlterationEffect -> Block Support
            _alterationBlockingSupport.Add(
                typeof(IFriendlyAlterationEffect), new SimpleDictionary<Type, bool>(){
                    { typeof(AttackAttributeMeleeAlterationEffect), true },
                    { typeof(AttackAttributeTemporaryAlterationEffect), true },
                    { typeof(PermanentAlterationEffect), true },
                    { typeof(TemporaryAlterationEffect), true }
                });
            // ITemporaryCharacterAlterationEffect -> Block Support
            _alterationBlockingSupport.Add(
                typeof(ITemporaryCharacterAlterationEffect), new SimpleDictionary<Type, bool>(){
                    { typeof(AttackAttributeMeleeAlterationEffect), true },
                    { typeof(AttackAttributeTemporaryAlterationEffect), true },
                    { typeof(PermanentAlterationEffect), true },
                    { typeof(TemporaryAlterationEffect), true }
                });
            // IEquipmentAttackAlterationEffect -> Block Support
            _alterationBlockingSupport.Add(
                typeof(IEquipmentAttackAlterationEffect), new SimpleDictionary<Type, bool>(){
                    { typeof(AttackAttributeMeleeAlterationEffect), false },
                    { typeof(DrainMeleeAlterationEffect), false },
                    { typeof(PermanentAlterationEffect), false }
                });
            // IEquipmentEquipAlterationEffect -> Block Support (NONE FOR PASSIVES)
            _alterationBlockingSupport.Add(
                typeof(IEquipmentEquipAlterationEffect), new SimpleDictionary<Type, bool>(){
                    { typeof(AttackAttributeAuraAlterationEffect), false },
                    { typeof(AttackAttributePassiveAlterationEffect), false },
                    { typeof(BlockAlterationAlterationEffect), false },
                    { typeof(AuraAlterationEffect), false },
                    { typeof(PassiveAlterationEffect), false }
                });
            // IEquipmentCurseAlterationEffect -> Block Support (NONE FOR CURSES)
            _alterationBlockingSupport.Add(
                typeof(IEquipmentCurseAlterationEffect), new SimpleDictionary<Type, bool>(){
                    { typeof(AttackAttributeAuraAlterationEffect), false },
                    { typeof(AttackAttributePassiveAlterationEffect), false },
                    { typeof(AuraAlterationEffect), false },
                    { typeof(PassiveAlterationEffect), false }
                });
            // IDoodadAlterationEffect -> Block Support (NONE FOR DOODADS)
            _alterationBlockingSupport.Add(
                typeof(IDoodadAlterationEffect), new SimpleDictionary<Type, bool>(){
                    { typeof(AttackAttributeMeleeAlterationEffect), false },
                    { typeof(AttackAttributeTemporaryAlterationEffect), false },
                    { typeof(ChangeLevelAlterationEffect), false },
                    { typeof(CreateEnemyAlterationEffect), false },
                    { typeof(CreateFriendlyAlterationEffect), false },
                    { typeof(CreateTemporaryCharacterAlterationEffect), false },
                    { typeof(DetectAlterationAlignmentAlterationEffect), false },
                    { typeof(DetectAlterationAlterationEffect), false },
                    { typeof(EquipmentDamageAlterationEffect), false },
                    { typeof(EquipmentEnhanceAlterationEffect), false },
                    { typeof(IdentifyAlterationEffect), false },
                    { typeof(PermanentAlterationEffect), false },
                    { typeof(RemedyAlterationEffect), false },
                    { typeof(RevealAlterationEffect), false },
                    { typeof(TeleportRandomAlterationEffect), false },
                    { typeof(TemporaryAlterationEffect), false },
                    { typeof(TransmuteAlterationEffect), false },
                    { typeof(UncurseAlterationEffect), false }
                });
            // IConsumableAlterationEffect -> Block Support (NONE FOR DOODADS)
            _alterationBlockingSupport.Add(
                typeof(IConsumableAlterationEffect), new SimpleDictionary<Type, bool>(){
                    { typeof(AttackAttributeMeleeAlterationEffect), false },
                    { typeof(AttackAttributeTemporaryAlterationEffect), false },
                    { typeof(ChangeLevelAlterationEffect), false },
                    { typeof(CreateEnemyAlterationEffect), false },
                    { typeof(CreateFriendlyAlterationEffect), false },
                    { typeof(CreateTemporaryCharacterAlterationEffect), false },
                    { typeof(DetectAlterationAlignmentAlterationEffect), false },
                    { typeof(DetectAlterationAlterationEffect), false },
                    { typeof(EquipmentDamageAlterationEffect), false },
                    { typeof(EquipmentEnhanceAlterationEffect), false },
                    { typeof(IdentifyAlterationEffect), false },
                    { typeof(PermanentAlterationEffect), false },
                    { typeof(RemedyAlterationEffect), false },
                    { typeof(RevealAlterationEffect), false },
                    { typeof(TeleportRandomAlterationEffect), false },
                    { typeof(TemporaryAlterationEffect), false },
                    { typeof(TransmuteAlterationEffect), false },
                    { typeof(UncurseAlterationEffect), false }
                });
            // IConsumableProjectileAlterationEffect -> Block Support (NONE FOR PROJECTILES)
            _alterationBlockingSupport.Add(
                typeof(IConsumableProjectileAlterationEffect), new SimpleDictionary<Type, bool>(){
                    { typeof(AttackAttributeMeleeAlterationEffect), false },
                    { typeof(AttackAttributeTemporaryAlterationEffect), false },
                    { typeof(PermanentAlterationEffect), false },
                    { typeof(TemporaryAlterationEffect), false }
                });



            // ISkillAlterationEffectTemplate -> AlterationCostType
            _alterationTemplateCostTypes.Add(
                typeof(ISkillAlterationEffectTemplate), new SimpleDictionary<Type, AlterationCostType>(){
                    { typeof(AttackAttributeAuraAlterationEffectTemplate), AlterationCostType.PerStep },
                    { typeof(AttackAttributeMeleeAlterationEffectTemplate), AlterationCostType.OneTime },
                    { typeof(AttackAttributePassiveAlterationEffectTemplate), AlterationCostType.PerStep },
                    { typeof(AttackAttributeTemporaryAlterationEffectTemplate), AlterationCostType.PerStep },
                    { typeof(AuraAlterationEffectTemplate), AlterationCostType.PerStep },
                    { typeof(BlockAlterationAlterationEffectTemplate), AlterationCostType.PerStep },
                    { typeof(ChangeLevelAlterationEffectTemplate), AlterationCostType.OneTime },
                    { typeof(CreateEnemyAlterationEffectTemplate), AlterationCostType.OneTime },
                    { typeof(CreateFriendlyAlterationEffectTemplate), AlterationCostType.OneTime },
                    { typeof(CreateTemporaryCharacterAlterationEffectTemplate), AlterationCostType.OneTime },
                    { typeof(DetectAlterationAlignmentAlterationEffectTemplate), AlterationCostType.OneTime },
                    { typeof(DetectAlterationAlterationEffectTemplate), AlterationCostType.OneTime },
                    { typeof(EquipmentEnhanceAlterationEffectTemplate), AlterationCostType.OneTime },
                    { typeof(IdentifyAlterationEffectTemplate), AlterationCostType.OneTime },
                    { typeof(PassiveAlterationEffectTemplate), AlterationCostType.PerStep },
                    { typeof(PermanentAlterationEffectTemplate), AlterationCostType.OneTime },
                    { typeof(RemedyAlterationEffectTemplate), AlterationCostType.OneTime },
                    { typeof(RevealAlterationEffectTemplate), AlterationCostType.OneTime },
                    { typeof(StealAlterationEffectTemplate), AlterationCostType.OneTime },
                    { typeof(TeleportManualAlterationEffectTemplate), AlterationCostType.OneTime },
                    { typeof(TeleportRandomAlterationEffectTemplate), AlterationCostType.OneTime },
                    { typeof(TemporaryAlterationEffectTemplate), AlterationCostType.OneTime },
                    { typeof(TransmuteAlterationEffectTemplate), AlterationCostType.OneTime },
                    { typeof(UncurseAlterationEffectTemplate), AlterationCostType.OneTime }
                });
            // IEnemyAlterationEffectTemplate -> AlterationCostType
            _alterationTemplateCostTypes.Add(
                typeof(IEnemyAlterationEffectTemplate), new SimpleDictionary<Type, AlterationCostType>(){
                    { typeof(AttackAttributeMeleeAlterationEffectTemplate), AlterationCostType.OneTime },
                    { typeof(AttackAttributeTemporaryAlterationEffectTemplate), AlterationCostType.OneTime },
                    { typeof(CreateEnemyAlterationEffectTemplate), AlterationCostType.OneTime },
                    { typeof(CreateFriendlyAlterationEffectTemplate), AlterationCostType.OneTime },
                    { typeof(CreateTemporaryCharacterAlterationEffectTemplate), AlterationCostType.OneTime },
                    { typeof(EquipmentDamageAlterationEffectTemplate), AlterationCostType.OneTime },
                    { typeof(PermanentAlterationEffectTemplate), AlterationCostType.OneTime },
                    { typeof(StealAlterationEffectTemplate), AlterationCostType.OneTime },
                    { typeof(RunAwayAlterationEffectTemplate), AlterationCostType.OneTime },
                    { typeof(TeleportRandomAlterationEffectTemplate), AlterationCostType.OneTime },
                    { typeof(TemporaryAlterationEffectTemplate), AlterationCostType.OneTime }
                });
            // IFriendlyAlterationEffect -> AlterationCostType
            _alterationTemplateCostTypes.Add(
                typeof(IFriendlyAlterationEffectTemplate), new SimpleDictionary<Type, AlterationCostType>(){
                    { typeof(AttackAttributeMeleeAlterationEffectTemplate), AlterationCostType.OneTime },
                    { typeof(AttackAttributeTemporaryAlterationEffectTemplate), AlterationCostType.OneTime },
                    { typeof(PermanentAlterationEffectTemplate), AlterationCostType.OneTime },
                    { typeof(TemporaryAlterationEffectTemplate), AlterationCostType.OneTime }
                });
            // ITemporaryCharacterAlterationEffect -> AlterationCostType
            _alterationTemplateCostTypes.Add(
                typeof(ITemporaryCharacterAlterationEffectTemplate), new SimpleDictionary<Type, AlterationCostType>(){
                    { typeof(AttackAttributeMeleeAlterationEffectTemplate), AlterationCostType.OneTime },
                    { typeof(AttackAttributeTemporaryAlterationEffectTemplate), AlterationCostType.OneTime },
                    { typeof(PermanentAlterationEffectTemplate), AlterationCostType.OneTime },
                    { typeof(TemporaryAlterationEffectTemplate), AlterationCostType.OneTime }
                });
            // IEquipmentAttackAlterationEffectTemplate -> AlterationCostType
            _alterationTemplateCostTypes.Add(
                typeof(IEquipmentAttackAlterationEffectTemplate), new SimpleDictionary<Type, AlterationCostType>(){
                    { typeof(AttackAttributeMeleeAlterationEffectTemplate), AlterationCostType.OneTime },
                    { typeof(DrainMeleeAlterationEffectTemplate), AlterationCostType.OneTime },
                    { typeof(PermanentAlterationEffectTemplate), AlterationCostType.OneTime }
                });
            // IEquipmentEquipAlterationEffectTemplate -> AlterationCostType
            _alterationTemplateCostTypes.Add(
                typeof(IEquipmentEquipAlterationEffectTemplate), new SimpleDictionary<Type, AlterationCostType>(){
                    { typeof(AttackAttributeAuraAlterationEffectTemplate), AlterationCostType.PerStep },
                    { typeof(AttackAttributePassiveAlterationEffectTemplate), AlterationCostType.PerStep },
                    { typeof(AuraAlterationEffectTemplate), AlterationCostType.PerStep },
                    { typeof(BlockAlterationAlterationEffectTemplate), AlterationCostType.PerStep },
                    { typeof(PassiveAlterationEffectTemplate), AlterationCostType.PerStep }
                });
            // IEquipmentCurseAlterationEffectTemplate -> AlterationCostType (NONE FOR CURSES)
            _alterationTemplateCostTypes.Add(
                typeof(IEquipmentCurseAlterationEffectTemplate), new SimpleDictionary<Type, AlterationCostType>(){
                    { typeof(AttackAttributeAuraAlterationEffectTemplate), AlterationCostType.None },
                    { typeof(AttackAttributePassiveAlterationEffectTemplate), AlterationCostType.None },
                    { typeof(AuraAlterationEffectTemplate), AlterationCostType.None },
                    { typeof(PassiveAlterationEffectTemplate), AlterationCostType.None }
                });
            // IDoodadAlterationEffectTemplate -> AlterationCostType
            _alterationTemplateCostTypes.Add(
                typeof(IDoodadAlterationEffectTemplate), new SimpleDictionary<Type, AlterationCostType>(){
                    { typeof(AttackAttributeMeleeAlterationEffectTemplate), AlterationCostType.OneTime },
                    { typeof(AttackAttributeTemporaryAlterationEffectTemplate), AlterationCostType.OneTime },
                    { typeof(ChangeLevelAlterationEffectTemplate), AlterationCostType.OneTime },
                    { typeof(CreateEnemyAlterationEffectTemplate), AlterationCostType.OneTime },
                    { typeof(CreateFriendlyAlterationEffectTemplate), AlterationCostType.OneTime },
                    { typeof(CreateTemporaryCharacterAlterationEffectTemplate), AlterationCostType.OneTime },
                    { typeof(DetectAlterationAlignmentAlterationEffectTemplate), AlterationCostType.OneTime },
                    { typeof(DetectAlterationAlterationEffectTemplate), AlterationCostType.OneTime },
                    { typeof(EquipmentDamageAlterationEffectTemplate), AlterationCostType.OneTime },
                    { typeof(EquipmentEnhanceAlterationEffectTemplate), AlterationCostType.OneTime },
                    { typeof(IdentifyAlterationEffectTemplate), AlterationCostType.OneTime },
                    { typeof(PermanentAlterationEffectTemplate), AlterationCostType.OneTime },
                    { typeof(RemedyAlterationEffectTemplate), AlterationCostType.OneTime },
                    { typeof(RevealAlterationEffectTemplate), AlterationCostType.OneTime },
                    { typeof(TeleportRandomAlterationEffectTemplate), AlterationCostType.OneTime },
                    { typeof(TemporaryAlterationEffectTemplate), AlterationCostType.OneTime },
                    { typeof(TransmuteAlterationEffectTemplate), AlterationCostType.OneTime },
                    { typeof(UncurseAlterationEffectTemplate), AlterationCostType.OneTime }
                });
            // IConsumableAlterationEffectTemplate -> AlterationCostType
            _alterationTemplateCostTypes.Add(
                typeof(IConsumableAlterationEffectTemplate), new SimpleDictionary<Type, AlterationCostType>(){
                    { typeof(AttackAttributeMeleeAlterationEffectTemplate), AlterationCostType.OneTime },
                    { typeof(AttackAttributeTemporaryAlterationEffectTemplate), AlterationCostType.OneTime },
                    { typeof(ChangeLevelAlterationEffectTemplate), AlterationCostType.OneTime },
                    { typeof(CreateEnemyAlterationEffectTemplate), AlterationCostType.OneTime },
                    { typeof(CreateFriendlyAlterationEffectTemplate), AlterationCostType.OneTime },
                    { typeof(CreateTemporaryCharacterAlterationEffectTemplate), AlterationCostType.OneTime },
                    { typeof(DetectAlterationAlignmentAlterationEffectTemplate), AlterationCostType.OneTime },
                    { typeof(DetectAlterationAlterationEffectTemplate), AlterationCostType.OneTime },
                    { typeof(EquipmentDamageAlterationEffectTemplate), AlterationCostType.OneTime },
                    { typeof(EquipmentEnhanceAlterationEffectTemplate), AlterationCostType.OneTime },
                    { typeof(IdentifyAlterationEffectTemplate), AlterationCostType.OneTime },
                    { typeof(PermanentAlterationEffectTemplate), AlterationCostType.OneTime },
                    { typeof(RemedyAlterationEffectTemplate), AlterationCostType.OneTime },
                    { typeof(RevealAlterationEffectTemplate), AlterationCostType.OneTime },
                    { typeof(TeleportRandomAlterationEffectTemplate), AlterationCostType.OneTime },
                    { typeof(TemporaryAlterationEffectTemplate), AlterationCostType.OneTime },
                    { typeof(TransmuteAlterationEffectTemplate), AlterationCostType.OneTime },
                    { typeof(UncurseAlterationEffectTemplate), AlterationCostType.OneTime }
                });
            // IConsumableProjectileAlterationEffectTemplate -> AlterationCostType
            _alterationTemplateCostTypes.Add(
                typeof(IConsumableProjectileAlterationEffectTemplate), new SimpleDictionary<Type, AlterationCostType>(){
                    { typeof(AttackAttributeMeleeAlterationEffectTemplate), AlterationCostType.None },
                    { typeof(AttackAttributeTemporaryAlterationEffectTemplate), AlterationCostType.None },
                    { typeof(PermanentAlterationEffectTemplate), AlterationCostType.None },
                    { typeof(TemporaryAlterationEffectTemplate), AlterationCostType.None }
                });


            // ISkillAlterationEffectTemplate -> Block Support
            _alterationTemplateBlockingSupport.Add(
                typeof(ISkillAlterationEffectTemplate), new SimpleDictionary<Type, bool>(){
                    { typeof(AttackAttributeAuraAlterationEffectTemplate), false },
                    { typeof(AttackAttributeMeleeAlterationEffectTemplate), true },
                    { typeof(AttackAttributePassiveAlterationEffectTemplate), false },
                    { typeof(AttackAttributeTemporaryAlterationEffectTemplate), true },
                    { typeof(AuraAlterationEffectTemplate), false },
                    { typeof(BlockAlterationAlterationEffectTemplate), false },
                    { typeof(ChangeLevelAlterationEffectTemplate), false },
                    { typeof(CreateEnemyAlterationEffectTemplate), false },
                    { typeof(CreateFriendlyAlterationEffectTemplate), false },
                    { typeof(CreateTemporaryCharacterAlterationEffectTemplate), false },
                    { typeof(DetectAlterationAlignmentAlterationEffectTemplate), false },
                    { typeof(DetectAlterationAlterationEffectTemplate), false },
                    { typeof(EquipmentEnhanceAlterationEffectTemplate), false },
                    { typeof(IdentifyAlterationEffectTemplate), false },
                    { typeof(PassiveAlterationEffectTemplate), false },
                    { typeof(PermanentAlterationEffectTemplate), true },
                    { typeof(RemedyAlterationEffectTemplate), false },
                    { typeof(RevealAlterationEffectTemplate), false },
                    { typeof(StealAlterationEffectTemplate), true },
                    { typeof(TeleportManualAlterationEffectTemplate), false },
                    { typeof(TeleportRandomAlterationEffectTemplate), false },
                    { typeof(TemporaryAlterationEffectTemplate), true },
                    { typeof(TransmuteAlterationEffectTemplate), false },
                    { typeof(UncurseAlterationEffectTemplate), false }
                });
            // IEnemyAlterationEffectTemplate -> Block Support
            _alterationTemplateBlockingSupport.Add(
                typeof(IEnemyAlterationEffectTemplate), new SimpleDictionary<Type, bool>(){
                    { typeof(AttackAttributeMeleeAlterationEffectTemplate), true },
                    { typeof(AttackAttributeTemporaryAlterationEffectTemplate), true },
                    { typeof(CreateEnemyAlterationEffectTemplate), false },
                    { typeof(CreateFriendlyAlterationEffectTemplate), false },
                    { typeof(CreateTemporaryCharacterAlterationEffectTemplate), false },
                    { typeof(EquipmentDamageAlterationEffectTemplate), true },
                    { typeof(PermanentAlterationEffectTemplate), true },
                    { typeof(StealAlterationEffectTemplate), true },
                    { typeof(RunAwayAlterationEffectTemplate), false },
                    { typeof(TeleportRandomAlterationEffectTemplate), false },
                    { typeof(TemporaryAlterationEffectTemplate), true }
                });
            // IFriendlyAlterationEffect -> Block Support
            _alterationTemplateBlockingSupport.Add(
                typeof(IFriendlyAlterationEffectTemplate), new SimpleDictionary<Type, bool>(){
                    { typeof(AttackAttributeMeleeAlterationEffectTemplate), true },
                    { typeof(AttackAttributeTemporaryAlterationEffectTemplate), true },
                    { typeof(PermanentAlterationEffectTemplate), true },
                    { typeof(TemporaryAlterationEffectTemplate), true }
                });
            // ITemporaryCharacterAlterationEffect -> Block Support
            _alterationTemplateBlockingSupport.Add(
                typeof(ITemporaryCharacterAlterationEffectTemplate), new SimpleDictionary<Type, bool>(){
                    { typeof(AttackAttributeMeleeAlterationEffectTemplate), true },
                    { typeof(AttackAttributeTemporaryAlterationEffectTemplate), true },
                    { typeof(PermanentAlterationEffectTemplate), true },
                    { typeof(TemporaryAlterationEffectTemplate), true }
                });
            // IEquipmentAttackAlterationEffectTemplate -> Block Support
            _alterationTemplateBlockingSupport.Add(
                typeof(IEquipmentAttackAlterationEffectTemplate), new SimpleDictionary<Type, bool>(){
                    { typeof(AttackAttributeMeleeAlterationEffectTemplate), false },
                    { typeof(DrainMeleeAlterationEffectTemplate), false },
                    { typeof(PermanentAlterationEffectTemplate), false }
                });
            // IEquipmentEquipAlterationEffectTemplate -> Block Support (NONE FOR PASSIVES)
            _alterationTemplateBlockingSupport.Add(
                typeof(IEquipmentEquipAlterationEffectTemplate), new SimpleDictionary<Type, bool>(){
                    { typeof(AttackAttributeAuraAlterationEffectTemplate), false },
                    { typeof(AttackAttributePassiveAlterationEffectTemplate), false },
                    { typeof(BlockAlterationAlterationEffectTemplate), false },
                    { typeof(AuraAlterationEffectTemplate), false },
                    { typeof(PassiveAlterationEffectTemplate), false }
                });
            // IEquipmentCurseAlterationEffectTemplate -> Block Support (NONE FOR CURSES)
            _alterationTemplateBlockingSupport.Add(
                typeof(IEquipmentCurseAlterationEffectTemplate), new SimpleDictionary<Type, bool>(){
                    { typeof(AttackAttributeAuraAlterationEffectTemplate), false },
                    { typeof(AttackAttributePassiveAlterationEffectTemplate), false },
                    { typeof(AuraAlterationEffectTemplate), false },
                    { typeof(PassiveAlterationEffectTemplate), false }
                });
            // IDoodadAlterationEffectTemplate -> Block Support (NONE FOR DOODADS)
            _alterationTemplateBlockingSupport.Add(
                typeof(IDoodadAlterationEffectTemplate), new SimpleDictionary<Type, bool>(){
                    { typeof(AttackAttributeMeleeAlterationEffectTemplate), false },
                    { typeof(AttackAttributeTemporaryAlterationEffectTemplate), false },
                    { typeof(ChangeLevelAlterationEffectTemplate), false },
                    { typeof(CreateEnemyAlterationEffectTemplate), false },
                    { typeof(CreateFriendlyAlterationEffectTemplate), false },
                    { typeof(CreateTemporaryCharacterAlterationEffectTemplate), false },
                    { typeof(DetectAlterationAlignmentAlterationEffectTemplate), false },
                    { typeof(DetectAlterationAlterationEffectTemplate), false },
                    { typeof(EquipmentDamageAlterationEffectTemplate), false },
                    { typeof(EquipmentEnhanceAlterationEffectTemplate), false },
                    { typeof(IdentifyAlterationEffectTemplate), false },
                    { typeof(PermanentAlterationEffectTemplate), false },
                    { typeof(RemedyAlterationEffectTemplate), false },
                    { typeof(RevealAlterationEffectTemplate), false },
                    { typeof(TeleportRandomAlterationEffectTemplate), false },
                    { typeof(TemporaryAlterationEffectTemplate), false },
                    { typeof(TransmuteAlterationEffectTemplate), false },
                    { typeof(UncurseAlterationEffectTemplate), false }
                });
            // IConsumableAlterationEffectTemplate -> Block Support (NONE FOR DOODADS)
            _alterationTemplateBlockingSupport.Add(
                typeof(IConsumableAlterationEffectTemplate), new SimpleDictionary<Type, bool>(){
                    { typeof(AttackAttributeMeleeAlterationEffectTemplate), false },
                    { typeof(AttackAttributeTemporaryAlterationEffectTemplate), false },
                    { typeof(ChangeLevelAlterationEffectTemplate), false },
                    { typeof(CreateEnemyAlterationEffectTemplate), false },
                    { typeof(CreateFriendlyAlterationEffectTemplate), false },
                    { typeof(CreateTemporaryCharacterAlterationEffectTemplate), false },
                    { typeof(DetectAlterationAlignmentAlterationEffectTemplate), false },
                    { typeof(DetectAlterationAlterationEffectTemplate), false },
                    { typeof(EquipmentDamageAlterationEffectTemplate), false },
                    { typeof(EquipmentEnhanceAlterationEffectTemplate), false },
                    { typeof(IdentifyAlterationEffectTemplate), false },
                    { typeof(PermanentAlterationEffectTemplate), false },
                    { typeof(RemedyAlterationEffectTemplate), false },
                    { typeof(RevealAlterationEffectTemplate), false },
                    { typeof(TeleportRandomAlterationEffectTemplate), false },
                    { typeof(TemporaryAlterationEffectTemplate), false },
                    { typeof(TransmuteAlterationEffectTemplate), false },
                    { typeof(UncurseAlterationEffectTemplate), false }
                });
            // IConsumableProjectileAlterationEffectTemplate -> Block Support (NONE FOR PROJECTILES)
            _alterationTemplateBlockingSupport.Add(
                typeof(IConsumableProjectileAlterationEffectTemplate), new SimpleDictionary<Type, bool>(){
                    { typeof(AttackAttributeMeleeAlterationEffectTemplate), false },
                    { typeof(AttackAttributeTemporaryAlterationEffectTemplate), false },
                    { typeof(PermanentAlterationEffectTemplate), false },
                    { typeof(TemporaryAlterationEffectTemplate), false }
                });

            Validate();
        }

        // Ensures that all alteration effect types are accounted for; and that the dictionaries are ready
        private static void Validate()
        {
            var alterationEffectTypes =
                typeof(AlterationSpecificationContainer).Assembly
                                                        .GetTypes()
                                                        .Where(x => typeof(IAlterationEffect).IsAssignableFrom(x));

            var alterationTemplateEffectTypes =
                typeof(AlterationSpecificationContainer).Assembly
                                                        .GetTypes()
                                                        .Where(x => typeof(IAlterationEffectTemplate).IsAssignableFrom(x));

            // Iterate each IAlterationEffect
            foreach (var alterationEffectType in alterationEffectTypes)
            {
                // Check for interface inheritance
                foreach(var interfaceType in alterationEffectType.GetInterfaces())
                {
                    // Have to filter out this inherited member
                    if (interfaceType == typeof(IAlterationEffect))
                        continue;

                    // Have to filter out anything that DOESN'T inherit from IAlterationEffect
                    if (!typeof(IAlterationEffect).IsAssignableFrom(interfaceType))
                        continue;

                    if (!_alterationCostTypes.ContainsKey(interfaceType))
                        throw new Exception("Unhandled IAlterationEffect Interface Type (Cost Type):  " + interfaceType.Name + " for " + alterationEffectType.Name);

                    if (!_alterationBlockingSupport.ContainsKey(interfaceType))
                        throw new Exception("Unhandled IAlterationEffect Interface Type (Blocking Support):  " + interfaceType.Name + " for " + alterationEffectType.Name);

                    if (!_alterationCostTypes[interfaceType].ContainsKey(alterationEffectType))
                        throw new Exception("Unhandled Alteration Effect Implementation Type Type (Cost Type):  " + interfaceType.Name + " for " + alterationEffectType.Name);

                    if (!_alterationBlockingSupport[interfaceType].ContainsKey(alterationEffectType))
                        throw new Exception("Unhandled Alteration Effect Implementation Type Type (Cost Type):  " + interfaceType.Name + " for " + alterationEffectType.Name);
                }
            }

            // Iterate each IAlterationEffectTemplate
            foreach (var alterationTemplateEffectType in alterationTemplateEffectTypes)
            {
                // Check for interface inheritance
                foreach (var interfaceType in alterationTemplateEffectType.GetInterfaces())
                {
                    // Have to filter out this inherited member
                    if (interfaceType == typeof(IAlterationEffectTemplate))
                        continue;

                    // Have to filter out anything that DOESN'T inherit from IAlterationEffectTemplate
                    if (!typeof(IAlterationEffectTemplate).IsAssignableFrom(interfaceType))
                        continue;

                    if (!_alterationTemplateCostTypes.ContainsKey(interfaceType))
                        throw new Exception("Unhandled IAlterationEffectTemplate Interface Type (Cost Type):  " + interfaceType.Name + " for " + alterationTemplateEffectType.Name);

                    if (!_alterationTemplateBlockingSupport.ContainsKey(interfaceType))
                        throw new Exception("Unhandled IAlterationEffectTemplate Interface Type (Blocking Support):  " + interfaceType.Name + " for " + alterationTemplateEffectType.Name);

                    if (!_alterationTemplateCostTypes[interfaceType].ContainsKey(alterationTemplateEffectType))
                        throw new Exception("Unhandled Alteration Effect Template Implementation Type Type (Cost Type):  " + interfaceType.Name + " for " + alterationTemplateEffectType.Name);

                    if (!_alterationTemplateBlockingSupport[interfaceType].ContainsKey(alterationTemplateEffectType))
                        throw new Exception("Unhandled Alteration Effect Template Implementation Type Type (Cost Type):  " + interfaceType.Name + " for " + alterationTemplateEffectType.Name);
                }
            }
        }

        public static AlterationCostType GetCostType(AlterationContainer alteration, IAlterationEffect alterationEffect)
        {
            // Get the Implementation Type
            var alterationEffectType = alterationEffect.GetType();

            if (alteration is ConsumableAlteration)
                return _alterationCostTypes[typeof(IConsumableAlterationEffect)][alterationEffectType];

            else if (alteration is ConsumableProjectileAlteration)
                return _alterationCostTypes[typeof(IConsumableProjectileAlterationEffect)][alterationEffectType];

            else if (alteration is DoodadAlteration)
                return _alterationCostTypes[typeof(IDoodadAlterationEffect)][alterationEffectType];

            else if (alteration is EnemyAlteration)
                return _alterationCostTypes[typeof(IEnemyAlterationEffect)][alterationEffectType];

            else if (alteration is EquipmentAttackAlteration)
                return _alterationCostTypes[typeof(IEquipmentAttackAlterationEffect)][alterationEffectType];

            else if (alteration is EquipmentCurseAlteration)
                return _alterationCostTypes[typeof(IEquipmentCurseAlterationEffect)][alterationEffectType];

            else if (alteration is EquipmentEquipAlteration)
                return _alterationCostTypes[typeof(IEquipmentCurseAlterationEffect)][alterationEffectType];

            else if (alteration is SkillAlteration)
                return _alterationCostTypes[typeof(ISkillAlterationEffect)][alterationEffectType];

            else
                throw new Exception("Unhandled IAlterationEffect Type");
        }

        public static bool GetSupportsBlocking(AlterationContainer alteration, IAlterationEffect alterationEffect)
        {
            // Get the Implementation Type
            var alterationEffectType = alterationEffect.GetType();

            if (alteration is ConsumableAlteration)
                return _alterationBlockingSupport[typeof(IConsumableAlterationEffect)][alterationEffectType];

            else if (alteration is ConsumableProjectileAlteration)
                return _alterationBlockingSupport[typeof(IConsumableProjectileAlterationEffect)][alterationEffectType];

            else if (alteration is DoodadAlteration)
                return _alterationBlockingSupport[typeof(IDoodadAlterationEffect)][alterationEffectType];

            else if (alteration is EnemyAlteration)
                return _alterationBlockingSupport[typeof(IEnemyAlterationEffect)][alterationEffectType];

            else if (alteration is EquipmentAttackAlteration)
                return _alterationBlockingSupport[typeof(IEquipmentAttackAlterationEffect)][alterationEffectType];

            else if (alteration is EquipmentCurseAlteration)
                return _alterationBlockingSupport[typeof(IEquipmentCurseAlterationEffect)][alterationEffectType];

            else if (alteration is EquipmentEquipAlteration)
                return _alterationBlockingSupport[typeof(IEquipmentCurseAlterationEffect)][alterationEffectType];

            else if (alteration is SkillAlteration)
                return _alterationBlockingSupport[typeof(ISkillAlterationEffect)][alterationEffectType];

            else
                throw new Exception("Unhandled IAlterationEffect Type");
        }

        public static AlterationCostType GetCostType(AlterationTemplate alteration, IAlterationEffectTemplate alterationEffect)
        {
            // Get the Implementation Type
            var alterationEffectType = alterationEffect.GetType();

            if (alteration is ConsumableAlterationTemplate)
                return _alterationTemplateCostTypes[typeof(IConsumableAlterationEffectTemplate)][alterationEffectType];

            else if (alteration is ConsumableProjectileAlterationTemplate)
                return _alterationTemplateCostTypes[typeof(IConsumableProjectileAlterationEffectTemplate)][alterationEffectType];

            else if (alteration is DoodadAlterationTemplate)
                return _alterationTemplateCostTypes[typeof(IDoodadAlterationEffectTemplate)][alterationEffectType];

            else if (alteration is EnemyAlterationTemplate)
                return _alterationTemplateCostTypes[typeof(IEnemyAlterationEffectTemplate)][alterationEffectType];

            else if (alteration is EquipmentAttackAlterationTemplate)
                return _alterationTemplateCostTypes[typeof(IEquipmentAttackAlterationEffectTemplate)][alterationEffectType];

            else if (alteration is EquipmentCurseAlterationTemplate)
                return _alterationTemplateCostTypes[typeof(IEquipmentCurseAlterationEffectTemplate)][alterationEffectType];

            else if (alteration is EquipmentEquipAlterationTemplate)
                return _alterationTemplateCostTypes[typeof(IEquipmentCurseAlterationEffectTemplate)][alterationEffectType];

            else if (alteration is SkillAlterationTemplate)
                return _alterationTemplateCostTypes[typeof(ISkillAlterationEffectTemplate)][alterationEffectType];

            else
                throw new Exception("Unhandled IAlterationEffect Type");
        }

        public static bool GetSupportsBlocking(AlterationTemplate alteration, IAlterationEffectTemplate alterationEffect)
        {
            // Get the Implementation Type
            var alterationEffectType = alterationEffect.GetType();

            if (alteration is ConsumableAlterationTemplate)
                return _alterationTemplateBlockingSupport[typeof(IConsumableAlterationEffectTemplate)][alterationEffectType];

            else if (alteration is ConsumableProjectileAlterationTemplate)
                return _alterationTemplateBlockingSupport[typeof(IConsumableProjectileAlterationEffectTemplate)][alterationEffectType];

            else if (alteration is DoodadAlterationTemplate)
                return _alterationTemplateBlockingSupport[typeof(IDoodadAlterationEffectTemplate)][alterationEffectType];

            else if (alteration is EnemyAlterationTemplate)
                return _alterationTemplateBlockingSupport[typeof(IEnemyAlterationEffectTemplate)][alterationEffectType];

            else if (alteration is EquipmentAttackAlterationTemplate)
                return _alterationTemplateBlockingSupport[typeof(IEquipmentAttackAlterationEffectTemplate)][alterationEffectType];

            else if (alteration is EquipmentCurseAlterationTemplate)
                return _alterationTemplateBlockingSupport[typeof(IEquipmentCurseAlterationEffectTemplate)][alterationEffectType];

            else if (alteration is EquipmentEquipAlterationTemplate)
                return _alterationTemplateBlockingSupport[typeof(IEquipmentCurseAlterationEffectTemplate)][alterationEffectType];

            else if (alteration is SkillAlterationTemplate)
                return _alterationTemplateBlockingSupport[typeof(ISkillAlterationEffectTemplate)][alterationEffectType];

            else
                throw new Exception("Unhandled IAlterationEffect Type");
        }
    }
}
