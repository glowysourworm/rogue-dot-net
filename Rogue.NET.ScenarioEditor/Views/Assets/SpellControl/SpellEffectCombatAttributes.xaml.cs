using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Assets.SpellControl
{
    [Export]
    public partial class SpellEffectCombatAttributes : UserControl
    {
        [ImportingConstructor]
        public SpellEffectCombatAttributes()
        {
            InitializeComponent();
        }
    }
}
