using Rogue.NET.ScenarioEditor.Events.Browser;
using Rogue.NET.Common.Extension.Prism.EventAggregator;

using System.ComponentModel.Composition;
using System.Windows.Controls;
using System.Linq;
using Rogue.NET.ScenarioEditor.ViewModel.Browser.Interface;

namespace Rogue.NET.ScenarioEditor.Views.Browser
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export]
    public partial class ScenarioAssetBrowser : UserControl
    {
        [ImportingConstructor]
        public ScenarioAssetBrowser(IScenarioAssetBrowserViewModel viewModel, IRogueEventAggregator eventAggregator)
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
