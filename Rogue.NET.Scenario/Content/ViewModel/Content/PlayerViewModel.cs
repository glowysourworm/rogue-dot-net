using Prism.Events;
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
using ExpressMapper;
using Rogue.NET.Core.Logic.Processing.Interface;
using Rogue.NET.Core.Logic.Processing.Enum;
using Rogue.NET.Model.Events;

namespace Rogue.NET.Scenario.Content.ViewModel.Content
{
    [Export]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class PlayerViewModel : NotifyViewModel
    {
        readonly IModelService _modelService;
        readonly ICharacterProcessor _characterProcessor;
        readonly IPlayerProcessor _playerProcessor;

        #region (private) Backing Fields
        int _level;
        string _class;
        string _rogueName;
        double _experience;
        double _experienceNext;
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
        SkillSetViewModel _activeSkillSet;
        #endregion

        #region (public) Properties
        public int Level
        {
            get { return _level; }
            set { this.RaiseAndSetIfChanged(ref _level, value); }
        }
        public string Class
        {
            get { return _class; }
            set { this.RaiseAndSetIfChanged(ref _class, value); }
        }
        public string RogueName
        {
            get { return _rogueName; }
            set { this.RaiseAndSetIfChanged(ref _rogueName, value); }
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

        public SkillSetViewModel ActiveSkillSet
        {
            get { return _activeSkillSet; }
            set { this.RaiseAndSetIfChanged(ref _activeSkillSet, value); }
        }
        #endregion

        public ObservableCollection<EquipmentViewModel> Equipment { get; set; }
        public ObservableCollection<SkillSetViewModel> SkillSets { get; set; }

        [ImportingConstructor]
        public PlayerViewModel(
            IEventAggregator eventAggregator, 
            IModelService modelService,
            IPlayerProcessor playerProcessor,
            ICharacterProcessor characterProcessor)
        {
            _modelService = modelService;
            _characterProcessor = characterProcessor;
            _playerProcessor = playerProcessor;

            this.Equipment = new ObservableCollection<EquipmentViewModel>();
            this.SkillSets = new ObservableCollection<SkillSetViewModel>();

            eventAggregator.GetEvent<LevelUpdateEvent>().Subscribe(update =>
            {
                // Filtered based on update type
                OnLevelUpdate(update);

            }, ThreadOption.UIThread, true);

            eventAggregator.GetEvent<LevelLoadedEvent>().Subscribe(() =>
            {
                // Unfiltered - processed on level loaded
                ProcessUpdate();
            }, ThreadOption.UIThread, true);
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

            // Base Collections
            SynchronizeCollection(player.Equipment.Values, this.Equipment, (x) => new EquipmentViewModel(x));
            SynchronizeCollection(player.SkillSets, this.SkillSets, x => new SkillSetViewModel(x));

            // Active Skill
            var activeSkillSet = player.SkillSets.FirstOrDefault(x => x.IsActive);

            this.ActiveSkillSet = activeSkillSet == null ? null : new SkillSetViewModel(activeSkillSet);

            this.Level = player.Level;
            this.Class = player.Class;
            this.Experience = player.Experience;
            this.ExperienceNext = _playerProcessor.CalculateExperienceNext(player);
            this.Haul = _characterProcessor.GetHaul(player);
            this.HaulMax = _characterProcessor.GetHaulMax(player);
            this.Hp = player.Hp;
            this.HpMax = player.HpMax;
            this.Hunger = player.Hunger;
            this.Mp = player.Mp;
            this.MpMax = player.MpMax;
            this.RogueName = player.RogueName;

            this.Agility = _characterProcessor.GetAgility(player);
            this.AgilityBase = player.AgilityBase;
            this.Attack = _playerProcessor.GetAttack(player);
            this.AttackBase = _playerProcessor.GetAttackBase(player);
            this.AuraRadius = _characterProcessor.GetAuraRadius(player);
            this.AuraRadiusBase = player.AuraRadiusBase;
            this.CriticalHitProbability = _playerProcessor.GetCriticalHitProbability(player);
            this.Defense = _playerProcessor.GetDefense(player);
            this.DefenseBase = _playerProcessor.GetDefenseBase(player);
            this.Dodge = _characterProcessor.GetDodge(player);
            this.DodgeBase = _characterProcessor.GetDodgeBase(player);
            this.FoodUsagePerTurn = _playerProcessor.GetFoodUsagePerTurn(player);
            this.FoodUsagePerTurnBase = player.FoodUsagePerTurnBase;
            this.HpRegen = _characterProcessor.GetHpRegen(player, true);
            this.HpRegenBase = player.HpRegenBase;
            this.Intelligence = _characterProcessor.GetIntelligence(player);
            this.IntelligenceBase = player.IntelligenceBase;
            this.MagicBlock = _characterProcessor.GetMagicBlock(player);
            this.MagicBlockBase = _characterProcessor.GetMagicBlockBase(player);
            this.MpRegen = _characterProcessor.GetMpRegen(player);
            this.MpRegenBase = player.MpRegenBase;
            this.Strength = _characterProcessor.GetStrength(player);
            this.StrengthBase = player.StrengthBase;
        }

        private void SynchronizeCollection<TSource, TDest>(
                        IEnumerable<TSource> sourceCollection, 
                        IList<TDest> destCollection,
                        Func<TSource, TDest> constructor) where TSource : ScenarioImage
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
                    Mapper.Map<TSource, TDest>(item, destItem);
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
