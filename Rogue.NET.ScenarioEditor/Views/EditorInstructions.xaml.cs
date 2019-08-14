using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views
{
    [Export]
    public partial class EditorInstructions : UserControl
    {
        [ImportingConstructor]
        public EditorInstructions()
        {
            InitializeComponent();
        }
    }
}
