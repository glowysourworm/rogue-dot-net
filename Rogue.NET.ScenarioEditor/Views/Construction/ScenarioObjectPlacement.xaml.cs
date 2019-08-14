using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Construction
{
    [Export]
    public partial class ScenarioObjectPlacement : UserControl
    {
        [ImportingConstructor]
        public ScenarioObjectPlacement()
        {
            InitializeComponent();
        }
    }
}
