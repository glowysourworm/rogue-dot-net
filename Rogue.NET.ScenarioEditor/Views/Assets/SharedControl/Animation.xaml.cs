using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Assets.SharedControl
{
    [Export]
    public partial class Animation : UserControl
    {
        [ImportingConstructor]
        public Animation()
        {
            InitializeComponent();
        }
    }
}
