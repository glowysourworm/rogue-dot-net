﻿using Prism.Events;
using Rogue.NET.Common.ViewModel;
using Rogue.NET.Core.Event.Scenario.Level.Event;
using Rogue.NET.Core.Logic.Content.Interface;
using Rogue.NET.Core.Service.Interface;
using Rogue.NET.Core.Model.Scenario.Content;

using System.Linq;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Collections.Generic;
using System;
using Rogue.NET.Core.Logic.Processing.Interface;
using Rogue.NET.Core.Logic.Processing.Enum;
using Rogue.NET.Model.Events;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content.Item;
using Rogue.NET.Core.Model.Scenario.Character.Extension;
using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Model.Scenario.Alteration;
using Rogue.NET.Core.Model.Scenario.Alteration.Extension;
using Rogue.NET.Scenario.Content.ViewModel.Content.Alteration;
using Rogue.NET.Scenario.Content.ViewModel.Content.ScenarioMetaData;
using Rogue.NET.Core.Logic.Static;
using Rogue.NET.Core.Model.Scenario.Content.Skill.Extension;

namespace Rogue.NET.Scenario.Content.ViewModel.Content
{
    [Export]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class PlayerViewModel : ScenarioImageViewModel
    {
        readonly IModelService _modelService;
        readonly IPlayerProcessor _playerProcessor;
        readonly IAlterationProcessor _alterationProcessor;
        readonly IEventAggregator _eventAggregator;
        readonly IScenarioResourceService _scenarioResourceService;

        #region (private) Backing Fields
        int _level;
        int _skillPointsEarned;
        int _skillPoints;
        string _class;
        double _experience;
        double _experienceNext;
        double _skillPointExperience;
        double _skillPointExperienceNext;
        double _hunger;
        double _haul;
        double _haulMax;
        double _hpMax;
        double _mpMax;
        double _hp;
        double _mp;
        double _attack;
        double _attackBase;
        double _defense;
        double _defenseBase;
        double _dodge;
        double _dodgeBase;
        double _foodUsagePerTurn;
        double _foodUsagePerTurnBase;
        double _criticalHitProbability;
        double _hpRegen;
        double _hpRegenBase;
        double _mpRegen;
        double _mpRegenBase;
        double _strength;
        double _strengthBase;
        double _agility;
        double _agilityBase;
        double _intelligence;
        double _intelligenceBase;
        double _auraRadius;
        double _auraRadiusBase;
        double _magicBlock;
        double _magicBlockBase;
        double _speed;
        double _speedBase;
        SkillSetViewModel _activeSkillSet;
        EquipmentViewModel _equippedAmulet;
        EquipmentViewModel _equippedArmor;
        EquipmentViewModel _equippedBelt;
        EquipmentViewModel _equippedBoots;
        EquipmentViewModel _equippedGauntlets;
        EquipmentViewModel _equippedHelmet;
        EquipmentViewModel _equippedLeftHandWeapon;
        EquipmentViewModel _equippedLeftRing;
        EquipmentViewModel _equippedOrb;
        EquipmentViewModel _equippedRightHandWeapon;
        EquipmentViewModel _equippedRightRing;
        EquipmentViewModel _equippedShoulder;
        #endregion

        #region (public) Properties
        public int Level
        {
            get { return _level; }
            set { this.RaiseAndSetIfChanged(ref _level, value); }
        }
        public int SkillPoints
        {
            get { return _skillPoints; }
            set { this.RaiseAndSetIfChanged(ref _skillPoints, value); }
        }
        public string Class
        {
            get { return _class; }
            set { this.RaiseAndSetIfChanged(ref _class, value); }
        }
        public double Experience
        {
            get { return _experience; }
            set { this.RaiseAndSetIfChanged(ref _experience, value); }
        }
        public double ExperienceNext
        {
            get { return _experienceNext; }
            set { this.RaiseAndSetIfChanged(ref _experienceNext, value); }
        }
        public double Hunger
        {
            get { return _hunger; }
            set { this.RaiseAndSetIfChanged(ref _hunger, value); }
        }
        public double HpMax
        {
            get { return _hpMax; }
            set { this.RaiseAndSetIfChanged(ref _hpMax, value); }
        }
        public double MpMax
        {
            get { return _mpMax; }
            set { this.RaiseAndSetIfChanged(ref _mpMax, value); }
        }
        public double Hp
        {
            get { return _hp; }
            set { this.RaiseAndSetIfChanged(ref _hp, value); }
        }
        public double Mp
        {
            get { return _mp; }
            set { this.RaiseAndSetIfChanged(ref _mp, value); }
        }
        public double Haul
        {
            get { return _haul; }
            set { this.RaiseAndSetIfChanged(ref _haul, value); }
        }
        public double HaulMax
        {
            get { return _haulMax; }
            set { this.RaiseAndSetIfChanged(ref _haulMax, value); }
        }
        public double Attack
        {
            get { return _attack; }
            set { this.RaiseAndSetIfChanged(ref _attack, value); }
        }
        public double AttackBase
        {
            get { return _attackBase; }
            set { this.RaiseAndSetIfChanged(ref _attackBase, value); }
        }
        public double Defense
        {
            get { return _defense; }
            set { this.RaiseAndSetIfChanged(ref _defense, value); }
        }
        public double DefenseBase
        {
            get { return _defenseBase; }
            set { this.RaiseAndSetIfChanged(ref _defenseBase, value); }
        }
        public double Dodge
        {
            get { return _dodge; }
            set { this.RaiseAndSetIfChanged(ref _dodge, value); }
        }
        public double DodgeBase
        {
            get { return _dodgeBase; }
            set { this.RaiseAndSetIfChanged(ref _dodgeBase, value); }
        }
        public double FoodUsagePerTurn
        {
            get { return _foodUsagePerTurn; }
            set { this.RaiseAndSetIfChanged(ref _foodUsagePerTurn, value); }
        }
        public double FoodUsagePerTurnBase
        {
            get { return _foodUsagePerTurnBase; }
            set { this.RaiseAndSetIfChanged(ref _foodUsagePerTurnBase, value); }
        }
        public double CriticalHitProbability
        {
            get { return _criticalHitProbability; }
            set { this.RaiseAndSetIfChanged(ref _criticalHitProbability, value); }
        }
        public double HpRegen
        {
            get { return _hpRegen; }
            set { this.RaiseAndSetIfChanged(ref _hpRegen, value); }
        }
        public double HpRegenBase
        {
            get { return _hpRegenBase; }
            set { this.RaiseAndSetIfChanged(ref _hpRegenBase, value); }
        }
        public double MpRegen
        {
            get { return _mpRegen; }
            set { this.RaiseAndSetIfChanged(ref _mpRegen, value); }
        }
        public double MpRegenBase
        {
            get { return _mpRegenBase; }
            set { this.RaiseAndSetIfChanged(ref _mpRegenBase, value); }
        }
        public double Strength
        {
            get { return _strength; }
            set { this.RaiseAndSetIfChanged(ref _strength, value); }
        }
        public double StrengthBase
        {
            get { return _strengthBase; }
            set { this.RaiseAndSetIfChanged(ref _strengthBase, value); }
        }
        public double Agility
        {
            get { return _agility; }
            set { this.RaiseAndSetIfChanged(ref _agility, value); }
        }
        public double AgilityBase
        {
            get { return _agilityBase; }
            set { this.RaiseAndSetIfChanged(ref _agilityBase, value); }
        }
        public double Intelligence
        {
            get { return _intelligence; }
            set { this.RaiseAndSetIfChanged(ref _intelligence, value); }
        }
        public double IntelligenceBase
        {
            get { return _intelligenceBase; }
            set { this.RaiseAndSetIfChanged(ref _intelligenceBase, value); }
        }
        public double AuraRadius
        {
            get { return _auraRadius; }
            set { this.RaiseAndSetIfChanged(ref _auraRadius, value); }
        }
        public double AuraRadiusBase
        {
            get { return _auraRadiusBase; }
            set { this.RaiseAndSetIfChanged(ref _auraRadiusBase, value); }
        }
        public double MagicBlock
        {
            get { return _magicBlock; }
            set { this.RaiseAndSetIfChanged(ref _magicBlock, value); }
        }
        public double MagicBlockBase
        {
            get { return _magicBlockBase; }
            set { this.RaiseAndSetIfChanged(ref _magicBlockBase, value); }
        }
        public double Speed
        {
            get { return _speed; }
            set { this.RaiseAndSetIfChanged(ref _speed, value); }
        }
        public double SpeedBase
        {
            get { return _speedBase; }
            set { this.RaiseAndSetIfChanged(ref _speedBase, value); }
        }
        public SkillSetViewModel ActiveSkillSet
        {
            get { return _activeSkillSet; }
            set { this.RaiseAndSetIfChanged(ref _activeSkillSet, value); }
        }
        #endregion

        #region (public) Equipped Item Properties
        public EquipmentViewModel EquippedAmulet
        {
            get { return _equippedAmulet; }
            set { this.RaiseAndSetIfChanged(ref _equippedAmulet, value); }
        }
        public EquipmentViewModel EquippedArmor
        {
            get { return _equippedArmor; }
            set { this.RaiseAndSetIfChanged(ref _equippedArmor, value); }
        }
        public EquipmentViewModel EquippedBelt
        {
            get { return _equippedBelt; }
            set { this.RaiseAndSetIfChanged(ref _equippedBelt, value); }
        }
        public EquipmentViewModel EquippedBoots
        {
            get { return _equippedBoots; }
            set { this.RaiseAndSetIfChanged(ref _equippedBoots, value); }
        }
        public EquipmentViewModel EquippedGauntlets
        {
            get { return _equippedGauntlets; }
            set { this.RaiseAndSetIfChanged(ref _equippedGauntlets, value); }
        }
        public EquipmentViewModel EquippedHelmet
        {
            get { return _equippedHelmet; }
            set { this.RaiseAndSetIfChanged(ref _equippedHelmet, value); }
        }
        public EquipmentViewModel EquippedLeftHandWeapon
        {
            get { return _equippedLeftHandWeapon; }
            set { this.RaiseAndSetIfChanged(ref _equippedLeftHandWeapon, value); }
        }
        public EquipmentViewModel EquippedLeftRing
        {
            get { return _equippedLeftRing; }
            set { this.RaiseAndSetIfChanged(ref _equippedLeftRing, value); }
        }
        public EquipmentViewModel EquippedOrb
        {
            get { return _equippedOrb; }
            set { this.RaiseAndSetIfChanged(ref _equippedOrb, value); }
        }
        public EquipmentViewModel EquippedRightHandWeapon
        {
            get { return _equippedRightHandWeapon; }
            set { this.RaiseAndSetIfChanged(ref _equippedRightHandWeapon, value); }
        }
        public EquipmentViewModel EquippedRightRing
        {
            get { return _equippedRightRing; }
            set { this.RaiseAndSetIfChanged(ref _equippedRightRing, value); }
        }
        public EquipmentViewModel EquippedShoulder
        {
            get { return _equippedShoulder; }
            set { this.RaiseAndSetIfChanged(ref _equippedShoulder, value); }
        }
        #endregion

        public ObservableCollection<SkillSetViewModel> SkillSets { get; set; }
        public ObservableCollection<SkillSetViewModel> SkillSetsLearned { get; set; }

        /// <summary>
        /// Set of aggregate attack attributes used for melee calculations
        /// </summary>
        public ObservableCollection<AttackAttributeViewModel> MeleeAttackAttributes { get; set; }

        /// <summary>
        /// Alteration effect / cause container for player - one per passive or temporary effect
        /// </summary>
        public ObservableCollection<AlterationViewModel> Alterations { get; set; }

        public CharacterClassViewModel CharacterClass { get; set; }

        [ImportingConstructor]
        public PlayerViewModel(
            IEventAggregator eventAggregator, 
            IModelService modelService,
            IPlayerProcessor playerProcessor,
            IAlterationProcessor alterationProcessor,
            IScenarioResourceService scenarioResourceService)
        {
            _modelService = modelService;
            _playerProcessor = playerProcessor;
            _alterationProcessor = alterationProcessor;
            _eventAggregator = eventAggregator;
            _scenarioResourceService = scenarioResourceService;

            this.SkillSets = new ObservableCollection<SkillSetViewModel>();
            this.SkillSetsLearned = new ObservableCollection<SkillSetViewModel>();
            this.MeleeAttackAttributes = new ObservableCollection<AttackAttributeViewModel>();
            this.Alterations = new ObservableCollection<AlterationViewModel>();
            this.CharacterClass = new CharacterClassViewModel();

            eventAggregator.GetEvent<LevelUpdateEvent>().Subscribe(update =>
            {
                // Filtered based on update type
                OnLevelUpdate(update);
            });

            eventAggregator.GetEvent<LevelLoadedEvent>().Subscribe(() =>
            {
                // Unfiltered - processed on level loaded
                ProcessUpdate();
            });
        }

        private void OnLevelUpdate(ILevelUpdate update)
        {
            switch (update.LevelUpdateType)
            {
                case LevelUpdateType.PlayerEquipmentRemove:
                case LevelUpdateType.PlayerEquipmentAddOrUpdate:
                case LevelUpdateType.PlayerSkillSetAdd:
                case LevelUpdateType.PlayerSkillSetRefresh:
                case LevelUpdateType.PlayerStats:
                case LevelUpdateType.PlayerAll:
                case LevelUpdateType.EncyclopediaCurseIdentify:
                case LevelUpdateType.EncyclopediaIdentify:
                    ProcessUpdate();
                    break;
                default:
                    break;
            }
        }

        private void ProcessUpdate()
        {
            var player = _modelService.Player;
            var encyclopedia = _modelService.ScenarioEncyclopedia;
            var equippedItems = player.Equipment.Values.Where(x => x.IsEquipped);

            // Sort Skill Sets
            var sortedSkillSets = player.SkillSets
                                        .OrderBy(x =>
                                        {
                                            return x.Skills.Count == 0 ? 0 :
                                                   x.Skills.Min(skill => skill.LevelRequirement);
                                        })
                                        .Actualize();

            // Base Collections
            SynchronizeCollection(
                sortedSkillSets, 
                this.SkillSets, 
                x => new SkillSetViewModel(x, player, _modelService.ScenarioEncyclopedia, _eventAggregator),
                (source, dest) =>
                {
                    // Update
                    dest.IsActive = source.IsActive;
                    dest.IsTurnedOn = source.IsTurnedOn;
                    dest.HasLearnedSkills = source.Skills.Any(x => x.IsLearned);
                    dest.HasUnlearnedSkills = source.Skills.Any(x => !x.IsLearned);
                    dest.HasUnlearnedAvailableSkills = source.Skills.Any(x => !x.IsLearned && x.AreRequirementsMet(player));
                    dest.HasLearnedUnavailableSkills = source.Skills.Any(x => x.IsLearned && !x.AreRequirementsMet(player));
                    dest.ActiveSkill = source.SelectedSkill == null ? null :
                                       dest.Skills.FirstOrDefault(x => x.Id == source.SelectedSkill.Id);

                    dest.Skills.ForEach(skill =>
                    {
                        var skillSource = source.Skills.First(x => x.Id == skill.Id);
                        
                        skill.IsLearned = skillSource.IsLearned;
                        skill.IsSelected = (source.SelectedSkill != null) && (source.SelectedSkill.Id == skill.Id);
                        skill.IsSkillPointRequirementMet = skillSource.IsLearned || player.SkillPoints >= skillSource.SkillPointRequirement;
                        skill.IsLevelRequirementMet = player.Level >= skillSource.LevelRequirement;
                        skill.IsAttributeRequirementMet = !skillSource.HasAttributeRequirement ||
                                                           player.GetAttribute(skillSource.AttributeRequirement) >= skillSource.AttributeLevelRequirement;
                        skill.IsCharacterClassRequirementMet = !skillSource.HasCharacterClassRequirement ||
                                                                 (skillSource.HasCharacterClassRequirement &&
                                                                 player.CharacterClassAlteration.HasCharacterClass() &&
                                                                 player.CharacterClassAlteration.CharacterClass.RogueName == skillSource.CharacterClass.RogueName);
                    });
                });

            // Create a (reference) copy of the skill sets that are learned for easier binding
            this.SkillSetsLearned.Clear();
            this.SkillSetsLearned.AddRange(this.SkillSets.Where(x => x.HasLearnedSkills));

            // Active Skill Set
            this.ActiveSkillSet = this.SkillSets.FirstOrDefault(x => x.IsActive);

            // Calculate experience
            var experienceLast = player.Level == 0 ? 0 : PlayerCalculator.CalculateExperienceNext(player.Level - 1);
            var experienceNext = PlayerCalculator.CalculateExperienceNext(player.Level);

            var deltaExperience = player.Experience - experienceLast;
            var deltaExperienceNext = experienceNext - experienceLast;

            // Player Stats
            this.Level = player.Level;
            this.Class = player.Class;
            this.Experience = deltaExperience;
            this.ExperienceNext = deltaExperienceNext;
            this.Haul = player.GetHaul();
            this.HaulMax = player.GetHaulMax();
            this.Hp = player.Hp;
            this.HpMax = player.HpMax;
            this.Hunger = player.Hunger;
            this.Mp = player.Mp;
            this.MpMax = player.MpMax;
            this.SkillPoints = player.SkillPoints;
            this.RogueName = player.RogueName;

            // Alterations
            this.Alterations.Clear();
            this.Alterations.AddRange(player.Alteration.Get().Select(tuple => new AlterationViewModel(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4)));

            // Religious Alterations

            // -> Attribute Effect
            if (player.CharacterClassAlteration.HasAttributeEffect)
                this.Alterations.Add(new AlterationViewModel(AlterationType.PassiveSource, AlterationAttackAttributeType.Passive, new AlterationCost(), player.CharacterClassAlteration.AttributeEffect));

            // -> Attack Attribute Effect
            if (player.CharacterClassAlteration.HasAttackAttributeEffect)
                this.Alterations.Add(new AlterationViewModel(AlterationType.PassiveSource, AlterationAttackAttributeType.Passive, new AlterationCost(), player.CharacterClassAlteration.AttackAttributeEffect));

            // Update Effective Symbol
            var symbol = _alterationProcessor.CalculateEffectiveSymbol(player);

            this.CharacterColor = symbol.CharacterColor;
            this.CharacterSymbol = symbol.CharacterSymbol;
            this.Icon = symbol.Icon;
            this.SmileyMood = symbol.SmileyMood;
            this.SmileyAuraColor = symbol.SmileyAuraColor;
            this.SmileyBodyColor = symbol.SmileyBodyColor;
            this.SmileyLineColor = symbol.SmileyLineColor;
            this.SymbolType = symbol.SymbolType;

            // Stats
            this.Agility = player.GetAgility();
            this.AgilityBase = player.AgilityBase;
            this.Attack = player.GetAttack();
            this.AttackBase = player.GetAttackBase();
            this.AuraRadius = player.GetAuraRadius();
            this.AuraRadiusBase = player.AuraRadiusBase;
            this.CriticalHitProbability = player.GetCriticalHitProbability();
            this.Defense = player.GetDefense();
            this.DefenseBase = player.GetDefenseBase();
            this.Dodge = player.GetDodge();
            this.DodgeBase = player.GetDodgeBase();
            this.FoodUsagePerTurn = player.GetFoodUsagePerTurn();
            this.FoodUsagePerTurnBase = player.FoodUsagePerTurnBase;
            this.HpRegen = player.GetHpRegen();
            this.HpRegenBase = player.HpRegenBase;
            this.Intelligence = player.GetIntelligence();
            this.IntelligenceBase = player.IntelligenceBase;
            this.MagicBlock = player.GetMagicBlock();
            this.MagicBlockBase = player.GetMagicBlockBase();
            this.MpRegen = player.GetMpRegen();
            this.MpRegenBase = player.MpRegenBase;
            this.Strength = player.GetStrength();
            this.StrengthBase = player.StrengthBase;
            this.Speed = player.GetSpeed();
            this.SpeedBase = player.SpeedBase;

            var constructor = new Func<Equipment, EquipmentViewModel>(x => x == null ? null : new EquipmentViewModel(x));

            this.EquippedAmulet = constructor(equippedItems.FirstOrDefault(x => x.Type == EquipmentType.Amulet));
            this.EquippedArmor = constructor(equippedItems.FirstOrDefault(x => x.Type == EquipmentType.Armor));
            this.EquippedBelt = constructor(equippedItems.FirstOrDefault(x => x.Type == EquipmentType.Belt));
            this.EquippedBoots = constructor(equippedItems.FirstOrDefault(x => x.Type == EquipmentType.Boots));
            this.EquippedGauntlets = constructor(equippedItems.FirstOrDefault(x => x.Type == EquipmentType.Gauntlets));
            this.EquippedHelmet = constructor(equippedItems.FirstOrDefault(x => x.Type == EquipmentType.Helmet));
            this.EquippedLeftHandWeapon = constructor(equippedItems.FirstOrDefault(x => x.Type == EquipmentType.OneHandedMeleeWeapon || 
                                                                                                                      x.Type == EquipmentType.TwoHandedMeleeWeapon ||
                                                                                                                      x.Type == EquipmentType.RangeWeapon));
            this.EquippedLeftRing = constructor(equippedItems.FirstOrDefault(x => x.Type == EquipmentType.Ring));
            this.EquippedOrb = constructor(equippedItems.FirstOrDefault(x => x.Type == EquipmentType.Orb));

            // Check for two handed weapons first
            this.EquippedRightHandWeapon = constructor(equippedItems.FirstOrDefault(x =>  x.Type == EquipmentType.TwoHandedMeleeWeapon ||
                                                                                                                        x.Type == EquipmentType.RangeWeapon));
            // If none, then check for second one-handed weapon
            if (this.EquippedRightHandWeapon == null)
                this.EquippedRightHandWeapon = constructor(equippedItems.Where(x => x.Type == EquipmentType.OneHandedMeleeWeapon)
                                                                                                      .Skip(1)
                                                                                                      .FirstOrDefault());

            this.EquippedRightRing = constructor(equippedItems.Where(x => x.Type == EquipmentType.Ring)
                                                                                            .Skip(1)
                                                                                            .FirstOrDefault());
            this.EquippedShoulder = constructor(equippedItems.FirstOrDefault(x => x.Type == EquipmentType.Shoulder));

            // Attack Attributes
            var attackAttributes = equippedItems.
            Aggregate(new List<AttackAttributeViewModel>(), (aggregate, equipment) =>
            {
                // Initialize Aggregate
                if (aggregate.Count == 0)
                    aggregate.AddRange(equipment.AttackAttributes.Select(x => new AttackAttributeViewModel(x)));

                // Aggregate Attack Attribute Quantities
                else
                {
                    foreach (var attackAttribute in aggregate)
                    {
                        var equipmentAttribute = equipment.AttackAttributes.First(x => x.RogueName == attackAttribute.RogueName);
                        attackAttribute.Attack += equipmentAttribute.Attack;
                        attackAttribute.Resistance += equipmentAttribute.Resistance;
                    }
                }

                return aggregate;
            }).
            Where(x => x.Attack > 0 || x.Resistance > 0 || x.Weakness > 0);

            this.MeleeAttackAttributes.Clear();
            this.MeleeAttackAttributes.AddRange(attackAttributes);

            // Character Class 
            if (player.CharacterClassAlteration.HasCharacterClass() &&
                _modelService.CharacterClasses.Any()) // Check that model is loaded (could be that no model has loaded for the level)
                                                      // TODO:  Force view model updates to wait until after IModelService is loaded
            {
                this.CharacterClass.DisplayName = player.CharacterClassAlteration.CharacterClass.RogueName;
                this.CharacterClass.HasAttackAttributeBonus = player.CharacterClassAlteration.AttackAttributeEffect != null;
                this.CharacterClass.HasAttributeBonus = player.CharacterClassAlteration.AttributeEffect != null;
                this.CharacterClass.HasCharacterClass = true;

                this.CharacterClass.UpdateSymbol(player.CharacterClassAlteration.Symbol);

                // Bonus Attack Attributes
                if (player.CharacterClassAlteration.AttackAttributeEffect != null)
                {
                    this.CharacterClass.AttackAttributeBonus.Clear();
                    this.CharacterClass
                        .AttackAttributeBonus
                        .AddRange(player.CharacterClassAlteration
                                        .AttackAttributeEffect
                                        .AttackAttributes
                                        .Where(x => x.Attack > 0 || x.Resistance > 0 || x.Weakness > 0)
                                        .Select(x => new AttackAttributeViewModel(x)));
                }

                if (player.CharacterClassAlteration.HasAttributeEffect)
                {
                    // Must be ONE attribute alteration if effect is non-null
                    var attribute = player.CharacterClassAlteration
                                           .AttributeEffect
                                           .GetUIAttributes()
                                           .First();

                    // Show SINGLE attribute bonus as a string
                    this.CharacterClass.AttributeBonus = attribute.Value.Sign() + attribute.Value.ToString("F2") + " " + attribute.Key;
                }
            }
            else
                this.CharacterClass.HasCharacterClass = false;
        }

        private void SynchronizeCollection<TSource, TDest>(
                        IEnumerable<TSource> sourceCollection, 
                        IList<TDest> destCollection,
                        Func<TSource, TDest> constructor,
                        Action<TSource, TDest> update) where TSource : ScenarioImage
                                                           where TDest : ScenarioImageViewModel
        {
            foreach (var item in sourceCollection)
            {
                var destItem = destCollection.FirstOrDefault(x => x.Id == item.Id);

                // Add
                if (destItem == null)
                    destCollection.Add(constructor(item));

                // Update
                else
                    update(item, destItem);
            }
            for (int i = destCollection.Count - 1; i >= 0; i--)
            {
                // Remove
                if (!sourceCollection.Any(x => x.Id == destCollection[i].Id))
                    destCollection.RemoveAt(i);
            }
        }
    }
}
