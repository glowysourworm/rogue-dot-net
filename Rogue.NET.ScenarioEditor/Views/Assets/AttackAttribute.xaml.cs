using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Assets
{
    [Export]
    public partial class AttackAttribute : UserControl
    {
        [ImportingConstructor]
        public AttackAttribute()
        {
            InitializeComponent();
        }
    }
}
