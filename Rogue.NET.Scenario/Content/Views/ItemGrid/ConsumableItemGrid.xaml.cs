using Rogue.NET.Common.Extension.Event;
using Rogue.NET.Scenario.Content.ViewModel.ItemGrid;
using Rogue.NET.Scenario.Content.Views.Dialog.Interface;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Controls;
using System.Linq;
using Rogue.NET.Common.Extension;
using System;
using Rogue.NET.Core.Model.Scenario.Content.Item;
using Rogue.NET.Scenario.Content.ViewModel.ItemGrid.Enum;

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
        public ConsumableItemGrid(ConsumableItemGridViewModel viewModel)
        {
            this.DataContext = viewModel;

            InitializeComponent();

            // Implement IDisposable to be good about event aggregator hooks
            // and cleaning up memory for observable collections
            this.Unloaded += (sender, e) =>
            {
                if (viewModel != null)
                    viewModel.Dispose();
            };
        }
        
        /// <summary>
        /// Manual constructor - used for dialog / multi-selection mode
        /// </summary>
        public ConsumableItemGrid()
        {
            InitializeComponent();

            this.DataContextChanged += (sender, e) =>
            {
                var viewModel = e.NewValue as ConsumableItemGridViewModel;
                if (viewModel != null)
                    viewModel.SingleDialogSelectionEvent += OnSingleDialogSelectionEvent;
            };

            // Implement IDisposable to be good about event aggregator hooks
            // and cleaning up memory for observable collections
            this.Unloaded += (sender, e) =>
            {
                var viewModel = this.DataContext as ConsumableItemGridViewModel;

                if (viewModel != null)
                {
                    viewModel.SingleDialogSelectionEvent -= OnSingleDialogSelectionEvent;
                    viewModel.Dispose();
                }
            };
        }

        public IEnumerable<string> GetSelectedItemIds()
        {
            var viewModel = this.DataContext as ConsumableItemGridViewModel;
            if (viewModel != null)
                return viewModel.Items
                                .Where(x => x.IsSelected)
                                .Select(x => x.Id)
                                .Actualize();

            else
                throw new Exception("Miss-handled view-model ConsumableItemGrid");
        }

        private void OnSingleDialogSelectionEvent(ItemGridRowViewModel<Consumable> item)
        {
            // Fire Dialog Finished Event
            if (this.DialogViewFinishedEvent != null)
                this.DialogViewFinishedEvent(this);
        }
    }
}
