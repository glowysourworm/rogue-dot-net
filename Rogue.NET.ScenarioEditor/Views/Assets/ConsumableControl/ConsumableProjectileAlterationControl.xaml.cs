using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.ScenarioEditor.Events;
using Rogue.NET.ScenarioEditor.Events.Asset.Alteration;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Consumable;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Interface;
using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Assets.ConsumableControl
{
    [Export]
    public partial class ConsumableProjectileAlterationControl : UserControl
    {
        [ImportingConstructor]
        public ConsumableProjectileAlterationControl(IRogueEventAggregator eventAggregator)
        {
            InitializeComponent();

            // NOTE*** THIS EVENT WILL UPDATE THE PROPER INSTANCE BECAUSE THERE IS ONLY ONE
            //         INSTANCE OF THIS CONTROL
            eventAggregator.GetEvent<AlterationEffectChangedEvent>()
                           .Subscribe(e =>
                           {
                               var viewModel = this.DataContext as ConsumableProjectileAlterationTemplateViewModel;
                               if (viewModel == null)
                                   return;

                               // Type cast the effect interface
                               if (e.Effect is IConsumableProjectileAlterationEffectTemplateViewModel &&
                                   e.Alteration == viewModel)
                                   viewModel.Effect = (e.Effect as IConsumableProjectileAlterationEffectTemplateViewModel);
                           });
        }
    }
}
