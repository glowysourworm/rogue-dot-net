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

            eventAggregator.GetEvent<AlterationEffectChangedEvent>()
                           .Subscribe(e =>
                           {
                               var viewModel = this.DataContext as DoodadAlterationTemplateViewModel;
                               if (viewModel == null)
                                   return;

                               // Check that alteration matches this alteration
                               if (viewModel != e.Alteration)
                                   return;

                               // Type cast the effect interface
                               if (e is IDoodadAlterationEffectTemplateViewModel)
                                   viewModel.Effect = (e as IDoodadAlterationEffectTemplateViewModel);

                               // TODO:  NOT SURE WHY BINDING ISN'T UPDATING! SHOULD NOT HAVE TO FORCE UPDATE
                               // this.DoodadAlterationRegion.SetCurrentValue(DataContextProperty, viewModel.Effect);
                           });
        }
    }
}
