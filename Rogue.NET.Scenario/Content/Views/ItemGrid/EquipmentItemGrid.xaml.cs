using Rogue.NET.Common.Extension.Event;
using Rogue.NET.Scenario.Content.ViewModel.ItemGrid;
using Rogue.NET.Scenario.Content.ViewModel.ItemGrid.ItemGridRow;
using Rogue.NET.Scenario.Content.ViewModel.ItemGrid.PrimaryMode;
using Rogue.NET.Scenario.Content.Views.Dialog.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace Rogue.NET.Scenario.Content.Views.ItemGrid
{
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export]
    public partial class EquipmentItemGrid : UserControl, IDialogView
    {
        public event SimpleEventHandler<IDialogView> DialogViewFinishedEvent;

        /// <summary>
        /// Importing Constructor - (single item selection default mode)
        /// </summary>
        [ImportingConstructor]
        public EquipmentItemGrid(EquipmentPrimaryItemGridViewModel viewModel)
        {
            this.DataContext = viewModel;

            InitializeComponent();
        }

        /// <summary>
        /// Manual constructor - used for dialog / multi-selection mode
        /// </summary>
        public EquipmentItemGrid()
        {
            InitializeComponent();

            this.DataContextChanged += (sender, e) =>
            {
                var viewModel = e.NewValue as EquipmentItemGridViewModelBase;
                if (viewModel != null)
                    viewModel.SingleDialogSelectionEvent += OnSingleDialogSelectionEvent;
            };

            // Implement IDisposable to be good about event aggregator hooks
            // and cleaning up memory for observable collections
            this.Unloaded += (sender, e) =>
            {
                var viewModel = this.DataContext as EquipmentItemGridViewModelBase;

                if (viewModel != null)
                    viewModel.SingleDialogSelectionEvent -= OnSingleDialogSelectionEvent;
            };
        }

        public IEnumerable<string> GetSelectedItemIds()
        {
            var viewModel = this.DataContext as EquipmentItemGridViewModelBase;
            if (viewModel != null)
                return viewModel.GetSelectedItemIds();

            else
                throw new Exception("Miss-handled view-model EquipmentItemGrid");
        }

        private void OnSingleDialogSelectionEvent(EquipmentItemGridRowViewModel item)
        {
            // Fire Dialog Finished Event
            if (this.DialogViewFinishedEvent != null)
                this.DialogViewFinishedEvent(this);
        }
    }
}
