using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Assets.SharedControl.CharacterControl
{
    [Export]
    public partial class CharacterAttackAttributes : UserControl
    {
        [ImportingConstructor]
        public CharacterAttackAttributes()
        {
            InitializeComponent();
        }
    }
}
