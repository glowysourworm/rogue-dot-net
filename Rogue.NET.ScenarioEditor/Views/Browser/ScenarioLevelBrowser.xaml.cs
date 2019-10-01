using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.ScenarioEditor.Events.Browser;
using Rogue.NET.ScenarioEditor.ViewModel.Browser.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Rogue.NET.ScenarioEditor.Views.Browser
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export]
    public partial class ScenarioLevelBrowser : UserControl
    {
        [ImportingConstructor]
        public ScenarioLevelBrowser(IRogueEventAggregator eventAggregator, IScenarioLevelBrowserViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;

            // Collapse tree event
            eventAggregator.GetEvent<CollapseLevelTreeEvent>().Subscribe(() =>
            {
                // Levels
                for(int i=0;i<this.LevelTreeView.Items.Count;i++)
                {
                    // Get tree view item from index
                    var item = this.LevelTreeView.ItemContainerGenerator.ContainerFromIndex(i) as TreeViewItem;

                    item.IsExpanded = false;

                    // TODO: Fix nested collapse

                    //// Level Branches
                    //foreach (var branchItem in item.Items)
                    //{
                    //    // Get tree view item from nested index
                    //    var branchTreeViewItem = item.ItemContainerGenerator.ContainerFromItem(branchItem) as TreeViewItem;

                    //    branchTreeViewItem.IsExpanded = false;

                    //    // Assets
                    //    foreach (var assetItem in branchTreeViewItem.Items.Cast<TreeViewItem>())
                    //        assetItem.IsExpanded = false;
                    //}
                }
            });
        }
    }
}
