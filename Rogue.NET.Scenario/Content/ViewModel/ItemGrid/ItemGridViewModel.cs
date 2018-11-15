using Prism.Events;
using Rogue.NET.Common.ViewModel;
using Rogue.NET.Core.Event.Scenario.Level.Event;
using Rogue.NET.Core.Logic.Content.Interface;
using Rogue.NET.Core.Logic.Processing.Enum;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Model.Scenario.Content.Item;
using Rogue.NET.Core.Service.Interface;
using Rogue.NET.Model.Events;
using Rogue.NET.Scenario.Content.ViewModel.Content;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;

namespace Rogue.NET.Scenario.ViewModel.ItemGrid
{
    [Export]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class ItemGridViewModel : NotifyViewModel
    {
        // Have to maintain these collections for use in the ItemGrid
        public ObservableCollection<ItemGridRowViewModel> Inventory { get; set; }
        public ObservableCollection<ItemGridRowViewModel> Consumables { get; set; }
        public ObservableCollection<ItemGridRowViewModel> Equipment { get; set; }
        public ObservableCollection<ItemGridRowViewModel> IdentifyInventory { get; set; }
        public ObservableCollection<ItemGridRowViewModel> UncurseEquipment { get; set; }
        public ObservableCollection<ItemGridRowViewModel> EnchantEquipment { get; set; }
        public ObservableCollection<ItemGridRowViewModel> ImbueEquipment { get; set; }

        [ImportingConstructor]
        public ItemGridViewModel(
            IEventAggregator eventAggregator, 
            IModelService modelService,
            IItemProcessor itemProcessor)
        {
            this.Inventory = new ObservableCollection<ItemGridRowViewModel>();
            this.Consumables = new ObservableCollection<ItemGridRowViewModel>();
            this.Equipment = new ObservableCollection<ItemGridRowViewModel>();
            this.IdentifyInventory = new ObservableCollection<ItemGridRowViewModel>();
            this.UncurseEquipment = new ObservableCollection<ItemGridRowViewModel>();
            this.EnchantEquipment = new ObservableCollection<ItemGridRowViewModel>();
            this.ImbueEquipment = new ObservableCollection<ItemGridRowViewModel>();

            // Player Events
            eventAggregator.GetEvent<LevelUpdateEvent>().Subscribe(update =>
            {
                switch (update.LevelUpdateType)
                {
                    case LevelUpdateType.PlayerConsumableRemove:
                    case LevelUpdateType.PlayerConsumableAddOrUpdate:
                    case LevelUpdateType.PlayerEquipmentRemove:
                    case LevelUpdateType.PlayerEquipmentAddOrUpdate:
                    case LevelUpdateType.PlayerAll:
                        UpdateCollections(modelService, itemProcessor);
                        break;
                    default:
                        break;
                }
            }, true);

            // Level Loaded
            eventAggregator.GetEvent<LevelLoadedEvent>().Subscribe(() =>
            {
                UpdateCollections(modelService, itemProcessor);
            }, true);
        }

        private void UpdateCollections(IModelService modelService, IItemProcessor itemProcessor)
        {
            // Inventory
            SynchronizeCollection<ItemBase, ItemGridRowViewModel>(
                modelService.Player.Inventory.Values,
                this.Inventory,
                x => (x is Consumable) ?
                      new ItemGridRowViewModel(x as Consumable,
                                               modelService.ScenarioEncyclopedia[x.RogueName],
                                               modelService.Player
                                                           .Inventory
                                                           .Values
                                                           .Count(z => z.RogueName == x.RogueName)) :
                      new ItemGridRowViewModel(x as Equipment,
                                               modelService.ScenarioEncyclopedia[x.RogueName]));

            // Consumables
            SynchronizeCollection<Consumable, ItemGridRowViewModel>(
                modelService.Player.Consumables.Values,
                this.Consumables,
                x => new ItemGridRowViewModel(x, modelService.ScenarioEncyclopedia[x.RogueName],
                                                 modelService.Player
                                                             .Inventory
                                                             .Values
                                                             .Count(z => z.RogueName == x.RogueName)));

            // Equipment
            SynchronizeCollection<Equipment, ItemGridRowViewModel>(
                modelService.Player.Equipment.Values,
                this.Equipment,
                x => new ItemGridRowViewModel(x, modelService.ScenarioEncyclopedia[x.RogueName]));

            // Identify Inventory
            SynchronizeCollection<ItemBase, ItemGridRowViewModel>(
                modelService.Player
                            .Inventory
                            .Values
                            .Where(x => !x.IsIdentified ||
                                        !modelService.ScenarioEncyclopedia[x.RogueName].IsIdentified),
                this.IdentifyInventory,
                x => (x is Consumable) ?
                      new ItemGridRowViewModel(x as Consumable,
                                               modelService.ScenarioEncyclopedia[x.RogueName],
                                               modelService.Player
                                                           .Inventory
                                                           .Values
                                                           .Count(z => z.RogueName == x.RogueName)) :
                      new ItemGridRowViewModel(x as Equipment,
                                               modelService.ScenarioEncyclopedia[x.RogueName]));

            // Uncurse Equipment
            SynchronizeCollection<Equipment, ItemGridRowViewModel>(
                modelService.Player.Equipment.Values.Where(x => x.IsCursed),
                this.UncurseEquipment,
                x => new ItemGridRowViewModel(x, modelService.ScenarioEncyclopedia[x.RogueName]));

            // Enchant Equipment
            SynchronizeCollection<Equipment, ItemGridRowViewModel>(
                modelService.Player.Equipment.Values.Where(x => itemProcessor.ClassApplies(x.Type)),
                this.EnchantEquipment,
                x => new ItemGridRowViewModel(x, modelService.ScenarioEncyclopedia[x.RogueName]));

            // Imbue Equipment (set to enchant for now)
            SynchronizeCollection<Equipment, ItemGridRowViewModel>(
                modelService.Player.Equipment.Values.Where(x => itemProcessor.ClassApplies(x.Type)),
                this.ImbueEquipment,
                x => new ItemGridRowViewModel(x, modelService.ScenarioEncyclopedia[x.RogueName]));
        }

        private void SynchronizeCollection<TSource, TDest>(
                IEnumerable<TSource> sourceCollection,
                IList<ItemGridRowViewModel> destCollection,
                Func<TSource, ItemGridRowViewModel> constructor) where TSource : ScenarioObject
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
