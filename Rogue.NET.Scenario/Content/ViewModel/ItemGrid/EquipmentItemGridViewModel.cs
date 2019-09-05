using Rogue.NET.Common.Extension;
using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.Core.Model.Scenario.Content.Item;
using Rogue.NET.Core.Model.Scenario.Content.Item.Extension;
using Rogue.NET.Core.Service.Interface;
using Rogue.NET.Scenario.Content.ViewModel.ItemGrid.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace Rogue.NET.Scenario.Content.ViewModel.ItemGrid
{
    /// <summary>
    /// View Model component for the equipment item grid. updates from the backend are
    /// subscribed to for updating individual items; and two constructors are provided for
    /// injection / manual use.
    /// </summary>
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export]
    public class EquipmentItemGridViewModel : ItemGridViewModel<Equipment>
    {
        [ImportingConstructor]
        public EquipmentItemGridViewModel(
            IRogueEventAggregator eventAggregator,
            IModelService modelService) : base(eventAggregator, modelService)
        {
            // Initializing Intended Action Here
            //
            // NOTE*** This is for the primary use of this view model - which is to
            //         provide data binding for the primary equipment grids.
            this.IntendedAction = ItemGridIntendedAction.Equip;
            this.SelectionMode = ItemGridSelectionMode.Single;
        }

        protected override bool IsItemEnabled(Equipment item, IModelService modelService)
        {
            var metaData = modelService.ScenarioEncyclopedia[item.RogueName];
            var equipment = item as Equipment;

            switch (this.IntendedAction)
            {
                case ItemGridIntendedAction.Drop:
                case ItemGridIntendedAction.Equip:
                    return true;

                // Enchant / Imbue / Enhance:  Check for equipment type and that modification applies
                //                             Enhance => Quality change => Weapon used for combat => "Class Applies"
                //
                case ItemGridIntendedAction.EnchantWeapon:
                    return equipment.IsWeaponType() && equipment.ClassApplies();
                case ItemGridIntendedAction.EnchantArmor:
                    return equipment.IsArmorType() && equipment.ClassApplies();
                case ItemGridIntendedAction.ImbueArmor:
                    return equipment.IsArmorType() && equipment.CanImbue();
                case ItemGridIntendedAction.ImbueWeapon:
                    return equipment.IsWeaponType() && equipment.CanImbue();
                case ItemGridIntendedAction.EnhanceWeapon:
                    return equipment.IsWeaponType() && equipment.ClassApplies();
                case ItemGridIntendedAction.EnhanceArmor:
                    return equipment.IsArmorType() && equipment.ClassApplies();

                // Identify any item where NONE have been identified OR the item hasn't been
                // identified AND it is a combat item. 
                case ItemGridIntendedAction.Identify:
                    return !metaData.IsIdentified ||
                           (!equipment.IsIdentified && equipment.ClassApplies());

                case ItemGridIntendedAction.Uncurse:
                    return equipment.IsCursed;

                case ItemGridIntendedAction.Transmute:
                    return !metaData.IsObjective;

                default:
                    throw new Exception("Improper use of Single Action Item Grid View Model");
            }
        }

        protected override void Update(IModelService modelService)
        {
            var equipment = modelService.Player.Equipment.Values;

            // Set up public properties for binding based on the intended action
            switch (this.IntendedAction)
            {
                case ItemGridIntendedAction.Equip:
                case ItemGridIntendedAction.Drop:
                case ItemGridIntendedAction.Transmute:
                    SynchronizeCollection(modelService.Player.Equipment.Values, modelService);
                    break;
                case ItemGridIntendedAction.EnchantWeapon:
                    SynchronizeCollection(modelService.Player.Equipment.Values.Where(x => x.IsWeaponType() && x.ClassApplies()), modelService);
                    break;
                case ItemGridIntendedAction.EnchantArmor:
                    SynchronizeCollection(modelService.Player.Equipment.Values.Where(x => x.IsArmorType() && x.ClassApplies()), modelService);
                    break;
                case ItemGridIntendedAction.ImbueArmor:
                    SynchronizeCollection(modelService.Player.Equipment.Values.Where(x => x.IsArmorType() && x.CanImbue()), modelService);
                    break;
                case ItemGridIntendedAction.ImbueWeapon:
                    SynchronizeCollection(modelService.Player.Equipment.Values.Where(x => x.IsWeaponType() && x.CanImbue()), modelService);
                    break;
                case ItemGridIntendedAction.EnhanceWeapon:
                    SynchronizeCollection(modelService.Player.Equipment.Values.Where(x => x.IsWeaponType() && x.ClassApplies()), modelService);
                    break;
                case ItemGridIntendedAction.EnhanceArmor:
                    SynchronizeCollection(modelService.Player.Equipment.Values.Where(x => x.IsArmorType() && x.ClassApplies()), modelService);
                    break;
                case ItemGridIntendedAction.Identify:
                    SynchronizeCollection(modelService.Player
                                                      .Equipment
                                                      .Values
                                                      .Where(x => !modelService.ScenarioEncyclopedia[x.RogueName].IsIdentified ||
                                                                  (!x.IsIdentified &&
                                                                    x.ClassApplies())), modelService);
                    break;
                case ItemGridIntendedAction.Uncurse:
                    SynchronizeCollection(modelService.Player.Equipment.Values.Where(x => x.IsCursed), modelService);
                    break;
                default:
                    throw new Exception("Unknown Item Grid Intended Action");
            }
        }

        /// <summary>
        /// Synchronize a source item collection
        /// </summary>
        /// <param name="source"></param>
        protected void SynchronizeCollection(IEnumerable<Equipment> source, IModelService modelService)
        {
            // Call Unhook to remove item events before synchronizing
            UnHookItems();

            this.Items.SynchronizeFrom(

                // Source
                source.OrderBy(x => x.Type.ToString())
                      .ThenBy(x => x.RogueName),

                // Comparer
                (item, viewModel) => item.Id == viewModel.Id,

                // Constructor
                item => new EquipmentItemGridRowViewModel(
                             item,
                             modelService.ScenarioEncyclopedia[item.RogueName],
                             modelService.GetDisplayName(item),
                             IsItemEnabled(item, modelService)),

                // Updater
                (item, viewModel) =>
                {
                    viewModel.Update(item,
                                    modelService.ScenarioEncyclopedia[item.RogueName],
                                    modelService.GetDisplayName(item),
                                    source.Gather(item, x => x.RogueName),
                                    IsItemEnabled(item, modelService));
                });

            // Call Hook to add listener to events from child items
            HookItems();
        }
    }
}
