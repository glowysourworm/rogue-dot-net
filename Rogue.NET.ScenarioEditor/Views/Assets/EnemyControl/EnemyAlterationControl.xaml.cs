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

            // NOTE*** THIS EVENT WILL UPDATE THE PROPER INSTANCE BECAUSE THERE IS ONLY ONE
            //         INSTANCE OF THIS CONTROL
            eventAggregator.GetEvent<AlterationEffectChangedEvent>()
                           .Subscribe(e =>
                           {
                               var viewModel = this.DataContext as EnemyAlterationTemplateViewModel;
                               if (viewModel == null)
                                   return;

                               // Type cast the effect interface
                               if (e.Effect is IEnemyAlterationEffectTemplateViewModel &&
                                   e.AlterationType == typeof(EnemyAlterationTemplateViewModel))
                                   viewModel.Effect = (e.Effect as IEnemyAlterationEffectTemplateViewModel);
                           });
        }
    }
}
