using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace Rogue.NET.Scenario.Content.Views
{
    [PartCreationPolicy(CreationPolicy.NonShared)]
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
