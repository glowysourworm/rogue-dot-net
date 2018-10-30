using Rogue.NET.ScenarioEditor.ViewModel;
using System.ComponentModel.Composition;
using System.Windows.Controls;

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
