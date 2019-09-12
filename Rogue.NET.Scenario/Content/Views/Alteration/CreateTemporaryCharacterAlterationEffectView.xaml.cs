using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace Rogue.NET.Scenario.Content.Views.Alteration
{
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export]
    public partial class CreateTemporaryCharacterAlterationEffectView : UserControl
    {
        public CreateTemporaryCharacterAlterationEffectView()
        {
            InitializeComponent();
        }
    }
}
