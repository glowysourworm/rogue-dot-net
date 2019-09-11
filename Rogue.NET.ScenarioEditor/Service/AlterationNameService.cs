using Rogue.NET.Core.Model.Enums;
using Rogue.NET.ScenarioEditor.Service.Interface;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Common;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Interface;
using System;
using System.ComponentModel.Composition;

namespace Rogue.NET.ScenarioEditor.Service
{
    [Export(typeof(IAlterationNameService))]
    public class AlterationNameService : IAlterationNameService
    {
        public void Execute(ScenarioConfigurationContainerViewModel configuration)
        {
            // Consumables
            foreach (var template in configuration.ConsumableTemplates)
            {
                if (template.HasAlteration)
                    SetName(template.ConsumableAlteration.Effect, template.ConsumableAlteration.Name);

                if (template.HasProjectileAlteration)
                    SetName(template.ConsumableProjectileAlteration.Effect, template.ConsumableProjectileAlteration.Name);
            }

            // Doodads
            foreach (var template in configuration.DoodadTemplates)
            {
                if (template.IsAutomatic)
                    SetName(template.AutomaticAlteration.Effect, template.AutomaticAlteration.Name);

                if (template.IsInvoked)
                    SetName(template.InvokedAlteration.Effect, template.InvokedAlteration.Name);
            }

            // Enemies
            foreach (var template in configuration.EnemyTemplates)
            {
                foreach (var behavior in template.BehaviorDetails.Behaviors)
                {
                    if (behavior.AttackType == CharacterAttackType.Alteration)
                        SetName(behavior.Alteration.Effect, behavior.Alteration.Name);
                }
            }

            // Equipment
            foreach (var template in configuration.EquipmentTemplates)
            {
                if (template.HasAttackAlteration)
                    SetName(template.EquipmentAttackAlteration.Effect, template.EquipmentAttackAlteration.Name);

                if (template.HasCurseAlteration)
                    SetName(template.EquipmentCurseAlteration.Effect, template.EquipmentCurseAlteration.Name);

                if (template.HasEquipAlteration)
                    SetName(template.EquipmentEquipAlteration.Effect, template.EquipmentEquipAlteration.Name);
            }

            // Skills
            foreach (var template in configuration.SkillTemplates)
            {
                foreach (var skill in template.Skills)
                    SetName(skill.SkillAlteration.Effect, skill.SkillAlteration.Name);
            }
        }

        private void SetName(IAlterationEffectTemplateViewModel effect, string name)
        {
            if (effect == null)
                return;

            if (effect is AttackAttributeAuraAlterationEffectTemplateViewModel)
                (effect as AttackAttributeAuraAlterationEffectTemplateViewModel).Name = name;

            else if (effect is AttackAttributeMeleeAlterationEffectTemplateViewModel)
                (effect as AttackAttributeMeleeAlterationEffectTemplateViewModel).Name = name;

            else if (effect is AttackAttributePassiveAlterationEffectTemplateViewModel)
                (effect as AttackAttributePassiveAlterationEffectTemplateViewModel).Name = name;

            else if (effect is AttackAttributeTemporaryAlterationEffectTemplateViewModel)
                (effect as AttackAttributeTemporaryAlterationEffectTemplateViewModel).Name = name;

            else if (effect is AuraAlterationEffectTemplateViewModel)
                (effect as AuraAlterationEffectTemplateViewModel).Name = name;

            else if (effect is ChangeLevelAlterationEffectTemplateViewModel)
                (effect as ChangeLevelAlterationEffectTemplateViewModel).Name = name;

            else if (effect is CreateMonsterAlterationEffectTemplateViewModel)
                (effect as CreateMonsterAlterationEffectTemplateViewModel).Name = name;

            else if (effect is DrainMeleeAlterationEffectTemplateViewModel)
                (effect as DrainMeleeAlterationEffectTemplateViewModel).Name = name;

            else if (effect is EquipmentDamageAlterationEffectTemplateViewModel)
                (effect as EquipmentDamageAlterationEffectTemplateViewModel).Name = name;

            else if (effect is EquipmentEnhanceAlterationEffectTemplateViewModel)
                (effect as EquipmentEnhanceAlterationEffectTemplateViewModel).Name = name;

            else if (effect is OtherAlterationEffectTemplateViewModel)
                (effect as OtherAlterationEffectTemplateViewModel).Name = name;

            else if (effect is PassiveAlterationEffectTemplateViewModel)
                (effect as PassiveAlterationEffectTemplateViewModel).Name = name;

            else if (effect is PermanentAlterationEffectTemplateViewModel)
                (effect as PermanentAlterationEffectTemplateViewModel).Name = name;

            else if (effect is RemedyAlterationEffectTemplateViewModel)
                (effect as RemedyAlterationEffectTemplateViewModel).Name = name;

            else if (effect is RevealAlterationEffectTemplateViewModel)
                (effect as RevealAlterationEffectTemplateViewModel).Name = name;

            else if (effect is RunAwayAlterationEffectTemplateViewModel)
                (effect as RunAwayAlterationEffectTemplateViewModel).Name = name;

            else if (effect is StealAlterationEffectTemplateViewModel)
                (effect as StealAlterationEffectTemplateViewModel).Name = name;

            else if (effect is TeleportRandomAlterationEffectTemplateViewModel)
                (effect as TeleportRandomAlterationEffectTemplateViewModel).Name = name;

            else if (effect is TemporaryAlterationEffectTemplateViewModel)
                (effect as TemporaryAlterationEffectTemplateViewModel).Name = name;

            else if (effect is TransmuteAlterationEffectTemplateViewModel)
                (effect as TransmuteAlterationEffectTemplateViewModel).Name = name;

            else
                throw new Exception("Unhandled Alteration Effect Type");
        }
    }
}
