﻿using Rogue.NET.Core.Model.Scenario.Alteration.Common;
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
using Rogue.NET.Core.Processing.Model.Generator.Interface;
using System;
using System.ComponentModel.Composition;
using System.Linq;

namespace Rogue.NET.Core.Processing.Model.Generator
{
    [Export(typeof(IAlterationGenerator))]
    public class AlterationGenerator : IAlterationGenerator
    {
        private readonly IRandomSequenceGenerator _randomSequenceGenerator;
        private readonly IAttackAttributeGenerator _attackAttributeGenerator;
        private readonly IAlteredStateGenerator _alteredStateGenerator;
        private readonly IAnimationGenerator _animationGenerator;
        private readonly IItemGenerator _itemGenerator;

        [ImportingConstructor]
        public AlterationGenerator(
            IRandomSequenceGenerator randomSequenceGenerator, 
            IAttackAttributeGenerator attackAttributeGenerator,
            IAlteredStateGenerator alteredStateGenerator,
            IAnimationGenerator animationGenerator,
            IItemGenerator itemGenerator)
        {
            _randomSequenceGenerator = randomSequenceGenerator;
            _attackAttributeGenerator = attackAttributeGenerator;
            _alteredStateGenerator = alteredStateGenerator;
            _animationGenerator = animationGenerator;
            _itemGenerator = itemGenerator;
        }

        public AlterationCost GenerateAlterationCost(AlterationCostTemplate template)
        {
            return new AlterationCost()
            {
                Agility = template.Agility,
                Speed = template.Speed,
                LightRadius = template.AuraRadius,
                Experience = template.Experience,
                FoodUsagePerTurn = template.FoodUsagePerTurn,
                Hp = template.Hp,
                Hunger = template.Hunger,
                Intelligence = template.Intelligence,
                Mp = template.Mp,
                Strength = template.Strength
            };
        }

        public ConsumableAlteration GenerateAlteration(ConsumableAlterationTemplate template)
        {
            return new ConsumableAlteration()
            {
                AnimationGroup = _animationGenerator.GenerateAnimationGroup(template.AnimationGroup),
                Cost = GenerateAlterationCost(template.Cost),
                Effect = GenerateAlterationEffect(template.Effect),
                RogueName = template.Name
            };
        }

        public ConsumableProjectileAlteration GenerateAlteration(ConsumableProjectileAlterationTemplate template)
        {
            return new ConsumableProjectileAlteration()
            {
                AnimationGroup = _animationGenerator.GenerateAnimationGroup(template.AnimationGroup),
                Effect = GenerateAlterationEffect(template.Effect),
                RogueName = template.Name
            };
        }

        public DoodadAlteration GenerateAlteration(DoodadAlterationTemplate template)
        {
            return new DoodadAlteration()
            {
                AnimationGroup = _animationGenerator.GenerateAnimationGroup(template.AnimationGroup),
                Effect = GenerateAlterationEffect(template.Effect),
                RogueName = template.Name
            };
        }

        public EnemyAlteration GenerateAlteration(EnemyAlterationTemplate template)
        {
            return new EnemyAlteration()
            {
                AnimationGroup = _animationGenerator.GenerateAnimationGroup(template.AnimationGroup),
                Cost = GenerateAlterationCost(template.Cost),
                Effect = GenerateAlterationEffect(template.Effect),
                RogueName = template.Name
            };
        }

        public EquipmentAttackAlteration GenerateAlteration(EquipmentAttackAlterationTemplate template)
        {
            return new EquipmentAttackAlteration()
            {
                AnimationGroup = _animationGenerator.GenerateAnimationGroup(template.AnimationGroup),
                Cost = GenerateAlterationCost(template.Cost),
                Effect = GenerateAlterationEffect(template.Effect),
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
                Effect = GenerateAlterationEffect(template.Effect),
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
                Effect = GenerateAlterationEffect(template.Effect),
                RogueName = template.Name
            };
        }

        public SkillAlteration GenerateAlteration(SkillAlterationTemplate template)
        {
            return new SkillAlteration()
            {
                AnimationGroup = _animationGenerator.GenerateAnimationGroup(template.AnimationGroup),
                AuraParameters = new AuraSourceParameters()
                {
                    AuraColor = template.AuraParameters.AuraColor,
                    AuraRange = template.AuraParameters.AuraRange
                },
                Cost = GenerateAlterationCost(template.Cost),
                Effect = GenerateAlterationEffect(template.Effect),
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

            else if (template is CreateMonsterAlterationEffectTemplate)
                return GenerateAlterationEffect(template as CreateMonsterAlterationEffectTemplate);

            else if (template is EquipmentDamageAlterationEffectTemplate)
                return GenerateAlterationEffect(template as EquipmentDamageAlterationEffectTemplate);

            else if (template is EquipmentEnhanceAlterationEffectTemplate)
                return GenerateAlterationEffect(template as EquipmentEnhanceAlterationEffectTemplate);

            else if (template is OtherAlterationEffectTemplate)
                return GenerateAlterationEffect(template as OtherAlterationEffectTemplate);

            else if (template is PermanentAlterationEffectTemplate)
                return GenerateAlterationEffect(template as PermanentAlterationEffectTemplate);

            else if (template is RemedyAlterationEffectTemplate)
                return GenerateAlterationEffect(template as RemedyAlterationEffectTemplate);

            else if (template is RevealAlterationEffectTemplate)
                return GenerateAlterationEffect(template as RevealAlterationEffectTemplate);

            else if (template is TeleportAlterationEffectTemplate)
                return GenerateAlterationEffect(template as TeleportAlterationEffectTemplate);

            else if (template is TemporaryAlterationEffectTemplate)
                return GenerateAlterationEffect(template as TemporaryAlterationEffectTemplate);

            else if (template is TransmuteAlterationEffectTemplate)
                return GenerateAlterationEffect(template as TransmuteAlterationEffectTemplate);

            else
                throw new Exception("Unhandled IConsumableAlterationEffectTemplate Type");
        }

        protected IConsumableProjectileAlterationEffect GenerateAlterationEffect(IConsumableProjectileAlterationEffectTemplate template)
        {
            if (template is AttackAttributeMeleeAlterationEffectTemplate)
                return GenerateAlterationEffect(template as AttackAttributeMeleeAlterationEffectTemplate);

            else if (template is AttackAttributeTemporaryAlterationEffectTemplate)
                return GenerateAlterationEffect(template as AttackAttributeTemporaryAlterationEffectTemplate);

            else if (template is EquipmentDamageAlterationEffectTemplate)
                return GenerateAlterationEffect(template as EquipmentDamageAlterationEffectTemplate);

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

            else if (template is CreateMonsterAlterationEffectTemplate)
                return GenerateAlterationEffect(template as CreateMonsterAlterationEffectTemplate);

            else if (template is EquipmentDamageAlterationEffectTemplate)
                return GenerateAlterationEffect(template as EquipmentDamageAlterationEffectTemplate);

            else if (template is EquipmentEnhanceAlterationEffectTemplate)
                return GenerateAlterationEffect(template as EquipmentEnhanceAlterationEffectTemplate);

            else if (template is OtherAlterationEffectTemplate)
                return GenerateAlterationEffect(template as OtherAlterationEffectTemplate);

            else if (template is PermanentAlterationEffectTemplate)
                return GenerateAlterationEffect(template as PermanentAlterationEffectTemplate);

            else if (template is RemedyAlterationEffectTemplate)
                return GenerateAlterationEffect(template as RemedyAlterationEffectTemplate);

            else if (template is RevealAlterationEffectTemplate)
                return GenerateAlterationEffect(template as RevealAlterationEffectTemplate);

            else if (template is TeleportAlterationEffectTemplate)
                return GenerateAlterationEffect(template as TeleportAlterationEffectTemplate);

            else if (template is TemporaryAlterationEffectTemplate)
                return GenerateAlterationEffect(template as TemporaryAlterationEffectTemplate);

            else if (template is TransmuteAlterationEffectTemplate)
                return GenerateAlterationEffect(template as TransmuteAlterationEffectTemplate);

            else
                throw new Exception("Unhandled IDoodadAlterationEffectTemplate Type");
        }

        protected IEnemyAlterationEffect GenerateAlterationEffect(IEnemyAlterationEffectTemplate template)
        {
            if (template is AttackAttributeMeleeAlterationEffectTemplate)
                return GenerateAlterationEffect(template as AttackAttributeMeleeAlterationEffectTemplate);

            else if (template is AttackAttributeTemporaryAlterationEffectTemplate)
                return GenerateAlterationEffect(template as AttackAttributeTemporaryAlterationEffectTemplate);

            else if (template is CreateMonsterAlterationEffectTemplate)
                return GenerateAlterationEffect(template as CreateMonsterAlterationEffectTemplate);

            else if (template is EquipmentDamageAlterationEffectTemplate)
                return GenerateAlterationEffect(template as EquipmentDamageAlterationEffectTemplate);

            else if (template is PermanentAlterationEffectTemplate)
                return GenerateAlterationEffect(template as PermanentAlterationEffectTemplate);

            else if (template is RunAwayAlterationEffectTemplate)
                return GenerateAlterationEffect(template as RunAwayAlterationEffectTemplate);

            else if (template is StealAlterationEffectTemplate)
                return GenerateAlterationEffect(template as StealAlterationEffectTemplate);

            else if (template is TeleportAlterationEffectTemplate)
                return GenerateAlterationEffect(template as TeleportAlterationEffectTemplate);

            else if (template is TemporaryAlterationEffectTemplate)
                return GenerateAlterationEffect(template as TemporaryAlterationEffectTemplate);

            else
                throw new Exception("Unhandled IEnemyAlterationEffectTemplate Type");
        }

        protected IEquipmentAttackAlterationEffect GenerateAlterationEffect(IEquipmentAttackAlterationEffectTemplate template)
        {
            if (template is AttackAttributeMeleeAlterationEffectTemplate)
                return GenerateAlterationEffect(template as AttackAttributeMeleeAlterationEffectTemplate);

            else if (template is DrainMeleeAlterationEffectTemplate)
                return GenerateAlterationEffect(template as DrainMeleeAlterationEffectTemplate);

            else if (template is EquipmentDamageAlterationEffectTemplate)
                return GenerateAlterationEffect(template as EquipmentDamageAlterationEffectTemplate);

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

            else if (template is PassiveAlterationEffectTemplate)
                return GenerateAlterationEffect(template as PassiveAlterationEffectTemplate);

            else
                throw new Exception("Unhandled IEquipmentEquipAlterationEffectTemplate Type");
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

            else if (template is ChangeLevelAlterationEffectTemplate)
                return GenerateAlterationEffect(template as ChangeLevelAlterationEffectTemplate);

            else if (template is CreateMonsterAlterationEffectTemplate)
                return GenerateAlterationEffect(template as CreateMonsterAlterationEffectTemplate);

            else if (template is EquipmentEnhanceAlterationEffectTemplate)
                return GenerateAlterationEffect(template as EquipmentEnhanceAlterationEffectTemplate);

            else if (template is EquipmentDamageAlterationEffectTemplate)
                return GenerateAlterationEffect(template as EquipmentDamageAlterationEffectTemplate);

            else if (template is OtherAlterationEffectTemplate)
                return GenerateAlterationEffect(template as OtherAlterationEffectTemplate);

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

            else if (template is TeleportAlterationEffectTemplate)
                return GenerateAlterationEffect(template as TeleportAlterationEffectTemplate);

            else if (template is TemporaryAlterationEffectTemplate)
                return GenerateAlterationEffect(template as TemporaryAlterationEffectTemplate);

            else if (template is TransmuteAlterationEffectTemplate)
                return GenerateAlterationEffect(template as TransmuteAlterationEffectTemplate);

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
                DodgeProbability = _randomSequenceGenerator.GetRandomValue(template.DodgeProbabilityRange),
                HpPerStep = _randomSequenceGenerator.GetRandomValue(template.HpPerStepRange),
                Intelligence = _randomSequenceGenerator.GetRandomValue(template.IntelligenceRange),
                MagicBlockProbability = _randomSequenceGenerator.GetRandomValue(template.MagicBlockProbabilityRange),
                MpPerStep = _randomSequenceGenerator.GetRandomValue(template.MpPerStepRange),
                RogueName = template.Name,
                Speed = _randomSequenceGenerator.GetRandomValue(template.SpeedRange),
                Strength = _randomSequenceGenerator.GetRandomValue(template.StrengthRange),
                SymbolAlteration = template.SymbolAlteration
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

        protected CreateMonsterAlterationEffect GenerateAlterationEffect(CreateMonsterAlterationEffectTemplate template)
        {
            return new CreateMonsterAlterationEffect()
            {
                CreateMonsterEnemy = template.CreateMonsterEnemy,
                RandomPlacementType = template.RandomPlacementType,
                Range = template.Range,
                RogueName = template.Name
            };
        }

        protected DrainMeleeAlterationEffect GenerateAlterationEffect(DrainMeleeAlterationEffectTemplate template)
        {
            return new DrainMeleeAlterationEffect()
            {
                Hp = _randomSequenceGenerator.GetRandomValue(template.Hp),
                Mp = _randomSequenceGenerator.GetRandomValue(template.Mp)
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

        protected OtherAlterationEffect GenerateAlterationEffect(OtherAlterationEffectTemplate template)
        {
            return new OtherAlterationEffect()
            {
                Type = template.Type,
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
                CriticalHit = _randomSequenceGenerator.GetRandomValue(template.CriticalHit),
                Defense = _randomSequenceGenerator.GetRandomValue(template.DefenseRange),
                DodgeProbability = _randomSequenceGenerator.GetRandomValue(template.DodgeProbabilityRange),
                FoodUsagePerTurn = _randomSequenceGenerator.GetRandomValue(template.FoodUsagePerTurnRange),
                HpPerStep = _randomSequenceGenerator.GetRandomValue(template.HpPerStepRange),
                Intelligence = _randomSequenceGenerator.GetRandomValue(template.IntelligenceRange),
                LightRadius = _randomSequenceGenerator.GetRandomValue(template.LightRadiusRange),
                MagicBlockProbability = _randomSequenceGenerator.GetRandomValue(template.MagicBlockProbabilityRange),
                MpPerStep = _randomSequenceGenerator.GetRandomValue(template.MpPerStepRange),
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
                Hp = _randomSequenceGenerator.GetRandomValue(template.HpRange),
                Hunger = _randomSequenceGenerator.GetRandomValue(template.HungerRange),
                Intelligence = _randomSequenceGenerator.GetRandomValue(template.IntelligenceRange),
                LightRadius = _randomSequenceGenerator.GetRandomValue(template.LightRadiusRange),
                Mp = _randomSequenceGenerator.GetRandomValue(template.MpRange),
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

        protected TeleportAlterationEffect GenerateAlterationEffect(TeleportAlterationEffectTemplate template)
        {
            return new TeleportAlterationEffect()
            {
                Range = template.Range,
                TeleportType = template.TeleportType,
                LocationSelectionType = template.LocationSelectionType,
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
                CriticalHit = _randomSequenceGenerator.GetRandomValue(template.CriticalHit),
                Defense = _randomSequenceGenerator.GetRandomValue(template.DefenseRange),
                DodgeProbability = _randomSequenceGenerator.GetRandomValue(template.DodgeProbabilityRange),
                EventTime = _randomSequenceGenerator.GetRandomValue(template.EventTime),
                FoodUsagePerTurn = _randomSequenceGenerator.GetRandomValue(template.FoodUsagePerTurnRange),
                HpPerStep = _randomSequenceGenerator.GetRandomValue(template.HpPerStepRange),
                Intelligence = _randomSequenceGenerator.GetRandomValue(template.IntelligenceRange),
                IsStackable = template.IsStackable,
                LightRadius = _randomSequenceGenerator.GetRandomValue(template.LightRadiusRange),
                MentalBlockProbability = _randomSequenceGenerator.GetRandomValue(template.MentalBlockProbabilityRange),
                MpPerStep = _randomSequenceGenerator.GetRandomValue(template.MpPerStepRange),
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
    }
}