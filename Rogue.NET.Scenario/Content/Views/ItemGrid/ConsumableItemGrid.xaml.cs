using Rogue.NET.Scenario.Content.ViewModel.ItemGrid;

using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace Rogue.NET.Scenario.Content.Views.ItemGrid
{
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export]
    public partial class ConsumableItemGrid : UserControl
    {
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
    }
}
