using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Animation;
using Rogue.NET.ScenarioEditor.Events;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Assets.SharedControl.AnimationControl
{
    [Export]
    public partial class AnimationParameters : UserControl
    {
        [ImportingConstructor]
        public AnimationParameters(IRogueEventAggregator eventAggregator)
        {
            InitializeComponent();
        }
    }
}
