using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.ScenarioEditor.Events;
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

            this.EffectChooser.AlterationEffectChosen += (sender, e) =>
            {
                var viewModel = this.DataContext as ConsumableProjectileAlterationTemplateViewModel;
                if (viewModel == null)
                    return;

                // TODO:REGIONMANAGER
                // Load new view
                //eventAggregator.GetEvent<AlterationEffectLoadRequestEvent>()
                //               .Publish(new AlterationEffectLoadRequestEventArgs()
                //               {
                //                   AlterationEffectViewType = e.AlterationEffectViewType,
                //                   AlterationEffectType = e.AlterationEffectType
                //               });
            };

            // Subscribe to response event for loading the alteration effect
            eventAggregator.GetEvent<AlterationEffectLoadResponseEvent>()
                           .Subscribe(e =>
                           {
                               var viewModel = this.DataContext as ConsumableProjectileAlterationTemplateViewModel;
                               if (viewModel == null)
                                   return;

                               // Set new effect (to trigger binding)
                               viewModel.Effect = e.AlterationEffect as IConsumableProjectileAlterationEffectTemplateViewModel;
                           });
        }
    }
}
