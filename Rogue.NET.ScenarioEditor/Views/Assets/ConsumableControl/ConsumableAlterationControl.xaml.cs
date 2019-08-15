using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Interface;
using Rogue.NET.ScenarioEditor.Events;
using Rogue.NET.Common.Extension.Prism.EventAggregator;

using System.ComponentModel.Composition;
using System.Windows.Controls;
using System;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Consumable;

namespace Rogue.NET.ScenarioEditor.Views.Assets.ConsumableControl
{
    [Export]
    public partial class ConsumableAlterationControl : UserControl
    {
        [ImportingConstructor]
        public ConsumableAlterationControl(IRogueEventAggregator eventAggregator)
        {
            InitializeComponent();

            // NOTE*** THIS EVENT WILL UPDATE THE PROPER INSTANCE BECAUSE THERE IS ONLY ONE
            //         INSTANCE OF THIS CONTROL
            eventAggregator.GetEvent<AlterationEffectChangedEvent>()
                           .Subscribe(e =>
                           {
                               var viewModel = this.DataContext as ConsumableAlterationTemplateViewModel;
                               if (viewModel == null)
                                   return;

                               // Type cast the effect interface
                               if (e.Effect is IConsumableAlterationEffectTemplateViewModel)
                                   viewModel.Effect = (e.Effect as IConsumableAlterationEffectTemplateViewModel);
                           });
        }
    }
}
