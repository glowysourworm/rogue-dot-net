using Rogue.NET.ScenarioEditor.ViewModel;
using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views
{
    [Export]
    public partial class Editor : UserControl
    {
        [ImportingConstructor]
        public Editor(IEditorViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;
        }
    }
}
