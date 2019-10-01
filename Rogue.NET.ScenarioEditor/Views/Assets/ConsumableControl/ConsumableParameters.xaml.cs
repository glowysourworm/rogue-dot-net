using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.ScenarioEditor.Events;
using Rogue.NET.ScenarioEditor.Service.Interface;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Extension;
using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Assets.ConsumableControl
{
    [Export]
    public partial class ConsumableParameters : UserControl
    {
        [ImportingConstructor]
        public ConsumableParameters(
                IRogueEventAggregator eventAggregator, 
                IScenarioCollectionProvider scenarioCollectionProvider)
        {
            InitializeComponent();
            Initialize(scenarioCollectionProvider);

            eventAggregator.GetEvent<ScenarioUpdateEvent>()
                           .Subscribe(provider =>
                           {
                               Initialize(provider);
                           });
        }

        private void Initialize(IScenarioCollectionProvider provider)
        {
            this.LearnedSkillCB.ItemsSource = provider.SkillSets.CreateDefaultView();
            this.PlayerClassCB.ItemsSource = provider.PlayerClasses.CreateDefaultView();
        }
    }
}
