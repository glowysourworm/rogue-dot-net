using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Design
{
    [Export]
    public partial class ObjectiveDesign : UserControl
    {
        [ImportingConstructor]
        public ObjectiveDesign()
        {
            InitializeComponent();
        }
    }
}
