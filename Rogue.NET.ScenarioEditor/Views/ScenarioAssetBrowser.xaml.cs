using Rogue.NET.ScenarioEditor.ViewModel;
using Rogue.NET.ScenarioEditor.ViewModel.Interface;
using System.ComponentModel.Composition;
using System.Windows.Controls;
using System.Windows.Data;

namespace Rogue.NET.ScenarioEditor.Views
{
    [Export]
    public partial class ScenarioAssetBrowser : UserControl
    {
        [ImportingConstructor]
        public ScenarioAssetBrowser(IScenarioAssetBrowserViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;
        }
    }
}
