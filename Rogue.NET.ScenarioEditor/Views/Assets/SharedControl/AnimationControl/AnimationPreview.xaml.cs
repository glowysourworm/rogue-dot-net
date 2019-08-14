using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Assets.SharedControl.AnimationControl
{
    [Export]
    public partial class AnimationPreview : UserControl
    {
        [ImportingConstructor]
        public AnimationPreview()
        {
            InitializeComponent();
        }
    }
}
