using Rogue.NET.Common.Extension;
using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.ScenarioEditor.Events;
using Rogue.NET.ScenarioEditor.Service.Interface;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Content;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Design;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Rogue.NET.ScenarioEditor.Views.Design.LevelBranchDesign
{
    [Export]
    public partial class EnemyDesign : UserControl
    {
        [ImportingConstructor]
        public EnemyDesign(IRogueEventAggregator eventAggregator, IScenarioCollectionProvider scenarioCollectionProvider)
        {
            InitializeComponent();

            eventAggregator.GetEvent<ScenarioLoadedEvent>()
                           .Subscribe(configurationData =>
                           {
                               this.EnemyLB.SourceItemsSource = configurationData.Configuration.EnemyTemplates;
                           });

            this.EnemyLB.SourceItemsSource = scenarioCollectionProvider.Enemies;
        }

        private void EnemyLB_AddEvent(object sender, object e)
        {
            var viewModel = this.EnemyLB.DestinationItemsSource as IList<EnemyGenerationTemplateViewModel>;
            var enemy = e as EnemyTemplateViewModel;

            if (viewModel != null &&
                viewModel.None(x => x.Name == enemy.Name))
                viewModel.Add(new EnemyGenerationTemplateViewModel()
                {
                    Asset = enemy,
                    GenerationWeight = 1.0,
                    Name = enemy.Name
                });
        }

        private void EnemyLB_RemoveEvent(object sender, object e)
        {
            var viewModel = this.EnemyLB.DestinationItemsSource as IList<EnemyGenerationTemplateViewModel>;
            var enemyGeneration = e as EnemyGenerationTemplateViewModel;

            if (viewModel != null)
                viewModel.Remove(enemyGeneration);
        }
    }
}
