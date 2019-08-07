using Rogue.NET.Core.Model.Generator.Interface;
using Rogue.NET.Core.Model.Scenario.Alteration;
using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.Core.Model.Scenario.Alteration.Consumable;
using Rogue.NET.Core.Model.Scenario.Alteration.Doodad;
using Rogue.NET.Core.Model.Scenario.Alteration.Effect;
using Rogue.NET.Core.Model.Scenario.Alteration.Enemy;
using Rogue.NET.Core.Model.Scenario.Alteration.Equipment;
using Rogue.NET.Core.Model.Scenario.Alteration.Interface;
using Rogue.NET.Core.Model.Scenario.Alteration.Skill;
using Rogue.NET.Core.Model.Scenario.Content.Skill;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Common;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Consumable;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Doodad;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Enemy;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Equipment;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Interface;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Skill;
using System;
using System.ComponentModel.Composition;
using System.Linq;

namespace Rogue.NET.Core.Model.Generator
{
    [Export(typeof(IAlterationGenerator))]
    public class AlterationGenerator : IAlterationGenerator
    {
        private readonly IRandomSequenceGenerator _randomSequenceGenerator;
        private readonly IAttackAttributeGenerator _attackAttributeGenerator;
        private readonly IAlteredStateGenerator _alteredStateGenerator;
        private readonly IAnimationGenerator _animationGenerator;

        [ImportingConstructor]
        public AlterationGenerator(
            IRandomSequenceGenerator randomSequenceGenerator, 
            IAttackAttributeGenerator attackAttributeGenerator,
            IAlteredStateGenerator alteredStateGenerator,
            IAnimationGenerator animationGenerator)
        {
            _randomSequenceGenerator = randomSequenceGenerator;
            _attackAttributeGenerator = attackAttributeGenerator;
            _alteredStateGenerator = alteredStateGenerator;
            _animationGenerator = animationGenerator;
        }

        public AlterationContainer GenerateAlteration(Spell spell)
        {
            AlterationCost alterationCost = GenerateAlterationCost(spell.Cost);
            AlterationEffect alterationEffect = GenerateAlterationEffect(spell.RogueName, spell.DisplayName, spell.EffectRange, spell.Effect);
            AlterationEffect auraEffect = GenerateAlterationEffect(spell.RogueName, spell.DisplayName, spell.EffectRange, spell.AuraEffect);

            AlterationContainer alterationContainer = new AlterationContainer();
            alterationContainer.Cost = alterationCost;
            alterationContainer.Effect = alterationEffect;
            alterationContainer.AuraEffect = auraEffect;
            alterationContainer.RogueName = spell.RogueName;
            alterationContainer.EffectRange = spell.EffectRange;
            alterationContainer.OtherEffectType = spell.OtherEffectType;
            alterationContainer.AttackAttributeType = spell.AttackAttributeType;
            alterationContainer.Type = spell.Type;
            alterationContainer.BlockType = spell.BlockType;
            alterationContainer.GeneratingSpellId = spell.Id;
            alterationContainer.GeneratingSpellName = spell.RogueName;
            alterationContainer.IsStackable = spell.IsStackable;
            alterationContainer.CreateMonsterEnemy = spell.CreateMonsterEnemyName;

            return alterationContainer;
        }
        public AlterationEffect GenerateAlterationEffect(
                string spellName, 
                string spellDisplayName, 
                double effectRange, 
                AlterationEffectTemplate alterationEffectTemplate)
        {
            AlterationEffect alterationEffect = new AlterationEffect();

            // Scaled Parameters
            alterationEffect.Agility = _randomSequenceGenerator.GetRandomValue(alterationEffectTemplate.AgilityRange);
            alterationEffect.Attack = _randomSequenceGenerator.GetRandomValue(alterationEffectTemplate.AttackRange);
            alterationEffect.AuraRadius = _randomSequenceGenerator.GetRandomValue(alterationEffectTemplate.AuraRadiusRange);
            alterationEffect.Defense = _randomSequenceGenerator.GetRandomValue(alterationEffectTemplate.DefenseRange);
            alterationEffect.DodgeProbability = _randomSequenceGenerator.GetRandomValue(alterationEffectTemplate.DodgeProbabilityRange);
            alterationEffect.Experience = _randomSequenceGenerator.GetRandomValue(alterationEffectTemplate.ExperienceRange);
            alterationEffect.FoodUsagePerTurn = _randomSequenceGenerator.GetRandomValue(alterationEffectTemplate.FoodUsagePerTurnRange);
            alterationEffect.Hp = _randomSequenceGenerator.GetRandomValue(alterationEffectTemplate.HpRange);
            alterationEffect.HpPerStep = _randomSequenceGenerator.GetRandomValue(alterationEffectTemplate.HpPerStepRange);
            alterationEffect.Hunger = _randomSequenceGenerator.GetRandomValue(alterationEffectTemplate.HungerRange);
            alterationEffect.Intelligence = _randomSequenceGenerator.GetRandomValue(alterationEffectTemplate.IntelligenceRange);
            alterationEffect.MagicBlockProbability = _randomSequenceGenerator.GetRandomValue(alterationEffectTemplate.MagicBlockProbabilityRange);
            alterationEffect.Mp = _randomSequenceGenerator.GetRandomValue(alterationEffectTemplate.MpRange);
            alterationEffect.MpPerStep = _randomSequenceGenerator.GetRandomValue(alterationEffectTemplate.MpPerStepRange);
            alterationEffect.Speed = _randomSequenceGenerator.GetRandomValue(alterationEffectTemplate.SpeedRange);
            alterationEffect.Strength = _randomSequenceGenerator.GetRandomValue(alterationEffectTemplate.StrengthRange);
            alterationEffect.CriticalHit = _randomSequenceGenerator.GetRandomValue(alterationEffectTemplate.CriticalHit);

            // Scaled - Attack Attributes
            alterationEffect.AttackAttributes = alterationEffectTemplate.AttackAttributes
                                                      .Select(x => _attackAttributeGenerator.GenerateAttackAttribute(x))
                                                      .Select(x =>
                                                      {
                                                          x.Attack = x.Attack;
                                                          x.Resistance = x.Resistance;
                                                          x.Weakness = x.Weakness;

                                                          return x;
                                                      })
                                                      .ToList();

            // Non-Scaled Parameters
            alterationEffect.EventTime = _randomSequenceGenerator.GetRandomValue(alterationEffectTemplate.EventTime);

            // Non-Scaled - Copied to aura effect so that it can detach from the spell
            alterationEffect.EffectRange = effectRange;

            alterationEffect.IsSymbolAlteration = alterationEffectTemplate.IsSymbolAlteration;
            alterationEffect.RogueName = alterationEffectTemplate.Name;
            alterationEffect.State = _alteredStateGenerator.GenerateAlteredState(alterationEffectTemplate.AlteredState);
            alterationEffect.SymbolAlteration = alterationEffectTemplate.SymbolAlteration;
            alterationEffect.RogueName = spellName;
            alterationEffect.DisplayName = spellDisplayName;
            alterationEffect.CanSeeInvisibleCharacters = alterationEffectTemplate.CanSeeInvisibleCharacters;

            //Store remedied state name
            alterationEffect.RemediedStateName = alterationEffectTemplate.RemediedState.Name;

            return alterationEffect;
        }

        protected AlterationCost GenerateAlterationCost(AlterationCostTemplate template)
        {
            return new AlterationCost()
            {
                Type = template.Type,
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

        #region (public) Alteration Container Generators
        public ConsumableAlteration GenerateAlteration(ConsumableAlterationTemplate template)
        {
            return new ConsumableAlteration(template.Guid)
            {
                AnimationGroup = _animationGenerator.GenerateAnimationGroup(template.AnimationGroup),
                Cost = GenerateAlterationCost(template.Cost),
                Effect = GenerateAlterationEffect(template.Effect),
                RogueName = template.Name,
                TargetType = template.TargetType
            };
        }

        public ConsumableProjectileAlteration GenerateAlteration(ConsumableProjectileAlterationTemplate template)
        {
            return new ConsumableProjectileAlteration(template.Guid)
            {
                AnimationGroup = _animationGenerator.GenerateAnimationGroup(template.AnimationGroup),
                Effect = GenerateAlterationEffect(template.Effect),
                RogueName = template.Name
            };
        }

        public DoodadAlteration GenerateAlteration(DoodadAlterationTemplate template)
        {
            return new DoodadAlteration(template.Guid)
            {
                AnimationGroup = _animationGenerator.GenerateAnimationGroup(template.AnimationGroup),
                Effect = GenerateAlterationEffect(template.Effect),
                RogueName = template.Name,
                TargetType = template.TargetType
            };
        }

        public EnemyAlteration GenerateAlteration(EnemyAlterationTemplate template)
        {
            return new EnemyAlteration(template.Guid)
            {
                AnimationGroup = _animationGenerator.GenerateAnimationGroup(template.AnimationGroup),
                Cost = GenerateAlterationCost(template.Cost),
                Effect = GenerateAlterationEffect(template.Effect),
                RogueName = template.Name,
                TargetType = template.TargetType                
            };
        }

        public EquipmentAttackAlteration GenerateAlteration(EquipmentAttackAlterationTemplate template)
        {
            return new EquipmentAttackAlteration(template.Guid)
            {
                AnimationGroup = _animationGenerator.GenerateAnimationGroup(template.AnimationGroup),
                Cost = GenerateAlterationCost(template.Cost),
                Effect = GenerateAlterationEffect(template.Effect),
                RogueName = template.Name                
            };
        }

        public EquipmentCurseAlteration GenerateAlteration(EquipmentCurseAlterationTemplate template)
        {
            return new EquipmentCurseAlteration(template.Guid)
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
            return new EquipmentEquipAlteration(template.Guid)
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
            return new SkillAlteration(template.Guid)
            {
                AnimationGroup = _animationGenerator.GenerateAnimationGroup(template.AnimationGroup),
                AuraParameters = new AuraSourceParameters()
                {
                    AuraColor = template.AuraParameters.AuraColor,
                    AuraRange = template.AuraParameters.AuraRange
                },
                Cost = GenerateAlterationCost(template.Cost),
                Effect = GenerateAlterationEffect(template.Effect),
                RogueName = template.Name,
                TargetType = template.TargetType
            };
        }
        #endregion

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

            else if (template is EquipmentModifyAlterationEffectTemplate)
                return GenerateAlterationEffect(template as EquipmentModifyAlterationEffectTemplate);

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

            else if (template is CreateMonsterAlterationEffectTemplate)
                return GenerateAlterationEffect(template as CreateMonsterAlterationEffectTemplate);

            else if (template is EquipmentModifyAlterationEffectTemplate)
                return GenerateAlterationEffect(template as EquipmentModifyAlterationEffectTemplate);

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

            else if (template is EquipmentModifyAlterationEffectTemplate)
                return GenerateAlterationEffect(template as EquipmentModifyAlterationEffectTemplate);

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

            else if (template is EquipmentModifyAlterationEffectTemplate)
                return GenerateAlterationEffect(template as EquipmentModifyAlterationEffectTemplate);

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
                CombatType = template.CombatType,
                RogueName = template.Name,
                TargetType = template.TargetType
            };
        }

        protected AttackAttributePassiveAlterationEffect GenerateAlterationEffect(AttackAttributePassiveAlterationEffectTemplate template)
        {
            return new AttackAttributePassiveAlterationEffect()
            {
                AttackAttributes = template.AttackAttributes.Select(x => _attackAttributeGenerator.GenerateAttackAttribute(x)).ToList(),
                CombatType = template.CombatType,
                RogueName = template.Name,
                TargetType = template.TargetType
            };
        }

        protected AttackAttributeTemporaryAlterationEffect GenerateAlterationEffect(AttackAttributeTemporaryAlterationEffectTemplate template)
        {
            return new AttackAttributeTemporaryAlterationEffect()
            {
                AlteredState = _alteredStateGenerator.GenerateAlteredState(template.AlteredState),
                AttackAttributes = template.AttackAttributes.Select(x => _attackAttributeGenerator.GenerateAttackAttribute(x)).ToList(),
                CombatType = template.CombatType,
                EventTime = template.EventTime,
                IsStackable = template.IsStackable,
                RogueName = template.Name,
                TargetType = template.TargetType
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

        protected EquipmentModifyAlterationEffect GenerateAlterationEffect(EquipmentModifyAlterationEffectTemplate template)
        {
            return new EquipmentModifyAlterationEffect()
            {
                AttackAttributes = template.AttackAttributes.Select(x => _attackAttributeGenerator.GenerateAttackAttribute(x)).ToList(),
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
                Experience = _randomSequenceGenerator.GetRandomValue(template.ExperienceRange),
                FoodUsagePerTurn = _randomSequenceGenerator.GetRandomValue(template.FoodUsagePerTurnRange),
                Hp = _randomSequenceGenerator.GetRandomValue(template.HpRange),
                HpPerStep = _randomSequenceGenerator.GetRandomValue(template.HpPerStepRange),
                Hunger = _randomSequenceGenerator.GetRandomValue(template.HungerRange),
                Intelligence = _randomSequenceGenerator.GetRandomValue(template.IntelligenceRange),
                LightRadius = _randomSequenceGenerator.GetRandomValue(template.LightRadiusRange),
                MagicBlockProbability = _randomSequenceGenerator.GetRandomValue(template.MagicBlockProbabilityRange),
                Mp = _randomSequenceGenerator.GetRandomValue(template.MpRange),
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
                Experience = _randomSequenceGenerator.GetRandomValue(template.ExperienceRange),
                FoodUsagePerTurn = _randomSequenceGenerator.GetRandomValue(template.FoodUsagePerTurnRange),
                Hp = _randomSequenceGenerator.GetRandomValue(template.HpRange),
                HpPerStep = _randomSequenceGenerator.GetRandomValue(template.HpPerStepRange),
                Hunger = _randomSequenceGenerator.GetRandomValue(template.HungerRange),
                Intelligence = _randomSequenceGenerator.GetRandomValue(template.IntelligenceRange),
                IsStackable = template.IsStackable,
                LightRadius = _randomSequenceGenerator.GetRandomValue(template.LightRadiusRange),
                MagicBlockProbability = _randomSequenceGenerator.GetRandomValue(template.MagicBlockProbabilityRange),
                Mp = _randomSequenceGenerator.GetRandomValue(template.MpRange),
                MpPerStep = _randomSequenceGenerator.GetRandomValue(template.MpPerStepRange),
                Speed = _randomSequenceGenerator.GetRandomValue(template.SpeedRange),
                Strength = _randomSequenceGenerator.GetRandomValue(template.StrengthRange),                
                SymbolAlteration = template.SymbolAlteration,
                RogueName = template.Name
            };
        }
        #endregion
    }
}
