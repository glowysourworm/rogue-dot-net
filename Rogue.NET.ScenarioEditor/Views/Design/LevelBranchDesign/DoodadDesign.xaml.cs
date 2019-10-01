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
    public partial class DoodadDesign : UserControl
    {
        [ImportingConstructor]
        public DoodadDesign(IRogueEventAggregator eventAggregator, IScenarioCollectionProvider scenarioCollectionProvider)
        {
            InitializeComponent();

            eventAggregator.GetEvent<ScenarioLoadedEvent>()
                           .Subscribe(configurationData =>
                           {
                               this.DoodadLB.SourceItemsSource = configurationData.Configuration.DoodadTemplates;
                           });

            this.DoodadLB.SourceItemsSource = scenarioCollectionProvider.Doodads;
        }

        private void DoodadLB_AddEvent(object sender, object e)
        {
            var viewModel = this.DoodadLB.DestinationItemsSource as IList<DoodadGenerationTemplateViewModel>;
            var doodad = e as DoodadTemplateViewModel;

            if (viewModel != null &&
                viewModel.None(x => x.Name == doodad.Name))
                viewModel.Add(new DoodadGenerationTemplateViewModel()
                {
                    Asset = doodad,
                    GenerationWeight = 1.0,
                    Name = doodad.Name
                });
        }

        private void DoodadLB_RemoveEvent(object sender, object e)
        {
            var viewModel = this.DoodadLB.DestinationItemsSource as IList<DoodadGenerationTemplateViewModel>;
            var doodadGeneration = e as DoodadGenerationTemplateViewModel;

            if (viewModel != null)
                viewModel.Remove(doodadGeneration);
        }
    }
}
