using Rogue.NET.Common.Extension.Event;
using Rogue.NET.Scenario.Content.ViewModel.ItemGrid;
using Rogue.NET.Scenario.Content.Views.Dialog.Interface;
using Rogue.NET.Scenario.Content.ViewModel.ItemGrid.ItemGridRow;
using Rogue.NET.Scenario.Content.ViewModel.ItemGrid.PrimaryMode;

using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Controls;
using System;


namespace Rogue.NET.Scenario.Content.Views.ItemGrid
{
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export]
    public partial class ConsumableItemGrid : UserControl, IDialogView
    {
        public event SimpleEventHandler<IDialogView> DialogViewFinishedEvent;

        /// <summary>
        /// Importing Constructor - (single item selection default mode)
        /// </summary>
        [ImportingConstructor]
        public ConsumableItemGrid(ConsumablePrimaryItemGridViewModel viewModel)
        {
            this.DataContext = viewModel;

            InitializeComponent();
        }
        
        /// <summary>
        /// Manual constructor - used for dialog / multi-selection mode
        /// </summary>
        public ConsumableItemGrid()
        {
            InitializeComponent();

            this.DataContextChanged += (sender, e) =>
            {
                var viewModel = e.NewValue as ConsumableItemGridViewModelBase;

                // Hook dialog selection event
                if (viewModel != null)
                    viewModel.SingleDialogSelectionEvent += OnSingleDialogSelectionEvent;
            };

            // Implement IDisposable to be good about event aggregator hooks
            // and cleaning up memory for observable collections
            this.Unloaded += (sender, e) =>
            {
                var viewModel = this.DataContext as ConsumableItemGridViewModelBase;

                if (viewModel != null)
                    viewModel.SingleDialogSelectionEvent -= OnSingleDialogSelectionEvent;
            };
        }

        public IEnumerable<string> GetSelectedItemIds()
        {
            var viewModel = this.DataContext as ConsumableItemGridViewModelBase;
            if (viewModel != null)
                return viewModel.GetSelectedItemIds();

            else
                throw new Exception("Miss-handled view-model ConsumableItemGrid");
        }

        private void OnSingleDialogSelectionEvent(ConsumableItemGridRowViewModel item)
        {
            // Fire Dialog Finished Event
            if (this.DialogViewFinishedEvent != null)
                this.DialogViewFinishedEvent(this);
        }
    }
}
