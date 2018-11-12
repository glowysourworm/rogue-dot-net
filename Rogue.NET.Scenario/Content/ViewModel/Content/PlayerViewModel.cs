using Prism.Events;
using Rogue.NET.Common.ViewModel;
using Rogue.NET.Core.Event.Scenario.Level.Event;
using Rogue.NET.Core.Logic.Processing.Enum;
using Rogue.NET.Core.Service.Interface;
using System.Linq;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using Rogue.NET.Core.Logic.Content.Interface;

namespace Rogue.NET.Scenario.Content.ViewModel.Content
{
    [Export]
    public class PlayerViewModel : NotifyViewModel
    {
        #region (private) Backing Fields
        int _level;
        string _class;
        string _rogueName;
        double _experience;
        double _hunger;
        double _haul;
        double _haulMax;
        double _hpMax;
        double _mpMax;
        double _hp;
        double _mp;
        SkillSetViewModel _activeSkillSet;
        #endregion

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
        public SkillSetViewModel ActiveSkillSet
        {
            get { return _activeSkillSet; }
            set { this.RaiseAndSetIfChanged(ref _activeSkillSet, value); }
        }

        public ObservableCollection<EquipmentViewModel> Equipment { get; set; }
        public ObservableCollection<ConsumableViewModel> Consumables { get; set; }

        [ImportingConstructor]
        public PlayerViewModel(
            IEventAggregator eventAggregator, 
            IModelService modelService,
            ICharacterProcessor characterProcessor)
        {
            this.Equipment = new ObservableCollection<EquipmentViewModel>();
            this.Consumables = new ObservableCollection<ConsumableViewModel>();

            eventAggregator.GetEvent<LevelUpdateEvent>().Subscribe(update =>
            {
                // TODO: Filter events using the Type

                var player = modelService.Player;

                // Synchronize Equipment
                foreach (var equipment in player.Equipment.Values)
                {
                    if (!this.Equipment.Any(x => x.Id == equipment.Id))
                        this.Equipment.Add(new EquipmentViewModel(equipment));
                }
                for (int i = this.Equipment.Count - 1; i >= 0; i--)
                {
                    if (!player.Equipment.ContainsKey(this.Equipment[i].Id))
                        this.Equipment.RemoveAt(i);
                }

                // Synchronize Consumables
                foreach (var consumable in player.Consumables.Values)
                {
                    if (!this.Consumables.Any(x => x.Id == consumable.Id))
                        this.Consumables.Add(new ConsumableViewModel(consumable));
                }
                for (int i = this.Consumables.Count - 1; i >= 0; i--)
                {
                    if (!player.Consumables.ContainsKey(this.Consumables[i].Id))
                        this.Consumables.RemoveAt(i);
                }

                // Active Skill
                var activeSkillSet = player.SkillSets.FirstOrDefault(x => x.IsActive);

                this.ActiveSkillSet = activeSkillSet == null ? null : new SkillSetViewModel(activeSkillSet);

                this.Level = player.Level;
                this.Class = player.Class;
                this.Experience = player.Experience;
                this.Haul = characterProcessor.GetHaul(player);
                this.HaulMax = characterProcessor.GetHaulMax(player);
                this.Hp = player.Hp;
                this.HpMax = player.HpMax;
                this.Hunger = player.Hunger;
                this.Mp = player.Mp;
                this.MpMax = player.MpMax;
                this.RogueName = player.RogueName;
            }, true);
        }
    }
}
