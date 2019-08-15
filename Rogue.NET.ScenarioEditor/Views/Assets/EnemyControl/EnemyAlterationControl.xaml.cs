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

            eventAggregator.GetEvent<AlterationEffectChangedEvent>()
                           .Subscribe(e =>
                           {
                               var viewModel = this.DataContext as EnemyAlterationTemplateViewModel;
                               if (viewModel == null)
                                   return;

                               // Check that alteration matches this alteration
                               if (viewModel != e.Alteration)
                                   return;

                               // Type cast the effect interface
                               if (e is IEnemyAlterationEffectTemplateViewModel)
                                   viewModel.Effect = (e as IEnemyAlterationEffectTemplateViewModel);

                               // TODO:  NOT SURE WHY BINDING ISN'T UPDATING! SHOULD NOT HAVE TO FORCE UPDATE
                               // this.DoodadAlterationRegion.SetCurrentValue(DataContextProperty, viewModel.Effect);
                           });
        }
    }
}
