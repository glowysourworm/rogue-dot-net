using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Assets.SharedControl.SpellControl
{
    [Export]
    public partial class SpellEffectAttackAttributes : UserControl
    {
        [ImportingConstructor]
        public SpellEffectAttackAttributes()
        {
            InitializeComponent();
        }
    }
}
