using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Assets.EquipmentControl
{

    [Export]
    public partial class EquipmentCombatAttributes : UserControl
    {
        [ImportingConstructor]
        public EquipmentCombatAttributes()
        {
            InitializeComponent();
        }
    }
}
