using Prism.Events;
using Rogue.NET.Common.ViewModel;
using Rogue.NET.Core.Event.Scenario.Level.Event;
using Rogue.NET.Core.Logic.Processing.Enum;
using Rogue.NET.Core.Service.Interface;
using System.Linq;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using Rogue.NET.Core.Logic.Content.Interface;
using System.Collections.Generic;
using Rogue.NET.Core.Model.Scenario.Content;
using System;

namespace Rogue.NET.Scenario.Content.ViewModel.Content
{
    [Export]
    [PartCreationPolicy(CreationPolicy.Shared)]
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

        [ImportingConstructor]
        public PlayerViewModel(
            IEventAggregator eventAggregator, 
            IModelService modelService,
            ICharacterProcessor characterProcessor)
        {
            this.Equipment = new ObservableCollection<EquipmentViewModel>();

            eventAggregator.GetEvent<LevelUpdateEvent>().Subscribe(update =>
            {
                // TODO: Filter events using the Type

                var player = modelService.Player;
                var encyclopedia = modelService.ScenarioEncyclopedia;

                // Base Collections
                SynchronizeCollection(player.Equipment.Values, this.Equipment, (x) => new EquipmentViewModel(x));

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

        private void SynchronizeCollection<TSource, TDest>(
                        IEnumerable<TSource> sourceCollection, 
                        IList<TDest> destCollection,
                        Func<TSource, TDest> constructor) where TSource : ScenarioObject 
                                                           where TDest : ScenarioObjectViewModel
        {
            foreach (var item in sourceCollection)
            {
                if (!destCollection.Any(x => x.Id == item.Id))
                    destCollection.Add(constructor(item));
            }
            for (int i = destCollection.Count - 1; i >= 0; i--)
            {
                if (!sourceCollection.Any(x => x.Id == destCollection[i].Id))
                    destCollection.RemoveAt(i);
            }
        }
    }
}
