﻿using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.Core.Model.Scenario.Alteration.Consumable;
using Rogue.NET.Core.Model.Scenario.Alteration.Doodad;
using Rogue.NET.Core.Model.Scenario.Alteration.Effect;
using Rogue.NET.Core.Model.Scenario.Alteration.Enemy;
using Rogue.NET.Core.Model.Scenario.Alteration.Equipment;
using Rogue.NET.Core.Model.Scenario.Alteration.Friendly;
using Rogue.NET.Core.Model.Scenario.Alteration.Interface;
using Rogue.NET.Core.Model.Scenario.Alteration.Skill;
using Rogue.NET.Core.Model.Scenario.Alteration.TemporaryCharacter;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Common;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Consumable;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Doodad;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Enemy;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Equipment;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Friendly;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Interface;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Skill;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.TemporaryCharacter;
using Rogue.NET.Core.Processing.Model.Generator.Interface;
using System;
using System.ComponentModel.Composition;
using System.Linq;

namespace Rogue.NET.Core.Processing.Model.Generator
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(IAlterationGenerator))]
    public class AlterationGenerator : IAlterationGenerator
    {
        private readonly IRandomSequenceGenerator _randomSequenceGenerator;
        private readonly IAttackAttributeGenerator _attackAttributeGenerator;
        private readonly IAlteredStateGenerator _alteredStateGenerator;
        private readonly IAnimationGenerator _animationGenerator;
        private readonly IItemGenerator _itemGenerator;
        private readonly ISymbolDetailsGenerator _symbolDetailsGenerator;

        [ImportingConstructor]
        public AlterationGenerator(
            IRandomSequenceGenerator randomSequenceGenerator,
            IAttackAttributeGenerator attackAttributeGenerator,
            IAlteredStateGenerator alteredStateGenerator,
            IAnimationGenerator animationGenerator,
            IItemGenerator itemGenerator,
            ISymbolDetailsGenerator symbolDetailsGenerator)
        {
            _randomSequenceGenerator = randomSequenceGenerator;
            _attackAttributeGenerator = attackAttributeGenerator;
            _alteredStateGenerator = alteredStateGenerator;
            _animationGenerator = animationGenerator;
            _itemGenerator = itemGenerator;
            _symbolDetailsGenerator = symbolDetailsGenerator;
        }

        public AlterationContainer GenerateAlteration(AlterationTemplate template)
        {
            if (template is ConsumableProjectileAlterationTemplate)
                return GenerateAlteration(template as ConsumableProjectileAlterationTemplate);

            else if (template is ConsumableAlterationTemplate)
                return GenerateAlteration(template as ConsumableAlterationTemplate);

            else if (template is EnemyAlterationTemplate)
                return GenerateAlteration(template as EnemyAlterationTemplate);

            else if (template is FriendlyAlterationTemplate)
                return GenerateAlteration(template as FriendlyAlterationTemplate);

            else if (template is TemporaryCharacterAlterationTemplate)
                return GenerateAlteration(template as TemporaryCharacterAlterationTemplate);

            else if (template is EquipmentAttackAlterationTemplate)
                return GenerateAlteration(template as EquipmentAttackAlterationTemplate);

            else if (template is EquipmentEquipAlterationTemplate)
                return GenerateAlteration(template as EquipmentEquipAlterationTemplate);

            else if (template is EquipmentCurseAlterationTemplate)
                return GenerateAlteration(template as EquipmentCurseAlterationTemplate);

            else if (template is DoodadAlterationTemplate)
                return GenerateAlteration(template as DoodadAlterationTemplate);

            else if (template is SkillAlterationTemplate)
                return GenerateAlteration(template as SkillAlterationTemplate);

            else
                throw new Exception("Unhandled AlterationTemplate Type");
        }

        public AlterationCost GenerateAlterationCost(AlterationCostTemplate template)
        {
            return new AlterationCost()
            {
                Experience = template.Experience,
                Health = template.Health,
                Hunger = template.Hunger,
                Stamina = template.Stamina
            };
        }

        public ConsumableAlteration GenerateAlteration(ConsumableAlterationTemplate template)
        {
            return new ConsumableAlteration()
            {
                Animation = _animationGenerator.GenerateAnimation(template.Animation),
                Cost = GenerateAlterationCost(template.Cost),
                Category = CreateAlterationCategory(template.AlterationCategory),
                Effect = GenerateAlterationEffect(template.Effect as IConsumableAlterationEffectTemplate),
                RogueName = template.Name
            };
        }

        public ConsumableProjectileAlteration GenerateAlteration(ConsumableProjectileAlterationTemplate template)
        {
            return new ConsumableProjectileAlteration()
            {
                Animation = _animationGenerator.GenerateAnimation(template.Animation),
                Effect = GenerateAlterationEffect(template.Effect as IConsumableProjectileAlterationEffectTemplate),
                Category = CreateAlterationCategory(template.AlterationCategory),
                RogueName = template.Name
            };
        }

        public DoodadAlteration GenerateAlteration(DoodadAlterationTemplate template)
        {
            return new DoodadAlteration()
            {
                Animation = _animationGenerator.GenerateAnimation(template.Animation),
                Category = CreateAlterationCategory(template.AlterationCategory),
                Effect = GenerateAlterationEffect(template.Effect as IDoodadAlterationEffectTemplate),
                RogueName = template.Name
            };
        }

        public EnemyAlteration GenerateAlteration(EnemyAlterationTemplate template)
        {
            return new EnemyAlteration()
            {
                Animation = _animationGenerator.GenerateAnimation(template.Animation),
                Cost = GenerateAlterationCost(template.Cost),
                Category = CreateAlterationCategory(template.AlterationCategory),
                Effect = GenerateAlterationEffect(template.Effect as IEnemyAlterationEffectTemplate),
                RogueName = template.Name
            };
        }

        public FriendlyAlteration GenerateAlteration(FriendlyAlterationTemplate template)
        {
            return new FriendlyAlteration()
            {
                Animation = _animationGenerator.GenerateAnimation(template.Animation),
                Cost = GenerateAlterationCost(template.Cost),
                Category = CreateAlterationCategory(template.AlterationCategory),
                Effect = GenerateAlterationEffect(template.Effect as IFriendlyAlterationEffectTemplate),
                RogueName = template.Name
            };
        }

        public TemporaryCharacterAlteration GenerateAlteration(TemporaryCharacterAlterationTemplate template)
        {
            return new TemporaryCharacterAlteration()
            {
                Animation = _animationGenerator.GenerateAnimation(template.Animation),
                Cost = GenerateAlterationCost(template.Cost),
                Category = CreateAlterationCategory(template.AlterationCategory),
                Effect = GenerateAlterationEffect(template.Effect as ITemporaryCharacterAlterationEffectTemplate),
                RogueName = template.Name
            };
        }

        public EquipmentAttackAlteration GenerateAlteration(EquipmentAttackAlterationTemplate template)
        {
            return new EquipmentAttackAlteration()
            {
                Animation = _animationGenerator.GenerateAnimation(template.Animation),
                Cost = GenerateAlterationCost(template.Cost),
                Category = CreateAlterationCategory(template.AlterationCategory),
                Effect = GenerateAlterationEffect(template.Effect as IEquipmentAttackAlterationEffectTemplate),
                RogueName = template.Name
            };
        }

        public EquipmentCurseAlteration GenerateAlteration(EquipmentCurseAlterationTemplate template)
        {
            return new EquipmentCurseAlteration()
            {
                AuraParameters = new AuraSourceParameters()
                {
                    AuraColor = template.AuraParameters.AuraColor,
                    AuraRange = template.AuraParameters.AuraRange
                },
                Category = CreateAlterationCategory(template.AlterationCategory),
                Effect = GenerateAlterationEffect(template.Effect as IEquipmentCurseAlterationEffectTemplate),
                RogueName = template.Name
            };
        }

        public EquipmentEquipAlteration GenerateAlteration(EquipmentEquipAlterationTemplate template)
        {
            return new EquipmentEquipAlteration()
            {
                AuraParameters = new AuraSourceParameters()
                {
                    AuraColor = template.AuraParameters.AuraColor,
                    AuraRange = template.AuraParameters.AuraRange
                },
                Category = CreateAlterationCategory(template.AlterationCategory),
                Effect = GenerateAlterationEffect(template.Effect as IEquipmentEquipAlterationEffectTemplate),
                RogueName = template.Name
            };
        }

        public SkillAlteration GenerateAlteration(SkillAlterationTemplate template)
        {
            return new SkillAlteration()
            {
                Animation = _animationGenerator.GenerateAnimation(template.Animation),
                AuraParameters = new AuraSourceParameters()
                {
                    AuraColor = template.AuraParameters.AuraColor,
                    AuraRange = template.AuraParameters.AuraRange
                },
                Category = CreateAlterationCategory(template.AlterationCategory),
                Cost = GenerateAlterationCost(template.Cost),
                Effect = GenerateAlterationEffect(template.Effect as ISkillAlterationEffectTemplate),
                RogueName = template.Name
            };
        }

        #region Interface Type Inspectors
        protected IConsumableAlterationEffect GenerateAlterationEffect(IConsumableAlterationEffectTemplate template)
        {
            if (template is AttackAttributeMeleeAlterationEffectTemplate)
                return GenerateAlterationEffect(template as AttackAttributeMeleeAlterationEffectTemplate);

            else if (template is AttackAttributeTemporaryAlterationEffectTemplate)
                return GenerateAlterationEffect(template as AttackAttributeTemporaryAlterationEffectTemplate);

            else if (template is ChangeLevelAlterationEffectTemplate)
                return GenerateAlterationEffect(template as ChangeLevelAlterationEffectTemplate);

            else if (template is CreateEnemyAlterationEffectTemplate)
                return GenerateAlterationEffect(template as CreateEnemyAlterationEffectTemplate);

            else if (template is CreateFriendlyAlterationEffectTemplate)
                return GenerateAlterationEffect(template as CreateFriendlyAlterationEffectTemplate);

            else if (template is CreateTemporaryCharacterAlterationEffectTemplate)
                return GenerateAlterationEffect(template as CreateTemporaryCharacterAlterationEffectTemplate);

            else if (template is DetectAlterationAlignmentAlterationEffectTemplate)
                return GenerateAlterationEffect(template as DetectAlterationAlignmentAlterationEffectTemplate);

            else if (template is DetectAlterationAlterationEffectTemplate)
                return GenerateAlterationEffect(template as DetectAlterationAlterationEffectTemplate);

            else if (template is EquipmentDamageAlterationEffectTemplate)
                return GenerateAlterationEffect(template as EquipmentDamageAlterationEffectTemplate);

            else if (template is EquipmentEnhanceAlterationEffectTemplate)
                return GenerateAlterationEffect(template as EquipmentEnhanceAlterationEffectTemplate);

            else if (template is IdentifyAlterationEffectTemplate)
                return GenerateAlterationEffect(template as IdentifyAlterationEffectTemplate);

            else if (template is PermanentAlterationEffectTemplate)
                return GenerateAlterationEffect(template as PermanentAlterationEffectTemplate);

            else if (template is RemedyAlterationEffectTemplate)
                return GenerateAlterationEffect(template as RemedyAlterationEffectTemplate);

            else if (template is RevealAlterationEffectTemplate)
                return GenerateAlterationEffect(template as RevealAlterationEffectTemplate);

            else if (template is TeleportRandomAlterationEffectTemplate)
                return GenerateAlterationEffect(template as TeleportRandomAlterationEffectTemplate);

            else if (template is TemporaryAlterationEffectTemplate)
                return GenerateAlterationEffect(template as TemporaryAlterationEffectTemplate);

            else if (template is TransmuteAlterationEffectTemplate)
                return GenerateAlterationEffect(template as TransmuteAlterationEffectTemplate);

            else if (template is UncurseAlterationEffectTemplate)
                return GenerateAlterationEffect(template as UncurseAlterationEffectTemplate);

            else
                throw new Exception("Unhandled IConsumableAlterationEffectTemplate Type");
        }

        protected IConsumableProjectileAlterationEffect GenerateAlterationEffect(IConsumableProjectileAlterationEffectTemplate template)
        {
            if (template is AttackAttributeMeleeAlterationEffectTemplate)
                return GenerateAlterationEffect(template as AttackAttributeMeleeAlterationEffectTemplate);

            else if (template is AttackAttributeTemporaryAlterationEffectTemplate)
                return GenerateAlterationEffect(template as AttackAttributeTemporaryAlterationEffectTemplate);

            else if (template is PermanentAlterationEffectTemplate)
                return GenerateAlterationEffect(template as PermanentAlterationEffectTemplate);

            else if (template is TemporaryAlterationEffectTemplate)
                return GenerateAlterationEffect(template as TemporaryAlterationEffectTemplate);

            else
                throw new Exception("Unhandled IConsumableProjectileAlterationEffect Type");
        }

        protected IDoodadAlterationEffect GenerateAlterationEffect(IDoodadAlterationEffectTemplate template)
        {
            if (template is AttackAttributeMeleeAlterationEffectTemplate)
                return GenerateAlterationEffect(template as AttackAttributeMeleeAlterationEffectTemplate);

            else if (template is AttackAttributeTemporaryAlterationEffectTemplate)
                return GenerateAlterationEffect(template as AttackAttributeTemporaryAlterationEffectTemplate);

            else if (template is ChangeLevelAlterationEffectTemplate)
                return GenerateAlterationEffect(template as ChangeLevelAlterationEffectTemplate);

            else if (template is CreateEnemyAlterationEffectTemplate)
                return GenerateAlterationEffect(template as CreateEnemyAlterationEffectTemplate);

            else if (template is CreateFriendlyAlterationEffectTemplate)
                return GenerateAlterationEffect(template as CreateFriendlyAlterationEffectTemplate);

            else if (template is CreateTemporaryCharacterAlterationEffectTemplate)
                return GenerateAlterationEffect(template as CreateTemporaryCharacterAlterationEffectTemplate);

            else if (template is DetectAlterationAlignmentAlterationEffectTemplate)
                return GenerateAlterationEffect(template as DetectAlterationAlignmentAlterationEffectTemplate);

            else if (template is DetectAlterationAlterationEffectTemplate)
                return GenerateAlterationEffect(template as DetectAlterationAlterationEffectTemplate);

            else if (template is EquipmentDamageAlterationEffectTemplate)
                return GenerateAlterationEffect(template as EquipmentDamageAlterationEffectTemplate);

            else if (template is EquipmentEnhanceAlterationEffectTemplate)
                return GenerateAlterationEffect(template as EquipmentEnhanceAlterationEffectTemplate);

            else if (template is IdentifyAlterationEffectTemplate)
                return GenerateAlterationEffect(template as IdentifyAlterationEffectTemplate);

            else if (template is PermanentAlterationEffectTemplate)
                return GenerateAlterationEffect(template as PermanentAlterationEffectTemplate);

            else if (template is RemedyAlterationEffectTemplate)
                return GenerateAlterationEffect(template as RemedyAlterationEffectTemplate);

            else if (template is RevealAlterationEffectTemplate)
                return GenerateAlterationEffect(template as RevealAlterationEffectTemplate);

            else if (template is TeleportRandomAlterationEffectTemplate)
                return GenerateAlterationEffect(template as TeleportRandomAlterationEffectTemplate);

            else if (template is TemporaryAlterationEffectTemplate)
                return GenerateAlterationEffect(template as TemporaryAlterationEffectTemplate);

            else if (template is TransmuteAlterationEffectTemplate)
                return GenerateAlterationEffect(template as TransmuteAlterationEffectTemplate);

            else if (template is UncurseAlterationEffectTemplate)
                return GenerateAlterationEffect(template as UncurseAlterationEffectTemplate);

            else
                throw new Exception("Unhandled IDoodadAlterationEffectTemplate Type");
        }

        protected IEnemyAlterationEffect GenerateAlterationEffect(IEnemyAlterationEffectTemplate template)
        {
            if (template is AttackAttributeMeleeAlterationEffectTemplate)
                return GenerateAlterationEffect(template as AttackAttributeMeleeAlterationEffectTemplate);

            else if (template is AttackAttributeTemporaryAlterationEffectTemplate)
                return GenerateAlterationEffect(template as AttackAttributeTemporaryAlterationEffectTemplate);

            else if (template is CreateEnemyAlterationEffectTemplate)
                return GenerateAlterationEffect(template as CreateEnemyAlterationEffectTemplate);

            else if (template is CreateFriendlyAlterationEffectTemplate)
                return GenerateAlterationEffect(template as CreateFriendlyAlterationEffectTemplate);

            else if (template is CreateTemporaryCharacterAlterationEffectTemplate)
                return GenerateAlterationEffect(template as CreateTemporaryCharacterAlterationEffectTemplate);

            else if (template is EquipmentDamageAlterationEffectTemplate)
                return GenerateAlterationEffect(template as EquipmentDamageAlterationEffectTemplate);

            else if (template is PermanentAlterationEffectTemplate)
                return GenerateAlterationEffect(template as PermanentAlterationEffectTemplate);

            else if (template is RunAwayAlterationEffectTemplate)
                return GenerateAlterationEffect(template as RunAwayAlterationEffectTemplate);

            else if (template is StealAlterationEffectTemplate)
                return GenerateAlterationEffect(template as StealAlterationEffectTemplate);

            else if (template is TeleportRandomAlterationEffectTemplate)
                return GenerateAlterationEffect(template as TeleportRandomAlterationEffectTemplate);

            else if (template is TemporaryAlterationEffectTemplate)
                return GenerateAlterationEffect(template as TemporaryAlterationEffectTemplate);

            else
                throw new Exception("Unhandled IEnemyAlterationEffectTemplate Type");
        }

        protected IFriendlyAlterationEffect GenerateAlterationEffect(IFriendlyAlterationEffectTemplate template)
        {
            if (template is AttackAttributeMeleeAlterationEffectTemplate)
                return GenerateAlterationEffect(template as AttackAttributeMeleeAlterationEffectTemplate);

            else if (template is AttackAttributeTemporaryAlterationEffectTemplate)
                return GenerateAlterationEffect(template as AttackAttributeTemporaryAlterationEffectTemplate);

            else if (template is PermanentAlterationEffectTemplate)
                return GenerateAlterationEffect(template as PermanentAlterationEffectTemplate);

            else if (template is TemporaryAlterationEffectTemplate)
                return GenerateAlterationEffect(template as TemporaryAlterationEffectTemplate);

            else
                throw new Exception("Unhandled IFriendlyAlterationEffectTemplate Type");
        }

        protected IEquipmentAttackAlterationEffect GenerateAlterationEffect(IEquipmentAttackAlterationEffectTemplate template)
        {
            if (template is AttackAttributeMeleeAlterationEffectTemplate)
                return GenerateAlterationEffect(template as AttackAttributeMeleeAlterationEffectTemplate);

            else if (template is DrainMeleeAlterationEffectTemplate)
                return GenerateAlterationEffect(template as DrainMeleeAlterationEffectTemplate);

            else if (template is PermanentAlterationEffectTemplate)
                return GenerateAlterationEffect(template as PermanentAlterationEffectTemplate);

            else
                throw new Exception("Unhandled IEquipmentAttackAlterationEffectTemplate Type");
        }

        protected IEquipmentCurseAlterationEffect GenerateAlterationEffect(IEquipmentCurseAlterationEffectTemplate template)
        {
            if (template is AttackAttributeAuraAlterationEffectTemplate)
                return GenerateAlterationEffect(template as AttackAttributeAuraAlterationEffectTemplate);

            else if (template is AttackAttributePassiveAlterationEffectTemplate)
                return GenerateAlterationEffect(template as AttackAttributePassiveAlterationEffectTemplate);

            else if (template is AuraAlterationEffectTemplate)
                return GenerateAlterationEffect(template as AuraAlterationEffectTemplate);

            else if (template is PassiveAlterationEffectTemplate)
                return GenerateAlterationEffect(template as PassiveAlterationEffectTemplate);

            else
                throw new Exception("Unhandled IEquipmentCurseAlterationEffect Type");
        }

        protected IEquipmentEquipAlterationEffect GenerateAlterationEffect(IEquipmentEquipAlterationEffectTemplate template)
        {
            if (template is AttackAttributeAuraAlterationEffectTemplate)
                return GenerateAlterationEffect(template as AttackAttributeAuraAlterationEffectTemplate);

            else if (template is AttackAttributePassiveAlterationEffectTemplate)
                return GenerateAlterationEffect(template as AttackAttributePassiveAlterationEffectTemplate);

            else if (template is AuraAlterationEffectTemplate)
                return GenerateAlterationEffect(template as AuraAlterationEffectTemplate);

            else if (template is BlockAlterationAlterationEffectTemplate)
                return GenerateAlterationEffect(template as BlockAlterationAlterationEffectTemplate);

            else if (template is PassiveAlterationEffectTemplate)
                return GenerateAlterationEffect(template as PassiveAlterationEffectTemplate);

            else
                throw new Exception("Unhandled IEquipmentEquipAlterationEffectTemplate Type");
        }

        protected ITemporaryCharacterAlterationEffect GenerateAlterationEffect(ITemporaryCharacterAlterationEffectTemplate template)
        {
            if (template is AttackAttributeMeleeAlterationEffectTemplate)
                return GenerateAlterationEffect(template as AttackAttributeMeleeAlterationEffectTemplate);

            else if (template is AttackAttributeTemporaryAlterationEffectTemplate)
                return GenerateAlterationEffect(template as AttackAttributeTemporaryAlterationEffectTemplate);

            else if (template is PermanentAlterationEffectTemplate)
                return GenerateAlterationEffect(template as PermanentAlterationEffectTemplate);

            else if (template is TemporaryAlterationEffectTemplate)
                return GenerateAlterationEffect(template as TemporaryAlterationEffectTemplate);

            else
                throw new Exception("Unhandled ITemporaryCharacterAlterationEffectTemplate Type");
        }

        protected ISkillAlterationEffect GenerateAlterationEffect(ISkillAlterationEffectTemplate template)
        {
            if (template is AttackAttributeAuraAlterationEffectTemplate)
                return GenerateAlterationEffect(template as AttackAttributeAuraAlterationEffectTemplate);

            else if (template is AttackAttributeMeleeAlterationEffectTemplate)
                return GenerateAlterationEffect(template as AttackAttributeMeleeAlterationEffectTemplate);

            else if (template is AttackAttributePassiveAlterationEffectTemplate)
                return GenerateAlterationEffect(template as AttackAttributePassiveAlterationEffectTemplate);

            else if (template is AttackAttributeTemporaryAlterationEffectTemplate)
                return GenerateAlterationEffect(template as AttackAttributeTemporaryAlterationEffectTemplate);

            else if (template is AuraAlterationEffectTemplate)
                return GenerateAlterationEffect(template as AuraAlterationEffectTemplate);

            else if (template is BlockAlterationAlterationEffectTemplate)
                return GenerateAlterationEffect(template as BlockAlterationAlterationEffectTemplate);

            else if (template is ChangeLevelAlterationEffectTemplate)
                return GenerateAlterationEffect(template as ChangeLevelAlterationEffectTemplate);

            else if (template is CreateEnemyAlterationEffectTemplate)
                return GenerateAlterationEffect(template as CreateEnemyAlterationEffectTemplate);

            else if (template is CreateFriendlyAlterationEffectTemplate)
                return GenerateAlterationEffect(template as CreateFriendlyAlterationEffectTemplate);

            else if (template is CreateTemporaryCharacterAlterationEffectTemplate)
                return GenerateAlterationEffect(template as CreateTemporaryCharacterAlterationEffectTemplate);

            else if (template is DetectAlterationAlignmentAlterationEffectTemplate)
                return GenerateAlterationEffect(template as DetectAlterationAlignmentAlterationEffectTemplate);

            else if (template is DetectAlterationAlterationEffectTemplate)
                return GenerateAlterationEffect(template as DetectAlterationAlterationEffectTemplate);

            else if (template is EquipmentEnhanceAlterationEffectTemplate)
                return GenerateAlterationEffect(template as EquipmentEnhanceAlterationEffectTemplate);

            else if (template is IdentifyAlterationEffectTemplate)
                return GenerateAlterationEffect(template as IdentifyAlterationEffectTemplate);

            else if (template is PassiveAlterationEffectTemplate)
                return GenerateAlterationEffect(template as PassiveAlterationEffectTemplate);

            else if (template is PermanentAlterationEffectTemplate)
                return GenerateAlterationEffect(template as PermanentAlterationEffectTemplate);

            else if (template is RemedyAlterationEffectTemplate)
                return GenerateAlterationEffect(template as RemedyAlterationEffectTemplate);

            else if (template is RevealAlterationEffectTemplate)
                return GenerateAlterationEffect(template as RevealAlterationEffectTemplate);

            else if (template is StealAlterationEffectTemplate)
                return GenerateAlterationEffect(template as StealAlterationEffectTemplate);

            else if (template is TeleportRandomAlterationEffectTemplate)
                return GenerateAlterationEffect(template as TeleportRandomAlterationEffectTemplate);

            else if (template is TemporaryAlterationEffectTemplate)
                return GenerateAlterationEffect(template as TemporaryAlterationEffectTemplate);

            else if (template is TransmuteAlterationEffectTemplate)
                return GenerateAlterationEffect(template as TransmuteAlterationEffectTemplate);

            else if (template is UncurseAlterationEffectTemplate)
                return GenerateAlterationEffect(template as UncurseAlterationEffectTemplate);

            else
                throw new Exception("Unhandled ISkillAlterationEffectTemplate Type");
        }
        #endregion

        #region Alteration Effect Generators
        protected AttackAttributeAuraAlterationEffect GenerateAlterationEffect(AttackAttributeAuraAlterationEffectTemplate template)
        {
            return new AttackAttributeAuraAlterationEffect()
            {
                AttackAttributes = template.AttackAttributes.Select(x => _attackAttributeGenerator.GenerateAttackAttribute(x)).ToList(),
                CombatType = template.CombatType,
                RogueName = template.Name,
                SymbolAlteration = template.SymbolAlteration
            };
        }

        protected AttackAttributeMeleeAlterationEffect GenerateAlterationEffect(AttackAttributeMeleeAlterationEffectTemplate template)
        {
            return new AttackAttributeMeleeAlterationEffect()
            {
                AttackAttributes = template.AttackAttributes.Select(x => _attackAttributeGenerator.GenerateAttackAttribute(x)).ToList(),
                RogueName = template.Name
            };
        }

        protected AttackAttributePassiveAlterationEffect GenerateAlterationEffect(AttackAttributePassiveAlterationEffectTemplate template)
        {
            return new AttackAttributePassiveAlterationEffect()
            {
                AttackAttributes = template.AttackAttributes.Select(x => _attackAttributeGenerator.GenerateAttackAttribute(x)).ToList(),
                CombatType = template.CombatType,
                RogueName = template.Name,
                SymbolAlteration = template.SymbolAlteration
            };
        }

        protected AttackAttributeTemporaryAlterationEffect GenerateAlterationEffect(AttackAttributeTemporaryAlterationEffectTemplate template)
        {
            return new AttackAttributeTemporaryAlterationEffect()
            {
                AlteredState = _alteredStateGenerator.GenerateAlteredState(template.AlteredState),
                AttackAttributes = template.AttackAttributes.Select(x => _attackAttributeGenerator.GenerateAttackAttribute(x)).ToList(),
                CombatType = template.CombatType,
                EventTime = _randomSequenceGenerator.GetRandomValue(template.EventTime),
                IsStackable = template.IsStackable,
                RogueName = template.Name,
                HasAlteredState = template.HasAlteredState,
                SymbolAlteration = template.SymbolAlteration
            };
        }

        protected AuraAlterationEffect GenerateAlterationEffect(AuraAlterationEffectTemplate template)
        {
            return new AuraAlterationEffect()
            {
                Agility = _randomSequenceGenerator.GetRandomValue(template.AgilityRange),
                Attack = _randomSequenceGenerator.GetRandomValue(template.AttackRange),
                Defense = _randomSequenceGenerator.GetRandomValue(template.DefenseRange),
                HealthPerStep = _randomSequenceGenerator.GetRandomValue(template.HealthPerStepRange),
                Intelligence = _randomSequenceGenerator.GetRandomValue(template.IntelligenceRange),
                StaminaPerStep = _randomSequenceGenerator.GetRandomValue(template.StaminaPerStepRange),
                RogueName = template.Name,
                Speed = _randomSequenceGenerator.GetRandomValue(template.SpeedRange),
                Strength = _randomSequenceGenerator.GetRandomValue(template.StrengthRange),
                SymbolAlteration = template.SymbolAlteration
            };
        }

        protected BlockAlterationAlterationEffect GenerateAlterationEffect(BlockAlterationAlterationEffectTemplate template)
        {
            return new BlockAlterationAlterationEffect()
            {
                AlterationCategory = CreateAlterationCategory(template.AlterationCategory),
                RogueName = template.Name
            };
        }

        protected ChangeLevelAlterationEffect GenerateAlterationEffect(ChangeLevelAlterationEffectTemplate template)
        {
            return new ChangeLevelAlterationEffect()
            {
                LevelChange = _randomSequenceGenerator.GetRandomValue(template.LevelChange),
                RogueName = template.Name
            };
        }

        protected CreateEnemyAlterationEffect GenerateAlterationEffect(CreateEnemyAlterationEffectTemplate template)
        {
            return new CreateEnemyAlterationEffect()
            {
                Enemy = template.Enemy,
                RandomPlacementType = template.RandomPlacementType,
                Range = template.Range,
                RogueName = template.Name
            };
        }

        protected CreateFriendlyAlterationEffect GenerateAlterationEffect(CreateFriendlyAlterationEffectTemplate template)
        {
            return new CreateFriendlyAlterationEffect()
            {
                Friendly = template.Friendly,
                RandomPlacementType = template.RandomPlacementType,
                Range = template.Range,
                RogueName = template.Name
            };
        }

        protected CreateTemporaryCharacterAlterationEffect GenerateAlterationEffect(CreateTemporaryCharacterAlterationEffectTemplate template)
        {
            return new CreateTemporaryCharacterAlterationEffect()
            {
                TemporaryCharacter = template.TemporaryCharacter,
                RandomPlacementType = template.RandomPlacementType,
                Range = template.Range,
                RogueName = template.Name
            };
        }

        protected DetectAlterationAlterationEffect GenerateAlterationEffect(DetectAlterationAlterationEffectTemplate template)
        {
            return new DetectAlterationAlterationEffect()
            {
                AlterationCategory = CreateAlterationCategory(template.AlterationCategory),
                RogueName = template.Name
            };
        }

        protected DetectAlterationAlignmentAlterationEffect GenerateAlterationEffect(DetectAlterationAlignmentAlterationEffectTemplate template)
        {
            return new DetectAlterationAlignmentAlterationEffect()
            {
                AlignmentType = template.AlignmentType,
                RogueName = template.Name,
                IncludeCursedEquipment = template.IncludeCursedEquipment
            };
        }

        protected DrainMeleeAlterationEffect GenerateAlterationEffect(DrainMeleeAlterationEffectTemplate template)
        {
            return new DrainMeleeAlterationEffect()
            {
                Health = _randomSequenceGenerator.GetRandomValue(template.Health),
                Stamina = _randomSequenceGenerator.GetRandomValue(template.Stamina)
            };
        }

        protected EquipmentDamageAlterationEffect GenerateAlterationEffect(EquipmentDamageAlterationEffectTemplate template)
        {
            return new EquipmentDamageAlterationEffect()
            {
                AttackAttributes = template.AttackAttributes.Select(x => _attackAttributeGenerator.GenerateAttackAttribute(x)).ToList(),
                ClassChange = template.ClassChange,
                Type = template.Type,
                QualityChange = template.QualityChange,
                RogueName = template.Name
            };
        }

        protected EquipmentEnhanceAlterationEffect GenerateAlterationEffect(EquipmentEnhanceAlterationEffectTemplate template)
        {
            return new EquipmentEnhanceAlterationEffect()
            {
                AttackAttributes = template.AttackAttributes.Select(x => _attackAttributeGenerator.GenerateAttackAttribute(x)).ToList(),
                UseDialog = template.UseDialog,
                ClassChange = template.ClassChange,
                Type = template.Type,
                QualityChange = template.QualityChange,
                RogueName = template.Name
            };
        }

        protected IdentifyAlterationEffect GenerateAlterationEffect(IdentifyAlterationEffectTemplate template)
        {
            return new IdentifyAlterationEffect()
            {
                IdentifyAll = template.IdentifyAll,
                RogueName = template.Name
            };
        }

        protected UncurseAlterationEffect GenerateAlterationEffect(UncurseAlterationEffectTemplate template)
        {
            return new UncurseAlterationEffect()
            {
                UncurseAll = template.UncurseAll,
                RogueName = template.Name
            };
        }

        protected PassiveAlterationEffect GenerateAlterationEffect(PassiveAlterationEffectTemplate template)
        {
            return new PassiveAlterationEffect()
            {
                Agility = _randomSequenceGenerator.GetRandomValue(template.AgilityRange),
                Attack = _randomSequenceGenerator.GetRandomValue(template.AttackRange),
                CanSeeInvisibleCharacters = template.CanSeeInvisibleCharacters,
                Defense = _randomSequenceGenerator.GetRandomValue(template.DefenseRange),
                FoodUsagePerTurn = _randomSequenceGenerator.GetRandomValue(template.FoodUsagePerTurnRange),
                HealthPerStep = _randomSequenceGenerator.GetRandomValue(template.HealthPerStepRange),
                Intelligence = _randomSequenceGenerator.GetRandomValue(template.IntelligenceRange),
                Vision = _randomSequenceGenerator.GetRandomValue(template.VisionRange),
                StaminaPerStep = _randomSequenceGenerator.GetRandomValue(template.StaminaPerStepRange),
                Speed = _randomSequenceGenerator.GetRandomValue(template.SpeedRange),
                Strength = _randomSequenceGenerator.GetRandomValue(template.StrengthRange),
                SymbolAlteration = template.SymbolAlteration,
                RogueName = template.Name
            };
        }

        protected PermanentAlterationEffect GenerateAlterationEffect(PermanentAlterationEffectTemplate template)
        {
            return new PermanentAlterationEffect()
            {
                Agility = _randomSequenceGenerator.GetRandomValue(template.AgilityRange),
                Experience = _randomSequenceGenerator.GetRandomValue(template.ExperienceRange),
                Health = _randomSequenceGenerator.GetRandomValue(template.HealthRange),
                Hunger = _randomSequenceGenerator.GetRandomValue(template.HungerRange),
                Intelligence = _randomSequenceGenerator.GetRandomValue(template.IntelligenceRange),
                Vision = _randomSequenceGenerator.GetRandomValue(template.VisionRange),
                Stamina = _randomSequenceGenerator.GetRandomValue(template.StaminaRange),
                Speed = _randomSequenceGenerator.GetRandomValue(template.SpeedRange),
                Strength = _randomSequenceGenerator.GetRandomValue(template.StrengthRange),
                RogueName = template.Name
            };
        }

        protected RemedyAlterationEffect GenerateAlterationEffect(RemedyAlterationEffectTemplate template)
        {
            return new RemedyAlterationEffect()
            {
                RemediedState = _alteredStateGenerator.GenerateAlteredState(template.RemediedState),
                RogueName = template.Name
            };
        }

        protected RevealAlterationEffect GenerateAlterationEffect(RevealAlterationEffectTemplate template)
        {
            return new RevealAlterationEffect()
            {
                Type = template.Type,
                RogueName = template.Name
            };
        }

        protected RunAwayAlterationEffect GenerateAlterationEffect(RunAwayAlterationEffectTemplate template)
        {
            return new RunAwayAlterationEffect()
            {
                RogueName = template.Name
            };
        }

        protected StealAlterationEffect GenerateAlterationEffect(StealAlterationEffectTemplate template)
        {
            return new StealAlterationEffect()
            {
                RogueName = template.Name
            };
        }

        protected TeleportRandomAlterationEffect GenerateAlterationEffect(TeleportRandomAlterationEffectTemplate template)
        {
            return new TeleportRandomAlterationEffect()
            {
                Range = template.Range,
                TeleportType = template.TeleportType,
                RogueName = template.Name
            };
        }

        protected TemporaryAlterationEffect GenerateAlterationEffect(TemporaryAlterationEffectTemplate template)
        {
            return new TemporaryAlterationEffect()
            {
                AlteredState = _alteredStateGenerator.GenerateAlteredState(template.AlteredState),
                Agility = _randomSequenceGenerator.GetRandomValue(template.AgilityRange),
                Attack = _randomSequenceGenerator.GetRandomValue(template.AttackRange),
                CanSeeInvisibleCharacters = template.CanSeeInvisibleCharacters,
                Defense = _randomSequenceGenerator.GetRandomValue(template.DefenseRange),
                EventTime = _randomSequenceGenerator.GetRandomValue(template.EventTime),
                FoodUsagePerTurn = _randomSequenceGenerator.GetRandomValue(template.FoodUsagePerTurnRange),
                HealthPerStep = _randomSequenceGenerator.GetRandomValue(template.HealthPerStepRange),
                Intelligence = _randomSequenceGenerator.GetRandomValue(template.IntelligenceRange),
                IsStackable = template.IsStackable,
                Vision = _randomSequenceGenerator.GetRandomValue(template.VisionRange),
                StaminaPerStep = _randomSequenceGenerator.GetRandomValue(template.StaminaPerStepRange),
                Speed = _randomSequenceGenerator.GetRandomValue(template.SpeedRange),
                Strength = _randomSequenceGenerator.GetRandomValue(template.StrengthRange),
                SymbolAlteration = template.SymbolAlteration,
                RogueName = template.Name,
                HasAlteredState = template.HasAlteredState
            };
        }

        protected TransmuteAlterationEffect GenerateAlterationEffect(TransmuteAlterationEffectTemplate template)
        {
            return new TransmuteAlterationEffect()
            {
                ProbabilityOfSuccess = template.ProbabilityOfSuccess,
                RogueName = template.Name,
                TransmuteItems = template.TransmuteItems.Select(x =>
                {
                    return new TransmuteAlterationEffectItem()
                    {
                        ConsumableProduct = x.IsConsumableProduct ? _itemGenerator.GenerateConsumable(x.ConsumableProduct) : null,
                        ConsumableRequirements = x.ConsumableRequirements.Select(z => _itemGenerator.GenerateConsumable(z)).ToList(),
                        EquipmentProduct = x.IsEquipmentProduct ? _itemGenerator.GenerateEquipment(x.EquipmentProduct) : null,
                        EquipmentRequirements = x.EquipmentRequirements.Select(z => _itemGenerator.GenerateEquipment(z)).ToList(),
                        IsConsumableProduct = x.IsConsumableProduct,
                        IsEquipmentProduct = x.IsEquipmentProduct,
                        RogueName = x.Name,
                        Weighting = x.Weighting
                    };
                }).ToList()
            };
        }
        #endregion

        private AlterationCategory CreateAlterationCategory(AlterationCategoryTemplate template)
        {
            var result = new AlterationCategory()
            {
                AlignmentType = template.AlignmentType,
                RogueName = template.Name
            };

            _symbolDetailsGenerator.MapSymbolDetails(template.SymbolDetails, result);

            return result;
        }
    }
}
