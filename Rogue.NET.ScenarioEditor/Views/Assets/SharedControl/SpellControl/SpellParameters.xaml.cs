using Prism.Events;
using Rogue.NET.ScenarioEditor.Events;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Animation;
using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Assets.SharedControl.SpellControl
{
    [Export]
    public partial class SpellParameters : UserControl
    {
        [ImportingConstructor]
        public SpellParameters(IEventAggregator eventAggregator)
        {
            InitializeComponent();

            eventAggregator.GetEvent<ScenarioLoadedEvent>().Subscribe(configuration =>
            {
                this.CreateMonsterCB.ItemsSource = configuration.EnemyTemplates;
                this.RemediedStateCB.ItemsSource = configuration.AlteredCharacterStates;
                this.AlteredStateCB.ItemsSource = configuration.AlteredCharacterStates;
                this.AlteredStateAuraCB.ItemsSource = configuration.AlteredCharacterStates;
                this.AnimationsLB.SourceItemsSource = configuration.AnimationTemplates;
            });

            this.AnimationsLB.AddEvent += AnimationsLB_AddEvent;
            this.AnimationsLB.RemoveEvent += AnimationsLB_RemoveEvent;
        }

        private void AnimationsLB_RemoveEvent(object sender, object animation)
        {
            // TODO:ALTERATION
            //var viewModel = this.DataContext as SpellTemplateViewModel;
            //var template = animation as AnimationTemplateViewModel;
            //viewModel.Animations.Remove(template);
        }

        private void AnimationsLB_AddEvent(object sender, object animation)
        {
            // TODO:ALTERATION
            //var viewModel = this.DataContext as SpellTemplateViewModel;
            //var template = animation as AnimationTemplateViewModel;
            //viewModel.Animations.Add(template);
        }
    }
}
