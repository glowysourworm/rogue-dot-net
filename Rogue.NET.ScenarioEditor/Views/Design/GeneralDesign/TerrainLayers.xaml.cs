using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.ScenarioEditor.Events.Asset;
using Rogue.NET.ScenarioEditor.Events.Browser;
using Rogue.NET.ScenarioEditor.Utility;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Layout;
using Rogue.NET.ScenarioEditor.Views.Controls.Symbol;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Design.GeneralDesign
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export]
    public partial class TerrainLayers : UserControl
    {
        [ImportingConstructor]
        public TerrainLayers(IRogueEventAggregator eventAggregator)
        {
            InitializeComponent();

            this.AddTerrainLayerButton.Click += (sender, e) =>
            {
                var terrainLayers = this.DataContext as IList<TerrainLayerTemplateViewModel>;

                if (terrainLayers == null || string.IsNullOrEmpty(this.TerrainLayerTB.Text))
                    return;

                if (terrainLayers.Any(x => x.Name == this.TerrainLayerTB.Text))
                    return;

                eventAggregator.GetEvent<AddGeneralAssetEvent>().Publish(new TerrainLayerTemplateViewModel()
                {
                    Name = this.TerrainLayerTB.Text
                });

                this.TerrainLayerTB.Text = "";
            };

            this.RemoveTerrainLayerButton.Click += (sender, e) =>
            {
                var selectedItem = this.TerrainLayerLB.SelectedItem as TerrainLayerTemplateViewModel;

                if (selectedItem != null)
                {
                    eventAggregator.GetEvent<RemoveGeneralAssetEvent>().Publish(selectedItem);
                }
            };
        }

        private void EditSymbolButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;

            if (button != null)
            {
                var viewModel = button.DataContext as TerrainLayerTemplateViewModel;
                if (viewModel != null)
                {
                    DialogWindowFactory.ShowSymbolEditor(viewModel.SymbolDetails);
                }
            }
        }
    }
}
