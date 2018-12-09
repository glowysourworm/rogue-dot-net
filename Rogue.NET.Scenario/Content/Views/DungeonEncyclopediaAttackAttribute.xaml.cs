using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace Rogue.NET.Scenario.Content.Views
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
