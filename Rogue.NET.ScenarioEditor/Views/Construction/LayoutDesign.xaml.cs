using Rogue.NET.ScenarioEditor.ViewModel;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration;
using System.ComponentModel.Composition;
using System.Windows.Controls;
using System.Linq;

namespace Rogue.NET.ScenarioEditor.Views.Construction
{
    [Export]
    public partial class LayoutDesign : UserControl
    {
        [ImportingConstructor]
        public LayoutDesign()
        {
            InitializeComponent();
        }
    }
}
