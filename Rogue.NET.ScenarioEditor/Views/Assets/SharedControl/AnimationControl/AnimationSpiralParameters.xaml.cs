using Rogue.NET.Common.Extension.Prism.EventAggregator;
using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Assets.SharedControl.AnimationControl
{
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export]
    public partial class AnimationSpiralParameters : UserControl
    {
        [ImportingConstructor]
        public AnimationSpiralParameters(IRogueEventAggregator eventAggregator)
        {
            InitializeComponent();
        }
    }
}
