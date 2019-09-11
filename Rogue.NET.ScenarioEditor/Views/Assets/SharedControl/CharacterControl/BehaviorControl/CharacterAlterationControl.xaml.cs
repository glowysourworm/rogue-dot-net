using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.ScenarioEditor.Events;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Enemy;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Interface;
using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Assets.SharedControl.CharacterControl.BehaviorControl
{
    [Export]
    public partial class CharacterAlterationControl : UserControl
    {
        [ImportingConstructor]
        public CharacterAlterationControl(IRogueEventAggregator eventAggregator)
        {
            InitializeComponent();

            // NOTE*** THIS EVENT WILL UPDATE THE PROPER INSTANCE BECAUSE THERE IS ONLY ONE
            //         INSTANCE OF THIS CONTROL
            eventAggregator.GetEvent<AlterationEffectChangedEvent>()
                           .Subscribe(e =>
                           {
                               var viewModel = this.DataContext as AlterationTemplateViewModel;
                               if (viewModel == null)
                                   return;

                               // Type cast the effect interface
                               if (e.Effect is IEnemyAlterationEffectTemplateViewModel &&
                                   e.Alteration == viewModel)
                                   viewModel.Effect = (e.Effect as IEnemyAlterationEffectTemplateViewModel);

                               else if (e.Effect is IFriendlyAlterationEffectTemplateViewModel &&
                                   e.Alteration == viewModel)
                                   viewModel.Effect = (e.Effect as IFriendlyAlterationEffectTemplateViewModel);

                               else if (e.Effect is ITemporaryCharacterAlterationEffectTemplateViewModel &&
                                   e.Alteration == viewModel)
                                   viewModel.Effect = (e.Effect as ITemporaryCharacterAlterationEffectTemplateViewModel);
                           });
        }
    }
}
