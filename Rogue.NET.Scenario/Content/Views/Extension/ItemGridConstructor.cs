using Microsoft.Practices.ServiceLocation;
using Rogue.NET.Scenario.Content.ViewModel.ItemGrid;
using Rogue.NET.Scenario.ViewModel.ItemGrid;
using System;
using System.Windows.Markup;

namespace Rogue.NET.Scenario.Content.Views.Extension
{
    public class ItemGridConstructor : MarkupExtension
    {
        public ItemGridModes Mode { get; set; }
        public ItemGridActions IntendedAction { get; set; }
        public bool IsDialogMode { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            // Get shared item grid view model
            var viewModel = ServiceLocator.Current.GetInstance<ItemGridViewModel>();

            // Get instance of ItemGrid (injects event aggregator)
            var view = ServiceLocator.Current.GetInstance<ItemGrid>();

            view.Mode = this.Mode;
            view.IntendedAction = this.IntendedAction;
            view.IsDialogMode = this.IsDialogMode;

            switch (this.Mode)
            {
                case ItemGridModes.Consumable:
                    view.DataContext = viewModel.Consumables;
                    break;
                case ItemGridModes.EnchantWeapon:
                    view.DataContext = viewModel.EnchantWeaponEquipment;
                    break;
                case ItemGridModes.EnchantArmor:
                    view.DataContext = viewModel.EnchantArmorEquipment;
                    break;
                case ItemGridModes.Equipment:
                    view.DataContext = viewModel.Equipment;
                    break;
                case ItemGridModes.Identify:
                    view.DataContext = viewModel.IdentifyInventory;
                    break;
                case ItemGridModes.ImbueArmor:
                    view.DataContext = viewModel.ImbueArmorEquipment;
                    break;
                case ItemGridModes.ImbueWeapon:
                    view.DataContext = viewModel.ImbueWeaponEquipment;
                    break;
                case ItemGridModes.EnhanceArmor:
                    view.DataContext = viewModel.ImbueArmorEquipment;
                    break;
                case ItemGridModes.EnhanceWeapon:
                    view.DataContext = viewModel.ImbueWeaponEquipment;
                    break;
                case ItemGridModes.Uncurse:
                    view.DataContext = viewModel.UncurseEquipment;
                    break;
                default:
                    throw new Exception("Item Grid Markup Extension - unhandled Item Grid Mode");
            }

            return view;
        }
    }
}
