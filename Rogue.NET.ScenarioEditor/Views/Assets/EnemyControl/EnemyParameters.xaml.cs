using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.ScenarioEditor.Events;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Animation;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Content;

using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Assets.EnemyControl
{
    [Export]
    public partial class EnemyParameters : UserControl
    {
        [ImportingConstructor]
        public EnemyParameters(IRogueEventAggregator eventAggregator)
        {
            InitializeComponent();

            eventAggregator.GetEvent<ScenarioLoadedEvent>().Subscribe((configuration) =>
            {
                // TODO:ALTERATION Add AnimationGroupTemplate for the death animations
                //this.DeathAnimationsLB.SourceItemsSource = configuration.AnimationTemplates;
            });
        }

        private void DeathAnimationsLB_AddEvent(object sender, object e)
        {
            var viewModel = this.DataContext as EnemyTemplateViewModel;
            if (viewModel != null)
                viewModel.DeathAnimations.Add(e as AnimationTemplateViewModel);
        }

        private void DeathAnimationsLB_RemoveEvent(object sender, object e)
        {
            var viewModel = this.DataContext as EnemyTemplateViewModel;
            if (viewModel != null)
                viewModel.DeathAnimations.Remove(e as AnimationTemplateViewModel);
        }
    }
}
