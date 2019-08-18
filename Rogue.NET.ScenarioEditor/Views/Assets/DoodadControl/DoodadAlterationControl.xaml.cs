using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.ScenarioEditor.Events;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Doodad;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Interface;
using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Assets.DoodadControl
{
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export]
    public partial class DoodadAlterationControl : UserControl
    {
        [ImportingConstructor]
        public DoodadAlterationControl(IRogueEventAggregator eventAggregator)
        {
            InitializeComponent();

            // Listen for alteration effect changed
            eventAggregator.GetEvent<AlterationEffectChangedEvent>()
                           .Subscribe(e =>
                           {
                               var viewModel = this.DataContext as DoodadAlterationTemplateViewModel;
                               if (viewModel == null)
                                   return;

                               // Type cast the effect interface
                               if (e.Effect is IDoodadAlterationEffectTemplateViewModel &&
                                   e.Alteration == viewModel)
                                   viewModel.Effect = (e.Effect as IDoodadAlterationEffectTemplateViewModel);
                           });
        }
    }
}
