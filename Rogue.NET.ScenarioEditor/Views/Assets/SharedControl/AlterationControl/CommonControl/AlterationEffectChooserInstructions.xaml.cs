using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Assets.SharedControl.AlterationControl.CommonControl
{
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export]
    public partial class AlterationEffectChooserInstructions : UserControl
    {
        public AlterationEffectChooserInstructions()
        {
            InitializeComponent();
        }
    }
}
