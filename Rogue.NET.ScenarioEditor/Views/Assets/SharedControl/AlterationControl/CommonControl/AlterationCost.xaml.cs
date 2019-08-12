using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Assets.SharedControl.AlterationControl.CommonControl
{
    [Export]
    public partial class AlterationCost : UserControl
    {
        [ImportingConstructor]
        public AlterationCost()
        {
            InitializeComponent();
        }
    }
}
