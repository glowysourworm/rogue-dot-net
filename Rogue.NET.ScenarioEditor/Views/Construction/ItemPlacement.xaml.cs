using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Construction
{
    [Export]
    public partial class ItemPlacement : UserControl
    {
        [ImportingConstructor]
        public ItemPlacement()
        {
            InitializeComponent();
        }
    }
}
