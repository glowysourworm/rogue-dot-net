using Rogue.NET.Core.Model;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration.Effect;
using Rogue.NET.Core.Model.ScenarioConfiguration;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Common;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Consumable;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Doodad;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Enemy;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Equipment;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Interface;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Skill;
using Rogue.NET.Core.Model.ScenarioConfiguration.Animation;
using Rogue.NET.Core.Model.ScenarioConfiguration.Content;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Rogue.NET.ScenarioEditor.Utility
{
    /// <summary>
    /// LAST RESORT... I tried AutoMapper, ExpressMapper, TinyMapper, and AgileMapper. Each had an issue
    /// preventing use for mapping the ScenarioConfigurationContainer <--> ScenarioConfigurationContainerViewModel.
    /// 
    /// Mostly, performance was a BIG factor - 4-5 minutes to compile the mappings (first run). The AgileMapper was
    /// by far the best fluent API (next to AutoMapper) - that still performed well; but it didn't support mapping
    /// generics and collections easily. So, configuration would have been less-feasible. I figured it would take
    /// just about as long to write it by hand...
    /// 
    /// So, here we are........ (I'm using AgileMapper here to help out!)
    /// </summary>
    public class ScenarioConfigurationMapper
    {
        // Constructed type maps for configuration name spaces
        private static readonly IDictionary<Type, Type> _forwardTypeMap;
        private static readonly IDictionary<Type, Type> _reverseTypeMap;

        static ScenarioConfigurationMapper()
        {
            _forwardTypeMap = new Dictionary<Type, Type>();
            _reverseTypeMap = new Dictionary<Type, Type>();

            var sourceTypes = typeof(ScenarioConfigurationContainer).Assembly.GetTypes();
            var destTypes = typeof(ScenarioConfigurationContainerViewModel).Assembly.GetTypes();

            // Foward Map
            foreach (var type in sourceTypes)
            {
                var destType = destTypes.FirstOrDefault(x => x.Name == type.Name + "ViewModel");

                if (destType != null)
                    _forwardTypeMap.Add(type, destType);
            }

            // Reverse Map
            foreach (var type in destTypes)
            {
                var sourceType = sourceTypes.FirstOrDefault(x => x.Name + "ViewModel" == type.Name);

                if (sourceType != null)
                    _reverseTypeMap.Add(type, sourceType);
            }
        }

        public ScenarioConfigurationContainerViewModel Map(ScenarioConfigurationContainer model)
        {
            var result = MapObject<ScenarioConfigurationContainer, ScenarioConfigurationContainerViewModel>(model, false);

            return FixReferences(result);
        }
        public ScenarioConfigurationContainer MapBack(ScenarioConfigurationContainerViewModel viewModel)
        {
            var result = MapObject<ScenarioConfigurationContainerViewModel, ScenarioConfigurationContainer>(viewModel, true);

            var configuration = FixReferences(result);

            // Sort collections 
            configuration.AnimationTemplates.Sort((x, y) => x.Name.CompareTo(y.Name));
            configuration.BrushTemplates.Sort((x, y) => x.Name.CompareTo(y.Name));
            configuration.ConsumableTemplates.Sort((x, y) => x.Name.CompareTo(y.Name));
            configuration.DoodadTemplates.Sort((x, y) => x.Name.CompareTo(y.Name));
            configuration.EnemyTemplates.Sort((x, y) => x.Name.CompareTo(y.Name));
            configuration.EquipmentTemplates.Sort((x, y) => x.Name.CompareTo(y.Name));
            configuration.MagicSpells.Sort((x, y) => x.Name.CompareTo(y.Name));
            configuration.CharacterClasses.Sort((x, y) => x.Name.CompareTo(y.Name));
            configuration.SkillTemplates.Sort((x, y) => x.Name.CompareTo(y.Name));
            /*
            // Map AnimationType enum to refactor
            foreach (var template in configuration.AnimationTemplates)
                MapAnimationType(template);

            foreach (var template in configuration.ConsumableTemplates)
            {
                foreach (var animation in template.AmmoSpellTemplate.Animations)
                    MapAnimationType(animation);

                foreach (var animation in template.ProjectileSpellTemplate.Animations)
                    MapAnimationType(animation);

                foreach (var animation in template.SpellTemplate.Animations)
                    MapAnimationType(animation);
            }

            foreach (var template in configuration.DoodadTemplates)
            {
                foreach (var animation in template.AutomaticMagicSpellTemplate.Animations)
                    MapAnimationType(animation);

                foreach (var animation in template.InvokedMagicSpellTemplate.Animations)
                    MapAnimationType(animation);
            }

            foreach (var template in configuration.EnemyTemplates)
            {
                foreach (var behavior in template.BehaviorDetails.Behaviors)
                    foreach (var animation in behavior.EnemySpell.Animations)
                        MapAnimationType(animation);

                foreach (var consumable in template.StartingConsumables)
                {
                    foreach (var animation in consumable.TheTemplate.AmmoSpellTemplate.Animations)
                        MapAnimationType(animation);

                    foreach (var animation in consumable.TheTemplate.ProjectileSpellTemplate.Animations)
                        MapAnimationType(animation);

                    foreach (var animation in consumable.TheTemplate.SpellTemplate.Animations)
                        MapAnimationType(animation);
                }

                foreach (var equipment in template.StartingEquipment)
                {
                    foreach (var animation in equipment.TheTemplate.CurseSpell.Animations)
                        MapAnimationType(animation);

                    foreach (var animation in equipment.TheTemplate.EquipSpell.Animations)
                        MapAnimationType(animation);
                }
            }

            foreach (var template in configuration.EquipmentTemplates)
            {
                foreach (var animation in template.CurseSpell.Animations)
                    MapAnimationType(animation);

                foreach (var animation in template.EquipSpell.Animations)
                    MapAnimationType(animation);
            }

            foreach (var template in configuration.MagicSpells)
                foreach (var animation in template.Animations)
                    MapAnimationType(animation);

            foreach (var template in configuration.SkillTemplates)
                foreach (var skill in template.Skills)
                    foreach (var animation in skill.Alteration.Animations)
                        MapAnimationType(animation);

            foreach (var skillSet in configuration.PlayerTemplate.Skills)
                foreach (var skill in skillSet.Skills)
                    foreach (var animation in skill.Alteration.Animations)
                        MapAnimationType(animation);
                        */
            // Map Alteration Data to new data structures
            // 0) Instantiate new data structures
            // 1) MagicSpells -> DELETE (Not Needed)
            // 2) Consumables -> Alterations, Animations, Symbol Delta
            // 3) Doodads -> same
            // 4) Equipment -> same
            // 5) Enemy -> same
            // 6) Skills -> same
            // 7) Also do Player, Enemy Skills and items (because of copied data)

            /* TODO: REMOVE THIS 
            configuration.AlterationContainer = new ScenarioConfigurationAlterationContainer();

            var consumableCopy = new Action<ConsumableTemplate>(template =>
            {
                template.ConsumableAlteration = new ConsumableAlterationTemplate();
                template.ConsumableProjectileAlteration = new ConsumableProjectileAlterationTemplate();
                template.AmmoAnimationGroup = new AnimationGroupTemplate();
                template.HasAlteration = template.HasSpell;
                template.HasProjectileAlteration = template.IsProjectile;

                if (template.HasSpell)
                {
                    template.ConsumableAlteration.AnimationGroup = CreateAnimation(template.SpellTemplate.Animations);
                    template.ConsumableAlteration.Cost = template.SpellTemplate.Cost;
                    template.ConsumableAlteration.Effect = CreateAlterationEffect(template.SpellTemplate) as IConsumableAlterationEffectTemplate;
                    template.ConsumableAlteration.Guid = Guid.NewGuid().ToString();
                    template.ConsumableAlteration.Name = template.SpellTemplate.DisplayName;

                    if (template.ConsumableAlteration.Effect == null || template.ConsumableAlteration.AnimationGroup.Animations == null)
                        throw new Exception("Unhandled template");

                    configuration.AlterationContainer.ConsumableAlterations.Add(template.ConsumableAlteration);
                }

                if (template.IsProjectile)
                {
                    template.ConsumableProjectileAlteration.AnimationGroup = CreateAnimation(template.ProjectileSpellTemplate.Animations);
                    template.ConsumableProjectileAlteration.Effect = CreateAlterationEffect(template.ProjectileSpellTemplate) as IConsumableProjectileAlterationEffectTemplate;
                    template.ConsumableProjectileAlteration.Guid = Guid.NewGuid().ToString();
                    template.ConsumableProjectileAlteration.Name = template.ProjectileSpellTemplate.DisplayName;

                    if (template.ConsumableProjectileAlteration.Effect == null || template.ConsumableProjectileAlteration.AnimationGroup.Animations == null)
                        throw new Exception("Unhandled template");

                    configuration.AlterationContainer.ConsumableProjectileAlterations.Add(template.ConsumableProjectileAlteration);
                }
            });
            var equipmentCopy = new Action<EquipmentTemplate>(template =>
            {
                template.EquipmentAttackAlteration = new EquipmentAttackAlterationTemplate();
                template.EquipmentCurseAlteration = new EquipmentCurseAlterationTemplate();
                template.EquipmentEquipAlteration = new EquipmentEquipAlterationTemplate();
                template.HasAttackAlteration = false;
                template.HasCurseAlteration = template.HasCurseSpell;
                template.HasEquipAlteration = template.HasEquipSpell;

                if (template.HasCurseAlteration)
                {
                    template.EquipmentCurseAlteration.Effect = CreateAlterationEffect(template.CurseSpell) as IEquipmentCurseAlterationEffectTemplate;
                    template.EquipmentCurseAlteration.Guid = Guid.NewGuid().ToString();
                    template.EquipmentCurseAlteration.Name = template.CurseSpell.DisplayName;

                    if (template.EquipmentCurseAlteration.Effect == null)
                        throw new Exception("Unhandled template");

                    configuration.AlterationContainer.EquipmentCurseAlterations.Add(template.EquipmentCurseAlteration);
                }

                if (template.HasEquipAlteration)
                {
                    template.EquipmentEquipAlteration.Effect = CreateAlterationEffect(template.EquipSpell) as IEquipmentEquipAlterationEffectTemplate;
                    template.EquipmentEquipAlteration.Guid = Guid.NewGuid().ToString();
                    template.EquipmentEquipAlteration.Name = template.EquipSpell.DisplayName;

                    if (template.EquipmentEquipAlteration.Effect == null)
                        throw new Exception("Unhandled template");

                    configuration.AlterationContainer.EquipmentEquipAlterations.Add(template.EquipmentEquipAlteration);
                }
            });
            var skillCopy = new Action<SkillTemplate>(template =>
            {
                template.SkillAlteration = new SkillAlterationTemplate();
                template.SkillAlteration.AnimationGroup = CreateAnimation(template.Alteration.Animations);
                template.SkillAlteration.BlockType = template.Alteration.BlockType;
                template.SkillAlteration.Cost = template.Alteration.Cost;
                template.SkillAlteration.Effect = CreateAlterationEffect(template.Alteration) as ISkillAlterationEffectTemplate;
                template.SkillAlteration.Guid = Guid.NewGuid().ToString();
                template.SkillAlteration.Name = template.Alteration.DisplayName;

                if (template.SkillAlteration.Effect == null)
                    throw new Exception("Unhandled effect");

                configuration.AlterationContainer.SkillAlterations.Add(template.SkillAlteration);
            });


            foreach (var template in configuration.ConsumableTemplates)
                consumableCopy(template);

            foreach (var template in configuration.DoodadTemplates)
            {
                template.AutomaticAlteration = new DoodadAlterationTemplate();
                template.InvokedAlteration = new DoodadAlterationTemplate();

                if (template.IsAutomatic)
                {
                    template.AutomaticAlteration.AnimationGroup = CreateAnimation(template.AutomaticMagicSpellTemplate.Animations);
                    template.AutomaticAlteration.Effect = CreateAlterationEffect(template.AutomaticMagicSpellTemplate) as IDoodadAlterationEffectTemplate;
                    template.AutomaticAlteration.Guid = Guid.NewGuid().ToString();
                    template.AutomaticAlteration.Name = template.AutomaticMagicSpellTemplate.DisplayName;

                    if (template.AutomaticAlteration.Effect == null || template.AutomaticAlteration.AnimationGroup.Animations == null)
                        throw new Exception("Unhandled template");

                    configuration.AlterationContainer.DoodadAlterations.Add(template.AutomaticAlteration);
                }
                if (template.IsInvoked)
                {
                    template.InvokedAlteration.AnimationGroup = CreateAnimation(template.InvokedMagicSpellTemplate.Animations);
                    template.InvokedAlteration.Effect = CreateAlterationEffect(template.InvokedMagicSpellTemplate) as IDoodadAlterationEffectTemplate;
                    template.InvokedAlteration.Guid = Guid.NewGuid().ToString();
                    template.InvokedAlteration.Name = template.InvokedMagicSpellTemplate.DisplayName;

                    if (template.InvokedAlteration.Effect == null || template.InvokedAlteration.AnimationGroup.Animations == null)
                        throw new Exception("Unhandled template");

                    configuration.AlterationContainer.DoodadAlterations.Add(template.InvokedAlteration);
                }
            }

            foreach (var template in configuration.EquipmentTemplates)
                equipmentCopy(template);

            foreach (var template in configuration.EnemyTemplates)
            {
                foreach (var behaviorTemplate in template.BehaviorDetails.Behaviors)
                {
                    if (behaviorTemplate.AttackType == CharacterAttackType.Skill ||
                        behaviorTemplate.AttackType == CharacterAttackType.SkillCloseRange)
                    {
                        behaviorTemplate.EnemyAlteration = new EnemyAlterationTemplate();
                        behaviorTemplate.EnemyAlteration.AnimationGroup = CreateAnimation(behaviorTemplate.EnemySpell.Animations);
                        behaviorTemplate.EnemyAlteration.Cost = behaviorTemplate.EnemySpell.Cost;
                        behaviorTemplate.EnemyAlteration.Effect = CreateAlterationEffect(behaviorTemplate.EnemySpell) as IEnemyAlterationEffectTemplate;
                        behaviorTemplate.EnemyAlteration.Guid = Guid.NewGuid().ToString();
                        behaviorTemplate.EnemyAlteration.Name = behaviorTemplate.EnemySpell.DisplayName;

                        if (behaviorTemplate.EnemyAlteration.Effect == null)
                            throw new Exception("Unhandled effect");

                        configuration.AlterationContainer.EnemyAlterations.Add(behaviorTemplate.EnemyAlteration);
                    }
                }

                foreach (var consumableTemplate in template.StartingConsumables)
                    consumableCopy(consumableTemplate.TheTemplate);

                foreach (var equipmentTemplate in template.StartingEquipment)
                    equipmentCopy(equipmentTemplate.TheTemplate);
            }

            foreach (var skillSetTemplate in configuration.SkillTemplates)
            {
                foreach (var template in skillSetTemplate.Skills)
                    skillCopy(template);
            }

            foreach (var skillSetTemplate in configuration.PlayerTemplate.Skills)
                foreach (var template in skillSetTemplate.Skills)
                    skillCopy(template);

            foreach (var startingConsumable in configuration.PlayerTemplate.StartingConsumables)
                consumableCopy(startingConsumable.TheTemplate);

            foreach (var startingEquipment in configuration.PlayerTemplate.StartingEquipment)
                equipmentCopy(startingEquipment.TheTemplate);

            */
            return configuration;
        }

        private object CreateAlterationEffect(SpellTemplate spell)
        {
            switch (spell.Type)
            {
                case AlterationType.PassiveSource:
                    return new PassiveAlterationEffectTemplate()
                    {
                        AgilityRange = spell.Effect.AgilityRange,
                        AttackRange = spell.Effect.AttackRange,
                        CanSeeInvisibleCharacters = spell.Effect.CanSeeInvisibleCharacters,
                        CriticalHit = spell.Effect.CriticalHit,
                        DefenseRange = spell.Effect.DefenseRange,
                        DodgeProbabilityRange = spell.Effect.DodgeProbabilityRange,
                        ExperienceRange = spell.Effect.ExperienceRange,
                        FoodUsagePerTurnRange = spell.Effect.FoodUsagePerTurnRange,
                        Guid = Guid.NewGuid().ToString(),
                        HpPerStepRange = spell.Effect.HpPerStepRange,
                        HpRange = spell.Effect.HpRange,
                        HungerRange = spell.Effect.HungerRange,
                        IntelligenceRange = spell.Effect.IntelligenceRange,
                        LightRadiusRange = spell.Effect.AuraRadiusRange,
                        MagicBlockProbabilityRange = spell.Effect.MagicBlockProbabilityRange,
                        MpPerStepRange = spell.Effect.MpPerStepRange,
                        MpRange = spell.Effect.MpRange,
                        Name = spell.DisplayName,
                        SpeedRange = spell.Effect.SpeedRange,
                        StrengthRange = spell.Effect.StrengthRange,
                        SymbolAlteration = MapSymbol(spell.Effect.SymbolAlteration)
                    };
                case AlterationType.PassiveAura:
                    return new AuraAlterationEffectTemplate()
                    {
                        AgilityRange = spell.AuraEffect.AgilityRange,
                        DefenseRange = spell.AuraEffect.DefenseRange,
                        AttackRange = spell.AuraEffect.AttackRange,
                        DodgeProbabilityRange = spell.AuraEffect.DodgeProbabilityRange,
                        HpPerStepRange = spell.AuraEffect.HpPerStepRange,
                        IntelligenceRange = spell.AuraEffect.IntelligenceRange,
                        MagicBlockProbabilityRange = spell.AuraEffect.MagicBlockProbabilityRange,
                        MpPerStepRange = spell.AuraEffect.MpPerStepRange,
                        Guid = Guid.NewGuid().ToString(),
                        Name = spell.DisplayName,
                        StrengthRange = spell.AuraEffect.StrengthRange,
                        SpeedRange = spell.AuraEffect.SpeedRange,
                        SymbolAlteration = MapSymbol(spell.Effect.SymbolAlteration)
                    };
                case AlterationType.TemporarySource:
                case AlterationType.TemporaryTarget:
                case AlterationType.TemporaryAllTargets:
                case AlterationType.TemporaryAllInRange:
                case AlterationType.TemporaryAllInRangeExceptSource:
                    return new TemporaryAlterationEffectTemplate()
                    {
                        AgilityRange = spell.Effect.AgilityRange,
                        AlteredState = spell.Effect.AlteredState,
                        AttackRange = spell.Effect.AttackRange,
                        CanSeeInvisibleCharacters = spell.Effect.CanSeeInvisibleCharacters,
                        CriticalHit = spell.Effect.CriticalHit,
                        DefenseRange = spell.Effect.DefenseRange,
                        DodgeProbabilityRange = spell.Effect.DodgeProbabilityRange,
                        EventTime = spell.Effect.EventTime,
                        ExperienceRange = spell.Effect.ExperienceRange,
                        FoodUsagePerTurnRange = spell.Effect.FoodUsagePerTurnRange,
                        Guid = Guid.NewGuid().ToString(),
                        HpPerStepRange = spell.Effect.HpPerStepRange,
                        HpRange = spell.Effect.HpRange,
                        HungerRange = spell.Effect.HungerRange,
                        IntelligenceRange = spell.Effect.IntelligenceRange,
                        LightRadiusRange = spell.Effect.AuraRadiusRange,
                        MagicBlockProbabilityRange = spell.Effect.MagicBlockProbabilityRange,
                        MpPerStepRange = spell.Effect.MpPerStepRange,
                        MpRange = spell.Effect.MpRange,
                        Name = spell.DisplayName,
                        SpeedRange = spell.Effect.SpeedRange,
                        StrengthRange = spell.Effect.StrengthRange,
                        SymbolAlteration = MapSymbol(spell.Effect.SymbolAlteration)
                    };
                case AlterationType.PermanentSource:
                case AlterationType.PermanentTarget:
                case AlterationType.PermanentAllTargets:
                case AlterationType.PermanentAllInRange:
                case AlterationType.PermanentAllInRangeExceptSource:
                    return new PermanentAlterationEffectTemplate()
                    {
                        AgilityRange = spell.Effect.AgilityRange,
                        ExperienceRange = spell.Effect.ExperienceRange,
                        Guid = Guid.NewGuid().ToString(),
                        HpRange = spell.Effect.HpRange,
                        HungerRange = spell.Effect.HungerRange,
                        IntelligenceRange = spell.Effect.IntelligenceRange,
                        LightRadiusRange = spell.Effect.AuraRadiusRange,
                        MpRange = spell.Effect.MpRange,
                        Name = spell.DisplayName,
                        SpeedRange = spell.Effect.SpeedRange,
                        StrengthRange = spell.Effect.StrengthRange                        
                    };
                case AlterationType.TeleportSelf:
                case AlterationType.TeleportTarget:
                case AlterationType.TeleportAllTargets:
                case AlterationType.TeleportAllInRange:
                case AlterationType.TeleportAllInRangeExceptSource:
                    return new TeleportAlterationEffectTemplate()
                    {
                        Guid = Guid.NewGuid().ToString(),
                        Name = spell.DisplayName,
                        TeleportType = AlterationRandomPlacementType.InLevel
                    };
                    // TODO:ALTERATION
                    /*
                case AlterationType.Steal:
                    return new StealAlterationEffectTemplate()
                    {
                        Guid = Guid.NewGuid().ToString(),
                        Name = spell.DisplayName,
                        Type = AlterationOtherEffectType.Steal
                    };
                case AlterationType.RunAway:
                    return new RunAwayAlterationEffectTemplate()
                    {
                        Guid = Guid.NewGuid().ToString(),
                        Name = spell.DisplayName,
                        Type = AlterationOtherEffectType.RunAway
                    };
                    */
                case AlterationType.OtherMagicEffect:
                    {
                        switch (spell.OtherEffectType)
                        {
                            case AlterationMagicEffectType.ChangeLevelRandomUp:
                                return new ChangeLevelAlterationEffectTemplate()
                                {
                                    Guid = Guid.NewGuid().ToString(),
                                    LevelChange = new Range<int>(-50, 5, 5, 50),
                                    Name = spell.DisplayName
                                };
                            case AlterationMagicEffectType.ChangeLevelRandomDown:
                                return new ChangeLevelAlterationEffectTemplate()
                                {
                                    Guid = Guid.NewGuid().ToString(),
                                    LevelChange = new Range<int>(-50, 5, 5, 50),
                                    Name = spell.DisplayName
                                };
                            case AlterationMagicEffectType.Identify:
                                return new OtherAlterationEffectTemplate()
                                {
                                    Guid = Guid.NewGuid().ToString(),
                                    Name = spell.DisplayName,
                                    Type = AlterationOtherEffectType.Identify
                                };
                            case AlterationMagicEffectType.Uncurse:
                                return new OtherAlterationEffectTemplate()
                                {
                                    Guid = Guid.NewGuid().ToString(),
                                    Name = spell.DisplayName,
                                    Type = AlterationOtherEffectType.Uncurse
                                };
                            case AlterationMagicEffectType.EnchantArmor:
                                return new EquipmentModifyAlterationEffectTemplate()
                                {
                                    ClassChange = 1,
                                    Guid = Guid.NewGuid().ToString(),
                                    Name = spell.DisplayName,
                                    QualityChange = 0,
                                    Type = AlterationModifyEquipmentType.WeaponClass
                                };
                            case AlterationMagicEffectType.EnchantWeapon:
                                return new EquipmentModifyAlterationEffectTemplate()
                                {
                                    ClassChange = 1,
                                    Guid = Guid.NewGuid().ToString(),
                                    Name = spell.DisplayName,
                                    QualityChange = 0,
                                    Type = AlterationModifyEquipmentType.WeaponClass
                                };
                            case AlterationMagicEffectType.RevealItems:
                                return new RevealAlterationEffectTemplate()
                                {
                                    Guid = Guid.NewGuid().ToString(),
                                    Name = spell.DisplayName,
                                    Type = AlterationRevealType.Items
                                };
                            case AlterationMagicEffectType.RevealMonsters:
                                return new RevealAlterationEffectTemplate()
                                {
                                    Guid = Guid.NewGuid().ToString(),
                                    Name = spell.DisplayName,
                                    Type = AlterationRevealType.Monsters
                                };
                            case AlterationMagicEffectType.RevealSavePoint:
                                return new RevealAlterationEffectTemplate()
                                {
                                    Guid = Guid.NewGuid().ToString(),
                                    Name = spell.DisplayName,
                                    Type = AlterationRevealType.SavePoint
                                };
                            case AlterationMagicEffectType.RevealFood:
                                return new RevealAlterationEffectTemplate()
                                {
                                    Guid = Guid.NewGuid().ToString(),
                                    Name = spell.DisplayName,
                                    Type = AlterationRevealType.Food
                                };
                            case AlterationMagicEffectType.RevealLevel:
                                return new RevealAlterationEffectTemplate()
                                {
                                    Guid = Guid.NewGuid().ToString(),
                                    Name = spell.DisplayName,
                                    Type = AlterationRevealType.Layout | 
                                           AlterationRevealType.Food | 
                                           AlterationRevealType.Items | 
                                           AlterationRevealType.Monsters |
                                           AlterationRevealType.SavePoint                                    
                                };
                            case AlterationMagicEffectType.CreateMonster:
                                return new CreateMonsterAlterationEffectTemplate()
                                {
                                    CreateMonsterEnemy = spell.CreateMonsterEnemy,
                                    Guid = Guid.NewGuid().ToString(),
                                    Name = spell.DisplayName,
                                    RandomPlacementType = AlterationRandomPlacementType.InRangeOfCharacter
                                };
                            default:
                                throw new Exception("Unhandled other type");
                        }
                    }
                case AlterationType.AttackAttribute:
                    {
                        switch (spell.AttackAttributeType)
                        {
                            case AlterationAttackAttributeType.ImbueArmor:
                                return new EquipmentModifyAlterationEffectTemplate()
                                {
                                    AttackAttributes = spell.Effect.AttackAttributes,
                                    ClassChange = 0,
                                    Guid = Guid.NewGuid().ToString(),
                                    Name = spell.DisplayName,
                                    QualityChange = 0,
                                    Type = AlterationModifyEquipmentType.ArmorImbue
                                };
                            case AlterationAttackAttributeType.ImbueWeapon:
                                return new EquipmentModifyAlterationEffectTemplate()
                                {
                                    AttackAttributes = spell.Effect.AttackAttributes,
                                    ClassChange = 0,
                                    Guid = Guid.NewGuid().ToString(),
                                    Name = spell.DisplayName,
                                    QualityChange = 0,
                                    Type = AlterationModifyEquipmentType.WeaponImbue
                                };
                            case AlterationAttackAttributeType.Passive:
                                return new AttackAttributePassiveAlterationEffectTemplate()
                                {
                                    AttackAttributes = spell.Effect.AttackAttributes,
                                    CombatType = AlterationAttackAttributeCombatType.Friendly,
                                    Guid = Guid.NewGuid().ToString(),
                                    Name = spell.DisplayName,
                                    TargetType = AlterationTargetType.Source                                    
                                };
                            case AlterationAttackAttributeType.TemporaryFriendlySource:
                                return new AttackAttributeTemporaryAlterationEffectTemplate()
                                {
                                    AttackAttributes = spell.Effect.AttackAttributes,
                                    CombatType = AlterationAttackAttributeCombatType.Friendly,
                                    Guid = Guid.NewGuid().ToString(),
                                    Name = spell.DisplayName,
                                    TargetType = AlterationTargetType.Source
                                };
                            case AlterationAttackAttributeType.TemporaryFriendlyTarget:
                                return new AttackAttributeTemporaryAlterationEffectTemplate()
                                {
                                    AttackAttributes = spell.Effect.AttackAttributes,
                                    CombatType = AlterationAttackAttributeCombatType.Friendly,
                                    Guid = Guid.NewGuid().ToString(),
                                    Name = spell.DisplayName,
                                    TargetType = AlterationTargetType.Target
                                };
                            case AlterationAttackAttributeType.TemporaryMalignSource:
                                return new AttackAttributeTemporaryAlterationEffectTemplate()
                                {
                                    AttackAttributes = spell.Effect.AttackAttributes,
                                    CombatType = AlterationAttackAttributeCombatType.Malign,
                                    Guid = Guid.NewGuid().ToString(),
                                    Name = spell.DisplayName,
                                    TargetType = AlterationTargetType.Source
                                };
                            case AlterationAttackAttributeType.TemporaryMalignTarget:
                                return new AttackAttributeTemporaryAlterationEffectTemplate()
                                {
                                    AttackAttributes = spell.Effect.AttackAttributes,
                                    CombatType = AlterationAttackAttributeCombatType.Malign,
                                    Guid = Guid.NewGuid().ToString(),
                                    Name = spell.DisplayName,
                                    TargetType = AlterationTargetType.Target
                                };
                            case AlterationAttackAttributeType.MeleeTarget:
                                return new AttackAttributeMeleeAlterationEffectTemplate()
                                {
                                    AttackAttributes = spell.Effect.AttackAttributes,
                                    CombatType = AlterationAttackAttributeCombatType.Malign,
                                    Guid = Guid.NewGuid().ToString(),
                                    Name = spell.DisplayName,
                                    TargetType = AlterationTargetType.Target
                                };
                            case AlterationAttackAttributeType.MeleeAllInRange:
                                return new AttackAttributeMeleeAlterationEffectTemplate()
                                {
                                    AttackAttributes = spell.Effect.AttackAttributes,
                                    CombatType = AlterationAttackAttributeCombatType.Malign,
                                    Guid = Guid.NewGuid().ToString(),
                                    Name = spell.DisplayName,
                                    TargetType = AlterationTargetType.AllInRange
                                };
                            case AlterationAttackAttributeType.MeleeAllInRangeExceptSource:
                                return new AttackAttributeMeleeAlterationEffectTemplate()
                                {
                                    AttackAttributes = spell.Effect.AttackAttributes,
                                    CombatType = AlterationAttackAttributeCombatType.Malign,
                                    Guid = Guid.NewGuid().ToString(),
                                    Name = spell.DisplayName,
                                    TargetType = AlterationTargetType.AllInRangeExceptSource
                                };
                            case AlterationAttackAttributeType.TemporaryMalignAllInRange:
                                return new AttackAttributeTemporaryAlterationEffectTemplate()
                                {
                                    AttackAttributes = spell.Effect.AttackAttributes,
                                    CombatType = AlterationAttackAttributeCombatType.Malign,
                                    Guid = Guid.NewGuid().ToString(),
                                    Name = spell.DisplayName,
                                    TargetType = AlterationTargetType.AllInRange
                                };
                            case AlterationAttackAttributeType.TemporaryMalignAllInRangeExceptSource:
                                return new AttackAttributeTemporaryAlterationEffectTemplate()
                                {
                                    AttackAttributes = spell.Effect.AttackAttributes,
                                    CombatType = AlterationAttackAttributeCombatType.Malign,
                                    Guid = Guid.NewGuid().ToString(),
                                    Name = spell.DisplayName,
                                    TargetType = AlterationTargetType.AllInRangeExceptSource
                                };
                            default:
                                throw new Exception("Attack Attribute not handled");
                        }
                    }
                case AlterationType.Remedy:
                    return new RemedyAlterationEffectTemplate()
                    {
                        Guid = Guid.NewGuid().ToString(),
                        Name = spell.DisplayName,
                        RemediedState = spell.Effect.RemediedState
                    };
                default:
                    throw new Exception("Unhandled SpellTemplate Type");
            }
        }
        private SymbolDeltaTemplate MapSymbol(SymbolDetailsTemplate template)
        {
            return new SymbolDeltaTemplate()
            {
                CharacterColor = template.CharacterColor,
                CharacterSymbol = template.CharacterSymbol,
                Guid = template.Guid,
                Icon = template.Icon,
                IsAuraDelta = template.IsAuraDelta,
                IsBodyDelta = template.IsBodyDelta,
                IsCharacterDelta = template.IsCharacterDelta,
                IsColorDelta = template.IsColorDelta,
                IsFullSymbolDelta = template.IsFullSymbolDelta,
                IsImageDelta = template.IsImageDelta,
                IsLineDelta = template.IsLineDelta,
                IsMoodDelta = template.IsMoodDelta,
                Name = template.Name,
                SmileyAuraColor = template.SmileyAuraColor,
                SmileyBodyColor = template.SmileyBodyColor,
                SmileyLineColor = template.SmileyLineColor,
                SmileyMood = template.SmileyMood,
                Type = template.Type
            };
        }
        private AnimationGroupTemplate CreateAnimation(List<AnimationTemplate> animations)
        {
            return new AnimationGroupTemplate()
            {
                Animations = animations
            };
        }

        private void MapAnimationType(AnimationTemplate template)
        {
            switch (template.Type)
            {
                case AnimationType.ProjectileSelfToTarget:
                case AnimationType.ProjectileSelfToTargetsInRange:
                    template.BaseType = AnimationBaseType.Projectile;
                    template.TargetType = AnimationTargetType.AffectedCharacters;
                    break;
                case AnimationType.ProjectileTargetToSelf:
                case AnimationType.ProjectileTargetsInRangeToSelf:
                    template.BaseType = AnimationBaseType.ProjectileReverse;
                    template.TargetType = AnimationTargetType.AffectedCharacters;
                    break;
                case AnimationType.AuraSelf:
                    template.BaseType = AnimationBaseType.Aura;
                    template.TargetType = AnimationTargetType.SourceCharacter;
                    break;
                case AnimationType.AuraTarget:
                    template.BaseType = AnimationBaseType.Aura;
                    template.TargetType = AnimationTargetType.AffectedCharacters;
                    break;
                case AnimationType.BubblesSelf:
                    template.BaseType = AnimationBaseType.Bubbles;
                    template.TargetType = AnimationTargetType.SourceCharacter;
                    break;
                case AnimationType.BubblesTarget:
                    template.BaseType = AnimationBaseType.Bubbles;
                    template.TargetType = AnimationTargetType.AffectedCharacters;
                    break;
                case AnimationType.BubblesScreen:
                    template.BaseType = AnimationBaseType.Bubbles;
                    template.TargetType = AnimationTargetType.Screen;
                    break;
                case AnimationType.BarrageSelf:
                    template.BaseType = AnimationBaseType.Barrage;
                    template.TargetType = AnimationTargetType.SourceCharacter;
                    break;
                case AnimationType.BarrageTarget:
                    template.BaseType = AnimationBaseType.Barrage;
                    template.TargetType = AnimationTargetType.AffectedCharacters;
                    break;
                case AnimationType.SpiralSelf:
                    template.BaseType = AnimationBaseType.Spiral;
                    template.TargetType = AnimationTargetType.SourceCharacter;
                    break;
                case AnimationType.SpiralTarget:
                    template.BaseType = AnimationBaseType.Spiral;
                    template.TargetType = AnimationTargetType.AffectedCharacters;
                    break;
                case AnimationType.ChainSelfToTargetsInRange:
                    template.BaseType = AnimationBaseType.Chain;
                    template.TargetType = AnimationTargetType.AffectedCharacters;
                    break;
                case AnimationType.ScreenBlink:
                    template.BaseType = AnimationBaseType.ScreenBlink;
                    template.TargetType = AnimationTargetType.Screen;
                    break;
                default:
                    break;
            }
        }
        
        public TDest MapObject<TSource, TDest>(TSource source, bool reverse)
        {
            // Map base ignoring collections
            var dest = Construct<TDest>();

            // Map collection properties separately
            var destProperties = typeof(TDest).GetProperties()
                                              .ToDictionary(x => x.Name, x => x);

            var sourceProperties = typeof(TSource).GetProperties()
                                                  .ToDictionary(x => x.Name, x => x);

            foreach (var sourceProperty in sourceProperties)
            {
                // Source Object
                var sourcePropertyValue = sourceProperty.Value.GetValue(source);

                // Skip Null Source Properties
                if (sourcePropertyValue == null)
                    continue;

                // Have to check collections first
                if (typeof(IList).IsAssignableFrom(sourceProperty.Value.PropertyType))
                {
                    var sourceList = (IList)sourcePropertyValue;

                    if (sourceList.Count <= 0)
                        continue;

                    var destList = (IList)destProperties[sourceProperty.Key].GetValue(dest);

                    // Get Source / Dest Generic Type - MUST BE ONE ONLY
                    var sourceItemType = sourceProperty.Value.PropertyType.GetGenericArguments().First();
                    var destItemType = destProperties[sourceProperty.Key].PropertyType.GetGenericArguments().First();

                    // Call method to map collection items -> Recurses Map<,>
                    MapCollectionInit(sourceList, destList, sourceItemType, destItemType, reverse);
                }

                // Next, Check for Value Types (EXCLUDES INTERFACES)
                else if (IsValueType(sourceProperty.Value.PropertyType))
                    destProperties[sourceProperty.Key].SetValue(dest, sourcePropertyValue);

                // Non-Collection Complex Types (INCLUDES INTERFACES)
                else
                {
                    // Source / Dest property types
                    var sourcePropertyType = sourceProperty.Value.PropertyType;
                    var destPropertyType = destProperties[sourceProperty.Key].PropertyType;

                    // INTERFACES
                    //
                    // Convention is to have interface types match with a suffix (IType -> ITypeViewModel).
                    // These are found here as the source / dest property types - will have matching interface
                    // names (one with the "ViewModel" suffix).
                    //
                    // We have to identify the underlying matched implementation types using pre-calculated 
                    // type information from the namespaces.

                    if (sourcePropertyType.IsInterface &&
                        destPropertyType.IsInterface)
                    {
                        // Proper IMPLEMENTATION type comes from the actual source value type
                        sourcePropertyType = sourcePropertyValue.GetType();
                        destPropertyType = reverse ? _reverseTypeMap[sourcePropertyType] :
                                                     _forwardTypeMap[sourcePropertyType];

                        // Then, just let the proper types fall through to recurse as they otherwise would.
                    }
                    else if (sourcePropertyType.IsInterface ||
                             destPropertyType.IsInterface)
                        throw new Exception("Mis-matching interface definitions");

                    // Create method call to MapBack<TSource, TDest> using reflection
                    var methodInfo = typeof(ScenarioConfigurationMapper).GetMethod("MapObject");
                    var genericMethodInfo = methodInfo.MakeGenericMethod(sourcePropertyType, destPropertyType);

                    var destObject = genericMethodInfo.Invoke(this, new object[] { sourcePropertyValue, reverse });

                    // Set Dest property
                    destProperties[sourceProperty.Key].SetValue(dest, destObject);
                }
            }

            return dest;
        }

        public void MapCollectionInit(IList sourceCollection, IList destCollection, Type sourceItemType, Type destItemType, bool reverse)
        {
            // Create method call to MapBack<TSource, TDest> using reflection
            var methodInfo = typeof(ScenarioConfigurationMapper).GetMethod("MapCollection");
            var genericMethodInfo = methodInfo.MakeGenericMethod(sourceItemType, destItemType);

            genericMethodInfo.Invoke(this, new object[] { sourceCollection, destCollection, reverse });
        }

        public void MapCollection<TSource, TDest>(IList<TSource> sourceCollection, IList<TDest> destCollection, bool reverse)
        {
            // Create method call to MapBack<TSource, TDest> using reflection
            var methodInfo = typeof(ScenarioConfigurationMapper).GetMethod("MapObject");
            var genericMethodInfo = methodInfo.MakeGenericMethod(typeof(TSource), typeof(TDest));

            // Use Recursion to map back object graph
            foreach (var sourceItem in sourceCollection)
            {
                // Create destination item recursively
                var destItem = (TDest)genericMethodInfo.Invoke(this, new object[] { sourceItem, reverse });

                // Add to destination collection
                destCollection.Add(destItem);
            }
        }

        /// <summary>
        /// Mapping breaks references due to issues with this algorithm. This has to be run prior to returning the object.
        /// </summary>
        public ScenarioConfigurationContainer FixReferences(ScenarioConfigurationContainer configuration)
        {
            // Animations
            foreach (var template in configuration.AnimationTemplates)
            {
                template.FillTemplate = Match(configuration.BrushTemplates, template.FillTemplate);
                template.StrokeTemplate = Match(configuration.BrushTemplates, template.StrokeTemplate);
            }

            // Alterations
            foreach (var template in configuration.MagicSpells)
            {
                MatchCollection(configuration.AnimationTemplates, template.Animations);

                template.Effect.AlteredState = Match(configuration.AlteredCharacterStates, template.Effect.AlteredState);
                template.AuraEffect.AlteredState = Match(configuration.AlteredCharacterStates, template.AuraEffect.AlteredState);

                template.Effect.RemediedState = Match(configuration.AlteredCharacterStates, template.Effect.RemediedState);
                template.AuraEffect.RemediedState = Match(configuration.AlteredCharacterStates, template.AuraEffect.RemediedState);
            }

            // Skill Sets
            foreach (var template in configuration.SkillTemplates)
            {
                foreach (var skillTemplate in template.Skills)
                    skillTemplate.Alteration = Match(configuration.MagicSpells, skillTemplate.Alteration);
            }

            // Doodads
            foreach (var template in configuration.DoodadTemplates)
            {
                template.AutomaticMagicSpellTemplate = Match(configuration.MagicSpells, template.AutomaticMagicSpellTemplate);
                template.InvokedMagicSpellTemplate = Match(configuration.MagicSpells, template.InvokedMagicSpellTemplate);
            }

            // Consumables
            foreach (var template in configuration.ConsumableTemplates)
            {
                template.AmmoSpellTemplate = Match(configuration.MagicSpells, template.AmmoSpellTemplate);
                template.LearnedSkill = Match(configuration.SkillTemplates, template.LearnedSkill);
                template.ProjectileSpellTemplate = Match(configuration.MagicSpells, template.ProjectileSpellTemplate);
                template.SpellTemplate = Match(configuration.MagicSpells, template.SpellTemplate);
            }

            // Equipment
            foreach (var template in configuration.EquipmentTemplates)
            {
                template.AmmoTemplate = Match(configuration.ConsumableTemplates, template.AmmoTemplate);
                template.CurseSpell = Match(configuration.MagicSpells, template.CurseSpell);
                template.EquipSpell = Match(configuration.MagicSpells, template.EquipSpell);
            }

            // Enemies
            foreach (var template in configuration.EnemyTemplates)
            {
                for (int i = 0; i < template.StartingConsumables.Count; i++)
                    template.StartingConsumables[i].TheTemplate = Match(configuration.ConsumableTemplates, template.StartingConsumables[i].TheTemplate);

                for (int i = 0; i < template.StartingEquipment.Count; i++)
                    template.StartingEquipment[i].TheTemplate = Match(configuration.EquipmentTemplates, template.StartingEquipment[i].TheTemplate);

                // Behavior Skills
                for (int i = 0; i < template.BehaviorDetails.Behaviors.Count; i++)
                    template.BehaviorDetails.Behaviors[i].EnemySpell = Match(configuration.MagicSpells, template.BehaviorDetails.Behaviors[i].EnemySpell);
            }

            // Player
            MatchCollection(configuration.SkillTemplates, configuration.PlayerTemplate.Skills);

            for (int i = 0; i < configuration.PlayerTemplate.StartingConsumables.Count; i++)
                configuration.PlayerTemplate.StartingConsumables[i].TheTemplate = Match(configuration.ConsumableTemplates, configuration.PlayerTemplate.StartingConsumables[i].TheTemplate);

            for (int i = 0; i < configuration.PlayerTemplate.StartingEquipment.Count; i++)
                configuration.PlayerTemplate.StartingEquipment[i].TheTemplate = Match(configuration.EquipmentTemplates, configuration.PlayerTemplate.StartingEquipment[i].TheTemplate);

            return configuration;
        }

        /// <summary>
        /// Mapping breaks references due to issues with this algorithm. This has to be run prior to returning the object.
        /// </summary>
        public ScenarioConfigurationContainerViewModel FixReferences(ScenarioConfigurationContainerViewModel configuration)
        {
            // Animations
            foreach (var template in configuration.AnimationTemplates)
            {
                template.FillTemplate = MatchVM(configuration.BrushTemplates, template.FillTemplate);
                template.StrokeTemplate = MatchVM(configuration.BrushTemplates, template.StrokeTemplate);
            }

            // Alterations
            foreach (var template in configuration.MagicSpells)
            {
                MatchCollectionVM(configuration.AnimationTemplates, template.Animations);

                template.Effect.AlteredState = MatchVM(configuration.AlteredCharacterStates, template.Effect.AlteredState);
                template.AuraEffect.AlteredState = MatchVM(configuration.AlteredCharacterStates, template.AuraEffect.AlteredState);

                template.Effect.RemediedState = MatchVM(configuration.AlteredCharacterStates, template.Effect.RemediedState);
                template.AuraEffect.RemediedState = MatchVM(configuration.AlteredCharacterStates, template.AuraEffect.RemediedState);
            }

            // Skill Sets
            foreach (var template in configuration.SkillTemplates)
            {
                foreach (var skillTemplate in template.Skills)
                    skillTemplate.Alteration = MatchVM(configuration.MagicSpells, skillTemplate.Alteration);
            }

            // Doodads
            foreach (var template in configuration.DoodadTemplates)
            {
                template.AutomaticMagicSpellTemplate = MatchVM(configuration.MagicSpells, template.AutomaticMagicSpellTemplate);
                template.InvokedMagicSpellTemplate = MatchVM(configuration.MagicSpells, template.InvokedMagicSpellTemplate);
            }

            // Consumables
            foreach (var template in configuration.ConsumableTemplates)
            {
                template.AmmoSpellTemplate = MatchVM(configuration.MagicSpells, template.AmmoSpellTemplate);
                template.LearnedSkill = MatchVM(configuration.SkillTemplates, template.LearnedSkill);
                template.ProjectileSpellTemplate = MatchVM(configuration.MagicSpells, template.ProjectileSpellTemplate);
                template.SpellTemplate = MatchVM(configuration.MagicSpells, template.SpellTemplate);
            }

            // Equipment
            foreach (var template in configuration.EquipmentTemplates)
            {
                template.AmmoTemplate = MatchVM(configuration.ConsumableTemplates, template.AmmoTemplate);
                template.CurseSpell = MatchVM(configuration.MagicSpells, template.CurseSpell);
                template.EquipSpell = MatchVM(configuration.MagicSpells, template.EquipSpell);
            }

            // Enemies
            foreach (var template in configuration.EnemyTemplates)
            {
                for (int i = 0; i < template.StartingConsumables.Count; i++)
                    template.StartingConsumables[i].TheTemplate = MatchVM(configuration.ConsumableTemplates, template.StartingConsumables[i].TheTemplate);

                for (int i = 0; i < template.StartingEquipment.Count; i++)
                    template.StartingEquipment[i].TheTemplate = MatchVM(configuration.EquipmentTemplates, template.StartingEquipment[i].TheTemplate);

                // Behavior Skills
                for (int i = 0; i < template.BehaviorDetails.Behaviors.Count; i++)
                    template.BehaviorDetails.Behaviors[i].EnemySpell = MatchVM(configuration.MagicSpells, template.BehaviorDetails.Behaviors[i].EnemySpell);
            }

            // Player
            MatchCollectionVM(configuration.SkillTemplates, configuration.PlayerTemplate.Skills);

            for (int i = 0; i < configuration.PlayerTemplate.StartingConsumables.Count; i++)
                configuration.PlayerTemplate.StartingConsumables[i].TheTemplate = MatchVM(configuration.ConsumableTemplates, configuration.PlayerTemplate.StartingConsumables[i].TheTemplate);

            for (int i = 0; i < configuration.PlayerTemplate.StartingEquipment.Count; i++)
                configuration.PlayerTemplate.StartingEquipment[i].TheTemplate = MatchVM(configuration.EquipmentTemplates, configuration.PlayerTemplate.StartingEquipment[i].TheTemplate);

            return configuration;
        }

        private T Match<T>(IList<T> source, T dest) where T : Template
        {
            if (dest == null)
                return dest;

            var item = source.FirstOrDefault(x => x.Guid == dest.Guid);

            // NOTE*** This will prevent null values in the configuration; but these are handled by 
            //         boolean values to show whether they have valid data (and are even used). The reason
            //         for this is to prevent "Null Reference" during mapping. (don't know why)
            return item == null ? dest : item;
        }

        private void MatchCollection<T>(IList<T> source, IList<T> dest) where T : Template
        {
            for (int i = 0; i < dest.Count; i++)
            {
                // Doing this to avoid NotifyCollectionChanged.Replace
                var replaceItem = source.First(x => x.Guid == dest[i].Guid);
                dest.RemoveAt(i);
                dest.Insert(i, replaceItem);
            }
        }

        private T MatchVM<T>(IList<T> source, T dest) where T : TemplateViewModel
        {
            if (dest == null)
                return dest;

            var item = source.FirstOrDefault(x => x.Guid == dest.Guid);

            // NOTE*** This will prevent null values in the configuration; but these are handled by 
            //         boolean values to show whether they have valid data (and are even used). The reason
            //         for this is to prevent "Null Reference" during mapping. (don't know why)
            return item == null ? dest : item;
        }

        private void MatchCollectionVM<T>(IList<T> source, IList<T> dest) where T : TemplateViewModel
        {
            for (int i = 0; i < dest.Count; i++)
            {
                // Doing this to avoid NotifyCollectionChanged.Replace
                var replaceItem = source.First(x => x.Guid == dest[i].Guid);
                dest.RemoveAt(i);
                dest.Insert(i, replaceItem);
            }
        }

        /// <summary>
        /// Creates a new instance of type T using the default constructor
        /// </summary>
        private T Construct<T>()
        {
            if (typeof(T).IsInterface)
            {
                int foo = 4;
            }

            var constructor = typeof(T).GetConstructor(new Type[] { });

            return (T)(constructor == null ? default(T) : constructor.Invoke(new object[] { }));
        }

        private bool IsValueType(Type type)
        {
            return (type.GetConstructor(new Type[] { }) == null) &&
                   !type.IsInterface;
        }
    }
}
