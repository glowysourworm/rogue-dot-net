using Prism.Events;
using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.Common.ViewModel;
using Rogue.NET.Core.Event.Scenario.Level.Event;
using Rogue.NET.Core.Logic.Processing.Enum;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Model.Scenario.Content.Item;
using Rogue.NET.Core.Model.Scenario.Content.Item.Extension;
using Rogue.NET.Core.Service.Interface;
using Rogue.NET.Model.Events;
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
        public ObservableCollection<ItemGridRowViewModel> Consumables { get; set; }
        public ObservableCollection<ItemGridRowViewModel> Equipment { get; set; }
        public ObservableCollection<ItemGridRowViewModel> IdentifyInventory { get; set; }
        public ObservableCollection<ItemGridRowViewModel> UncurseEquipment { get; set; }
        public ObservableCollection<ItemGridRowViewModel> EnchantArmorEquipment { get; set; }
        public ObservableCollection<ItemGridRowViewModel> EnchantWeaponEquipment { get; set; }
        public ObservableCollection<ItemGridRowViewModel> EnhanceWeaponEquipment { get; set; }
        public ObservableCollection<ItemGridRowViewModel> EnhanceArmorEquipment { get; set; }
        public ObservableCollection<ItemGridRowViewModel> ImbueWeaponEquipment { get; set; }
        public ObservableCollection<ItemGridRowViewModel> ImbueArmorEquipment { get; set; }

        [ImportingConstructor]
        public ItemGridViewModel(
            IRogueEventAggregator eventAggregator, 
            IModelService modelService)
        {
            this.Consumables = new ObservableCollection<ItemGridRowViewModel>();
            this.Equipment = new ObservableCollection<ItemGridRowViewModel>();
            this.IdentifyInventory = new ObservableCollection<ItemGridRowViewModel>();
            this.UncurseEquipment = new ObservableCollection<ItemGridRowViewModel>();
            this.EnchantArmorEquipment = new ObservableCollection<ItemGridRowViewModel>();
            this.EnchantWeaponEquipment = new ObservableCollection<ItemGridRowViewModel>();
            this.EnhanceArmorEquipment = new ObservableCollection<ItemGridRowViewModel>();
            this.EnhanceWeaponEquipment = new ObservableCollection<ItemGridRowViewModel>();
            this.ImbueWeaponEquipment = new ObservableCollection<ItemGridRowViewModel>();
            this.ImbueArmorEquipment = new ObservableCollection<ItemGridRowViewModel>();

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
                    case LevelUpdateType.EncyclopediaCurseIdentify:
                    case LevelUpdateType.EncyclopediaIdentify:
                        UpdateCollections(modelService);
                        break;
                    default:
                        break;
                }
            });

            // Level Loaded
            eventAggregator.GetEvent<LevelLoadedEvent>().Subscribe(() =>
            {
                UpdateCollections(modelService);
            });
        }

        private void UpdateCollections(IModelService modelService)
        {
            var encyclopedia = modelService.ScenarioEncyclopedia;
            var consumables = modelService.Player.Consumables.Values;
            var inventory = modelService.Player.Inventory.Values;
            var identifyConsumable = inventory.Any(x => (!x.IsIdentified && x is Equipment) || !modelService.ScenarioEncyclopedia[x.RogueName].IsIdentified);

            // Consumables
            SynchronizeCollection<Consumable, ItemGridRowViewModel>(
                consumables.GroupBy(x => x.RogueName).Select(x => x.First()),
                this.Consumables,
                item => new ItemGridRowViewModel(item, encyclopedia[item.RogueName],
                                                 modelService.CharacterClasses,
                                                 identifyConsumable,
                                                 consumables.Count(z => z.RogueName == item.RogueName),
                                                 consumables.Where(z => z.RogueName == item.RogueName).Sum(z => z.Uses),
                                                 consumables.Where(z => z.RogueName == item.RogueName).Sum(z => z.Weight)),
                (viewModel, consumable) =>
                {
                    viewModel.UpdateConsumable(consumable, 
                                               encyclopedia[viewModel.RogueName],
                                               modelService.CharacterClasses,
                                               identifyConsumable,
                                               consumables.Count(z => z.RogueName == consumable.RogueName),
                                               consumables.Where(z => z.RogueName == consumable.RogueName).Sum(z => z.Uses),
                                               consumables.Where(z => z.RogueName == consumable.RogueName).Sum(z => z.Weight));
                });

            // Equipment
            SynchronizeCollection<Equipment, ItemGridRowViewModel>(
                modelService.Player.Equipment.Values,
                this.Equipment,
                item => new ItemGridRowViewModel(item, encyclopedia[item.RogueName], modelService.CharacterClasses),
                (viewModel, item) => viewModel.UpdateEquipment(item, encyclopedia[item.RogueName], modelService.CharacterClasses));

            // Identify Inventory
            SynchronizeCollection<ItemBase, ItemGridRowViewModel>(
                inventory.Where(x => (!x.IsIdentified && x is Equipment) ||
                                      !modelService.ScenarioEncyclopedia[x.RogueName].IsIdentified),
                this.IdentifyInventory,
                item =>
                {
                    if (item is Consumable)
                        return new ItemGridRowViewModel(item as Consumable,
                                             encyclopedia[item.RogueName],
                                             modelService.CharacterClasses,
                                             identifyConsumable, 1, (item as Consumable).Uses, item.Weight);
                    else
                        return new ItemGridRowViewModel(item as Equipment, encyclopedia[item.RogueName], modelService.CharacterClasses);
                },
                (viewModel, item) =>
                {
                    if (item is Consumable)
                        viewModel.UpdateConsumable(item as Consumable, encyclopedia[viewModel.RogueName], modelService.CharacterClasses, identifyConsumable, 1, (item as Consumable).Uses, item.Weight);
                    else
                        viewModel.UpdateEquipment(item as Equipment, encyclopedia[viewModel.RogueName], modelService.CharacterClasses);
                });


            // Uncurse Equipment
            SynchronizeCollection<Equipment, ItemGridRowViewModel>(
                modelService.Player.Equipment.Values.Where(x => x.IsCursed),
                this.UncurseEquipment,
                item => new ItemGridRowViewModel(item, encyclopedia[item.RogueName], modelService.CharacterClasses),
                (viewModel, item) => viewModel.UpdateEquipment(item, encyclopedia[item.RogueName], modelService.CharacterClasses));

            // Enchant Weapon Equipment
            SynchronizeCollection<Equipment, ItemGridRowViewModel>(
                modelService.Player.Equipment.Values.Where(x => x.IsWeaponType() && x.ClassApplies()),
                this.EnchantWeaponEquipment,
                item => new ItemGridRowViewModel(item, encyclopedia[item.RogueName], modelService.CharacterClasses),
                (viewModel, item) => viewModel.UpdateEquipment(item, encyclopedia[item.RogueName], modelService.CharacterClasses));

            // Enchant Armor Equipment
            SynchronizeCollection<Equipment, ItemGridRowViewModel>(
                modelService.Player.Equipment.Values.Where(x => x.IsArmorType() && x.ClassApplies()),
                this.EnchantArmorEquipment,
                item => new ItemGridRowViewModel(item, encyclopedia[item.RogueName], modelService.CharacterClasses),
                (viewModel, item) => viewModel.UpdateEquipment(item, encyclopedia[item.RogueName], modelService.CharacterClasses));

            // Imbue Weapon Equipment
            SynchronizeCollection<Equipment, ItemGridRowViewModel>(
                modelService.Player.Equipment.Values.Where(x => x.IsWeaponType() && x.CanImbue()),
                this.ImbueWeaponEquipment,
                item => new ItemGridRowViewModel(item, encyclopedia[item.RogueName], modelService.CharacterClasses),
                (viewModel, item) => viewModel.UpdateEquipment(item, encyclopedia[item.RogueName], modelService.CharacterClasses));

            // Imbue Armor Equipment
            SynchronizeCollection<Equipment, ItemGridRowViewModel>(
                modelService.Player.Equipment.Values.Where(x => x.IsArmorType() && x.CanImbue()),
                this.ImbueArmorEquipment,
                item => new ItemGridRowViewModel(item, encyclopedia[item.RogueName], modelService.CharacterClasses),
                (viewModel, item) => viewModel.UpdateEquipment(item, encyclopedia[item.RogueName], modelService.CharacterClasses));

            // Enhance Armor Equipment
            SynchronizeCollection<Equipment, ItemGridRowViewModel>(
                modelService.Player.Equipment.Values.Where(x => x.IsArmorType() && x.ClassApplies()),
                this.EnhanceArmorEquipment,
                item => new ItemGridRowViewModel(item, encyclopedia[item.RogueName], modelService.CharacterClasses),
                (viewModel, item) => viewModel.UpdateEquipment(item, encyclopedia[item.RogueName], modelService.CharacterClasses));

            // Enhance Weapon Equipment
            SynchronizeCollection<Equipment, ItemGridRowViewModel>(
                modelService.Player.Equipment.Values.Where(x => x.IsWeaponType() && x.ClassApplies()),
                this.EnhanceWeaponEquipment,
                item => new ItemGridRowViewModel(item, encyclopedia[item.RogueName], modelService.CharacterClasses),
                (viewModel, item) => viewModel.UpdateEquipment(item, encyclopedia[item.RogueName], modelService.CharacterClasses));
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
