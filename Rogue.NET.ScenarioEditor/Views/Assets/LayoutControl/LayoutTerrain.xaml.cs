using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.ScenarioEditor.Events;
using Rogue.NET.ScenarioEditor.Service.Interface;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Layout;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Assets.LayoutControl
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export]
    public partial class LayoutTerrain : UserControl
    {
        [ImportingConstructor]
        public LayoutTerrain(IRogueEventAggregator eventAggregator, IScenarioCollectionProvider scenarioCollectionProvider)
        {
            InitializeComponent();

            eventAggregator.GetEvent<ScenarioUpdateEvent>().Subscribe(collectionProvider =>
            {
                this.TerrainLayerLB.SourceItemsSource = collectionProvider.TerrainLayers;
            });

            this.TerrainLayerLB.SourceItemsSource = scenarioCollectionProvider.TerrainLayers;
        }

        private void TerrainLayerLB_AddEvent(object sender, object e)
        {
            var viewModel = this.DataContext as IList<TerrainLayerGenerationTemplateViewModel>;
            var layer = e as TerrainLayerTemplateViewModel;

            if (viewModel != null &&
                layer != null)
            {
                if (!viewModel.Any(item => item.Name == layer.Name))
                    viewModel.Add(new TerrainLayerGenerationTemplateViewModel()
                    {
                        Name = layer.Name,
                        TerrainLayer = layer
                    });
            }
        }

        private void TerrainLayerLB_RemoveEvent(object sender, object e)
        {
            var viewModel = this.DataContext as IList<TerrainLayerGenerationTemplateViewModel>;

            if (viewModel != null)
                viewModel.Remove(e as TerrainLayerGenerationTemplateViewModel);
        }
    }
}
