using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Assets.EquipmentControl
{
    [Export]
    public partial class EquipmentMetadata : UserControl
    {
        [ImportingConstructor]
        public EquipmentMetadata()
        {
            InitializeComponent();
        }
    }
}
