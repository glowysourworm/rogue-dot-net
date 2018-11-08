using Prism.Events;
using Rogue.NET.ScenarioEditor.Events;
using Rogue.NET.ScenarioEditor.ViewModel.Interface;
using System.ComponentModel.Composition;
using System.Windows.Controls;
using System.Linq;

namespace Rogue.NET.ScenarioEditor.Views
{
    [Export]
    public partial class ScenarioAssetBrowser : UserControl
    {
        [ImportingConstructor]
        public ScenarioAssetBrowser(IScenarioAssetBrowserViewModel viewModel, IEventAggregator eventAggregator)
        {
            InitializeComponent();

            this.DataContext = viewModel;

            // Collapse tree event
            eventAggregator.GetEvent<CollapseAssetTreeEvent>().Subscribe(() =>
            {
                foreach (var item in this.AssetTreeView.Items.Cast<TreeViewItem>())
                    item.IsExpanded = false;
            });
        }
    }
}
