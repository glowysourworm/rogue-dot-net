using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration;
using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Assets.SharedControl
{
    [Export]
    public partial class Spell : UserControl
    {
        [ImportingConstructor]
        public Spell()
        {
            InitializeComponent();
        }
    }
}
