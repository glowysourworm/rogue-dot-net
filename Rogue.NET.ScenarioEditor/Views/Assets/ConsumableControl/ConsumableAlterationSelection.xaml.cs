using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Assets.ConsumableControl
{
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export]
    public partial class ConsumableAlterationSelection : UserControl
    {
        [ImportingConstructor]
        public ConsumableAlterationSelection()
        {
            InitializeComponent();
        }
    }
}
