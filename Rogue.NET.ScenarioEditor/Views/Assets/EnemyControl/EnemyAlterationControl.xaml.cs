using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.ScenarioEditor.Events;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Enemy;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Interface;
using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Assets.EnemyControl
{
    [Export]
    public partial class EnemyAlterationControl : UserControl
    {
        [ImportingConstructor]
        public EnemyAlterationControl(IRogueEventAggregator eventAggregator)
        {
            InitializeComponent();

            this.EffectChooser.AlterationEffectChosen += (sender, e) =>
            {
                var viewModel = this.DataContext as EnemyAlterationTemplateViewModel;
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
                               var viewModel = this.DataContext as EnemyAlterationTemplateViewModel;
                               if (viewModel == null)
                                   return;

                               // Set new effect (to trigger binding)
                               viewModel.Effect = e.AlterationEffect as IEnemyAlterationEffectTemplateViewModel;
                           });
        }
    }
}
