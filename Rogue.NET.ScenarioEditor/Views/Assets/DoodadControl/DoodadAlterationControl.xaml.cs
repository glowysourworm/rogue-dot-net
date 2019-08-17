using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.ScenarioEditor.Events;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Doodad;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Interface;
using Rogue.NET.ScenarioEditor.Views.Extension;
using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Assets.DoodadControl
{
    [Export]
    public partial class DoodadAlterationControl : UserControl
    {
        [ImportingConstructor]
        public DoodadAlterationControl(IRogueEventAggregator eventAggregator)
        {
            InitializeComponent();

            // NOTE*** THIS EVENT WILL UPDATE THE PROPER INSTANCE BECAUSE THERE IS ONLY ONE
            //         INSTANCE OF THIS CONTROL
            eventAggregator.GetEvent<AlterationEffectChangedEvent>()
                           .Subscribe(e =>
                           {
                               var viewModel = this.DataContext as DoodadAlterationTemplateViewModel;
                               if (viewModel == null)
                                   return;

                               // Type cast the effect interface
                               if (e.Effect is IDoodadAlterationEffectTemplateViewModel)
                                   viewModel.Effect = (e.Effect as IDoodadAlterationEffectTemplateViewModel);
                           });
        }
    }
}
