using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.ScenarioEditor.Events;
using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Assets.SharedControl.AlterationControl.EffectControl
{
    [Export]
    public partial class TemporaryEffectParameters : UserControl
    {
        [ImportingConstructor]
        public TemporaryEffectParameters(IRogueEventAggregator eventAggregator)
        {
            InitializeComponent();

            eventAggregator.GetEvent<ScenarioLoadedEvent>()
                           .Subscribe(configuration =>
                           {
                               this.AlteredStateCB.ItemsSource = configuration.AlteredCharacterStates;
                           });
        }
    }
}
