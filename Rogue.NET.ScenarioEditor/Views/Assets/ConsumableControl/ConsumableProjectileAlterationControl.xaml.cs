using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.ScenarioEditor.Events;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Consumable;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Interface;
using Rogue.NET.ScenarioEditor.Views.Extension;
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

            eventAggregator.GetEvent<AlterationEffectChangedEvent>()
                           .Subscribe(e =>
                           {
                               var viewModel = this.DataContext as ConsumableProjectileAlterationTemplateViewModel;
                               if (viewModel == null)
                                   return;

                               // Check that alteration matches this alteration
                               if (viewModel != e.Alteration)
                                   return;

                               // Type cast the effect interface
                               if (e is IConsumableProjectileAlterationEffectTemplateViewModel)
                                   viewModel.Effect = (e as IConsumableProjectileAlterationEffectTemplateViewModel);

                               // TODO:  NOT SURE WHY BINDING ISN'T UPDATING! SHOULD NOT HAVE TO FORCE UPDATE
                               // this.DoodadAlterationRegion.SetCurrentValue(DataContextProperty, viewModel.Effect);
                           });
        }
    }
}
