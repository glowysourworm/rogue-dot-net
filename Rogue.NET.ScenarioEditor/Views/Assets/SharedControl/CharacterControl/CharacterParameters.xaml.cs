using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Animation;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Content;

using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Assets.SharedControl.CharacterControl
{
    [Export]
    public partial class CharacterParameters : UserControl
    {
        [ImportingConstructor]
        public CharacterParameters(IRogueEventAggregator eventAggregator)
        {
            InitializeComponent();
        }
    }
}
