using System.ComponentModel.Composition;
using System.Windows.Controls;
using System.Windows.Data;

namespace Rogue.NET.ScenarioEditor.Views.Assets.ConsumableControl
{
    [Export]
    public partial class ConsumableType : UserControl
    {
        [ImportingConstructor]
        public ConsumableType()
        {
            InitializeComponent();
        }
    }
}
