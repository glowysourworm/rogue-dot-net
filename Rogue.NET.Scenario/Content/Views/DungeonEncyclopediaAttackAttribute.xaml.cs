using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace Rogue.NET.Scenario.Views
{
    [Export]
    public partial class DungeonEncyclopediaAttackAttributeCtrl : UserControl
    {
        [ImportingConstructor]
        public DungeonEncyclopediaAttackAttributeCtrl()
        {
            InitializeComponent();
        }
    }
}
