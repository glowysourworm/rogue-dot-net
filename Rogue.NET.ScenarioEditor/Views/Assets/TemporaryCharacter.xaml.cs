using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Assets
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export]
    public partial class TemporaryCharacter : UserControl
    {
        public TemporaryCharacter()
        {
            InitializeComponent();
        }
    }
}
