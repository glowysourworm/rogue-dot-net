using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Assets.EnemyControl
{
    [Export]
    public partial class EnemyCombatAttributes : UserControl
    {
        [ImportingConstructor]
        public EnemyCombatAttributes()
        {
            InitializeComponent();
        }
    }
}
