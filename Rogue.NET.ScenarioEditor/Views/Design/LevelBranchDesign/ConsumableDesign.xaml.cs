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
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export]
    public partial class ConsumableDesign : UserControl
    {
        [ImportingConstructor]
        public ConsumableDesign(IRogueEventAggregator eventAggregator, IScenarioCollectionProvider scenarioCollectionProvider)
        {
            InitializeComponent();

            eventAggregator.GetEvent<ScenarioLoadedEvent>()
                           .Subscribe(configurationData =>
                           {
                               this.ConsumableLB.SourceItemsSource = configurationData.Configuration.ConsumableTemplates;
                           });

            this.ConsumableLB.SourceItemsSource = scenarioCollectionProvider.Consumables;
        }

        private void ConsumableLB_AddEvent(object sender, object e)
        {
            var viewModel = this.ConsumableLB.DestinationItemsSource as IList<ConsumableGenerationTemplateViewModel>;
            var consumable = e as ConsumableTemplateViewModel;

            if (viewModel != null &&
                viewModel.None(x => x.Name == consumable.Name))
                viewModel.Add(new ConsumableGenerationTemplateViewModel()
                {
                    Asset = consumable,
                    GenerationWeight = 1.0,
                    Name = consumable.Name
                });
        }

        private void ConsumableLB_RemoveEvent(object sender, object e)
        {
            var viewModel = this.ConsumableLB.DestinationItemsSource as IList<ConsumableGenerationTemplateViewModel>;
            var consumableGeneration = e as ConsumableGenerationTemplateViewModel;

            if (viewModel != null)
                viewModel.Remove(consumableGeneration);
        }
    }
}
