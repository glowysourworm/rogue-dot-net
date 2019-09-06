using Rogue.NET.Common.Extension;
using Rogue.NET.Common.Extension.Event;
using Rogue.NET.Core.Model.Scenario.Content.Item;
using Rogue.NET.Scenario.Content.ViewModel.ItemGrid;
using Rogue.NET.Scenario.Content.ViewModel.ItemGrid.ItemGridRow;
using Rogue.NET.Scenario.Content.Views.Dialog.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace Rogue.NET.Scenario.Content.Views.ItemGrid
{
    /// <summary>
    /// Component for showing (typically) a user dialog with some kind of return value. 
    /// </summary>
    public partial class DualItemGrid : UserControl, IDialogView
    {
        public event SimpleEventHandler<IDialogView> DialogViewFinishedEvent;

        public DualItemGrid(EquipmentItemGridViewModelBase equipmentViewModel,
                            ConsumableItemGridViewModelBase consumableViewModel)
        {
            InitializeComponent();

            this.EquipmentGrid.DataContext = equipmentViewModel;
            this.ConsumablesGrid.DataContext = consumableViewModel;

            // Event for listening for selection during single selection mode
            equipmentViewModel.SingleDialogSelectionEvent += OnEquipmentSingleDialogEvent;
            consumableViewModel.SingleDialogSelectionEvent += OnConsumableSingleDialogEvent;

            // PRIMARY EVENT FOR MULTIPLE SELECTION MODE
            this.OkMultiSelectionButton.Click += (sender, e) =>
            {
                // This will only fire when there is at LEAST ONE ITEM SELECTED
                if (this.DialogViewFinishedEvent != null)
                    this.DialogViewFinishedEvent(this);
            };
        }

        public IEnumerable<string> GetSelectedItemIds()
        {
            var equipmentViewModel = this.EquipmentGrid.DataContext as EquipmentItemGridViewModelBase;
            var consumableViewModel = this.ConsumablesGrid.DataContext as ConsumableItemGridViewModelBase;

            return equipmentViewModel
                    .Items
                    .Where(x => x.IsSelected)
                    .Select(x => x.Id)
                    .Union(consumableViewModel
                            .Items
                            .Where(x => x.IsSelected)
                            .Select(x => x.Id))
                            .Actualize();
        }

        private void OnEquipmentSingleDialogEvent(EquipmentItemGridRowViewModel sender)
        {
            // An item has been selected in single selection mode
            if (this.DialogViewFinishedEvent != null)
                this.DialogViewFinishedEvent(this);
        }
        private void OnConsumableSingleDialogEvent(ConsumableItemGridRowViewModel sender)
        {
            // An item has been selected in single selection mode
            if (this.DialogViewFinishedEvent != null)
                this.DialogViewFinishedEvent(this);
        }
    }
}
