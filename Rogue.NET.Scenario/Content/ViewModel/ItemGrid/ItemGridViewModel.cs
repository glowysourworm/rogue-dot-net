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
            }, ThreadOption.UIThread, true);

            // Level Loaded
            eventAggregator.GetEvent<LevelLoadedEvent>().Subscribe(() =>
            {
                UpdateCollections(modelService, itemProcessor);
            }, ThreadOption.UIThread, true);
        }

        private void UpdateCollections(IModelService modelService, IItemProcessor itemProcessor)
        {
            var encyclopedia = modelService.ScenarioEncyclopedia;
            var consumables = modelService.Player.Consumables.Values;
            var inventory = modelService.Player.Inventory.Values;
            var identifyConsumable = inventory.Any(x => (!x.IsIdentified && x is Equipment) || !modelService.ScenarioEncyclopedia[x.RogueName].IsIdentified);

            // Inventory
            SynchronizeCollection<ItemBase, ItemGridRowViewModel>(
                inventory,
                this.Inventory,
                item =>
                {
                    if (item is Consumable)
                        return new ItemGridRowViewModel(item as Consumable,
                                             encyclopedia[item.RogueName],
                                             identifyConsumable, 1, (item as Consumable).Uses, item.Weight);
                    else
                        return new ItemGridRowViewModel(item as Equipment, encyclopedia[item.RogueName]);
                },
                (viewModel, item) =>
                {
                    if (item is Consumable)
                        viewModel.UpdateConsumable(item as Consumable, encyclopedia[viewModel.RogueName], identifyConsumable, 1, (item as Consumable).Uses, item.Weight);
                    else
                        viewModel.UpdateEquipment(item as Equipment, encyclopedia[viewModel.RogueName]);
                });

            // Consumables
            SynchronizeCollection<Consumable, ItemGridRowViewModel>(
                consumables.GroupBy(x => x.RogueName).Select(x => x.First()),
                this.Consumables,
                item => new ItemGridRowViewModel(item, encyclopedia[item.RogueName],
                                                 identifyConsumable,
                                                 consumables.Count(z => z.RogueName == item.RogueName),
                                                 consumables.Where(z => z.RogueName == item.RogueName).Sum(z => z.Uses),
                                                 consumables.Where(z => z.RogueName == item.RogueName).Sum(z => z.Weight)),
                (viewModel, consumable) =>
                {
                    viewModel.UpdateConsumable(consumable, 
                                               encyclopedia[viewModel.RogueName], 
                                               identifyConsumable,
                                               consumables.Count(z => z.RogueName == consumable.RogueName),
                                               consumables.Where(z => z.RogueName == consumable.RogueName).Sum(z => z.Uses),
                                               consumables.Where(z => z.RogueName == consumable.RogueName).Sum(z => z.Weight));
                });

            // Equipment
            SynchronizeCollection<Equipment, ItemGridRowViewModel>(
                modelService.Player.Equipment.Values,
                this.Equipment,
                item => new ItemGridRowViewModel(item, encyclopedia[item.RogueName]),
                (viewModel, item) => viewModel.UpdateEquipment(item, encyclopedia[item.RogueName]));

            // Identify Inventory
            SynchronizeCollection<ItemBase, ItemGridRowViewModel>(
                inventory.Where(x => !x.IsIdentified ||
                                     !modelService.ScenarioEncyclopedia[x.RogueName].IsIdentified),
                this.IdentifyInventory,
                item =>
                {
                    if (item is Consumable)
                        return new ItemGridRowViewModel(item as Consumable,
                                             encyclopedia[item.RogueName],
                                             identifyConsumable, 1, (item as Consumable).Uses, item.Weight);
                    else
                        return new ItemGridRowViewModel(item as Equipment, encyclopedia[item.RogueName]);
                },
                (viewModel, item) =>
                {
                    if (item is Consumable)
                        viewModel.UpdateConsumable(item as Consumable, encyclopedia[viewModel.RogueName], identifyConsumable, 1, (item as Consumable).Uses, item.Weight);
                    else
                        viewModel.UpdateEquipment(item as Equipment, encyclopedia[viewModel.RogueName]);
                });


            // Uncurse Equipment
            SynchronizeCollection<Equipment, ItemGridRowViewModel>(
                modelService.Player.Equipment.Values.Where(x => x.IsCursed),
                this.UncurseEquipment,
                item => new ItemGridRowViewModel(item, encyclopedia[item.RogueName]),
                (viewModel, item) => viewModel.UpdateEquipment(item, encyclopedia[item.RogueName]));

            // Enchant Equipment
            SynchronizeCollection<Equipment, ItemGridRowViewModel>(
                modelService.Player.Equipment.Values.Where(x => itemProcessor.ClassApplies(x.Type)),
                this.EnchantEquipment,
                item => new ItemGridRowViewModel(item, encyclopedia[item.RogueName]),
                (viewModel, item) => viewModel.UpdateEquipment(item, encyclopedia[item.RogueName]));

            // Imbue Equipment (set to enchant for now)
            SynchronizeCollection<Equipment, ItemGridRowViewModel>(
                modelService.Player.Equipment.Values.Where(x => itemProcessor.ClassApplies(x.Type)),
                this.ImbueEquipment,
                item => new ItemGridRowViewModel(item, encyclopedia[item.RogueName]),
                (viewModel, item) => viewModel.UpdateEquipment(item, encyclopedia[item.RogueName]));

        }

        private void SynchronizeCollection<TSource, TDest>(
                IEnumerable<TSource> sourceCollection,
                IList<ItemGridRowViewModel> destCollection,
                Func<TSource, ItemGridRowViewModel> constructor,
                Action<ItemGridRowViewModel, TSource> updateAction) where TSource : ScenarioObject
        {
            foreach (var item in sourceCollection)
            {
                var itemGridRowItem = destCollection.FirstOrDefault(x => x.Id == item.Id);

                // Add
                if (itemGridRowItem == null)
                    destCollection.Add(constructor(item));

                // Update
                else
                    updateAction(itemGridRowItem, item);
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
