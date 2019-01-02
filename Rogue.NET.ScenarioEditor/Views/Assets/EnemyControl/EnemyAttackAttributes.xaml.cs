using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Assets.EnemyControl
{
    [Export]
    public partial class EnemyAttackAttributes : UserControl
    {
        [ImportingConstructor]
        public EnemyAttackAttributes()
        {
            InitializeComponent();
        }
    }
}
