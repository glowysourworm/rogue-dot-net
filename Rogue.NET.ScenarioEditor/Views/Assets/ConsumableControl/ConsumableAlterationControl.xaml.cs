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

            eventAggregator.GetEvent<AlterationEffectChangedEvent>()
                           .Subscribe(e =>
                           {
                               var viewModel = this.DataContext as ConsumableAlterationTemplateViewModel;
                               if (viewModel == null)
                                   return;

                               // Check that alteration matches this alteration
                               // if (viewModel != e.Alteration)
                               //    return;

                               // Type cast the effect interface
                               if (e.Effect is IConsumableAlterationEffectTemplateViewModel)
                                   viewModel.Effect = (e.Effect as IConsumableAlterationEffectTemplateViewModel);

                               // TODO:  NOT SURE WHY BINDING ISN'T UPDATING! SHOULD NOT HAVE TO FORCE UPDATE
                               // this.DoodadAlterationRegion.SetCurrentValue(DataContextProperty, viewModel.Effect);
                           });
        }
    }
}
