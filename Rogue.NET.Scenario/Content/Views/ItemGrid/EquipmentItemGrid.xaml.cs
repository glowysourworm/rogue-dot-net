using Rogue.NET.Common.Extension;
using Rogue.NET.Common.Extension.Event;
using Rogue.NET.Core.Model.Scenario.Content.Item;
using Rogue.NET.Scenario.Content.ViewModel.ItemGrid;
using Rogue.NET.Scenario.Content.Views.Dialog.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
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
        public EquipmentItemGrid(EquipmentItemGridViewModel viewModel)
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
        public EquipmentItemGrid()
        {
            InitializeComponent();

            this.DataContextChanged += (sender, e) =>
            {
                var viewModel = e.NewValue as EquipmentItemGridViewModel;
                if (viewModel != null)
                    viewModel.SingleDialogSelectionEvent += OnSingleDialogSelectionEvent;
            };

            // Implement IDisposable to be good about event aggregator hooks
            // and cleaning up memory for observable collections
            this.Unloaded += (sender, e) =>
            {
                var viewModel = this.DataContext as EquipmentItemGridViewModel;

                if (viewModel != null)
                {
                    viewModel.SingleDialogSelectionEvent -= OnSingleDialogSelectionEvent;
                    viewModel.Dispose();
                }
            };
        }

        public IEnumerable<string> GetSelectedItemIds()
        {
            var viewModel = this.DataContext as EquipmentItemGridViewModel;
            if (viewModel != null)
                return viewModel.Items
                                .Where(x => x.IsSelected)
                                .Select(x => x.Id)
                                .Actualize();

            else
                throw new Exception("Miss-handled view-model EquipmentItemGrid");
        }

        private void OnSingleDialogSelectionEvent(ItemGridRowViewModel<Equipment> item)
        {
            // Fire Dialog Finished Event
            if (this.DialogViewFinishedEvent != null)
                this.DialogViewFinishedEvent(this);
        }
    }
}
