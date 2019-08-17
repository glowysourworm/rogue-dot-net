using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Assets.SharedControl
{
    [PartCreationPolicy(CreationPolicy.NonShared)]
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
