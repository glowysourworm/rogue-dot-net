using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Assets.EquipmentControl
{
    [Export]
    public partial class EquipmentSymbol : UserControl
    {
        [ImportingConstructor]
        public EquipmentSymbol()
        {
            InitializeComponent();
        }
    }
}
