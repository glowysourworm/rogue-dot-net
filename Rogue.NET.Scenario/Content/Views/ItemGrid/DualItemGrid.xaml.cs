using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.Core.Logic.Processing;
using Rogue.NET.Scenario.Content.ViewModel.ItemGrid;
using Rogue.NET.Scenario.Content.ViewModel.ItemGrid.Enum;
using System;
using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace Rogue.NET.Scenario.Content.Views.ItemGrid
{
    /// <summary>
    /// Component for showing (typically) a user dialog with some kind of return value. This component supports
    /// injection; but - for multiple selection mode - must be hand constructed. The SelectionMode property
    /// MUST BE SET ON BOTH view models in the constructor for multiple selection mode ONLY. It is, by default,
    /// set up for single selection mode.
    /// </summary>
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export]
    public partial class DualItemGrid : UserControl
    {
        [ImportingConstructor]
        public DualItemGrid(EquipmentItemGridViewModel equipmentViewModel,
                            ConsumableItemGridViewModel consumableViewModel,
                            IRogueEventAggregator eventAggregator)
        {
            this.EquipmentRegion.DataContext = equipmentViewModel;
            this.ConsumablesRegion.DataContext = consumableViewModel;

            InitializeComponent();

            // PRIMARY EVENT FOR MULTIPLE SELECTION MODE
            this.OkMultiSelectionButton.Click += (sender, e) =>
            {
                switch (equipmentViewModel.IntendedAction)
                {
                    default:
                        throw new Exception("Unknown Mulitple Selection Intended Grid Action");
                }
            };

            // Clean up event aggregator handles
            this.Unloaded += (sender, e) =>
            {
                if (equipmentViewModel != null)
                    equipmentViewModel.Dispose();

                if (consumableViewModel != null)
                    consumableViewModel.Dispose();
            };
        }
    }
}
