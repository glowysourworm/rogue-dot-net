using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using Rogue.NET.Core.Model.ScenarioConfiguration.Layout;
using Rogue.NET.Core.Processing.Model.Generator.Interface;
using Rogue.NET.Core.Processing.Service.Interface;
using Rogue.NET.Core.Processing.Service.Rendering;
using Rogue.NET.ScenarioEditor.Service.Interface;

using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;

using static System.Windows.Media.Imaging.WriteableBitmapExtensions;

namespace Rogue.NET.ScenarioEditor.Service
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(IPreviewRenderingService))]
    public class PreviewRenderingService : IPreviewRenderingService
    {
        readonly IScenarioRenderingService _scenarioRenderingService;

        [ImportingConstructor]
        public PreviewRenderingService(IScenarioRenderingService scenarioRenderingService)
        {
            _scenarioRenderingService = scenarioRenderingService;
        }

        public WriteableBitmap CombineRendering(WriteableBitmap lowerLayerRendering, WriteableBitmap upperLayerRendering, BlendMode blendMode)
        {
            // Trying to use WriteableBitmap.BlitRender wasn't allowing overlay of two bitmaps without blanking out pixels
            using (var lowerContext = lowerLayerRendering.GetBitmapContext())
            {
                var bounds = new Rect(new Size(upperLayerRendering.PixelWidth, upperLayerRendering.PixelHeight));

                lowerContext.WriteableBitmap
                            .Blit(bounds, upperLayerRendering, bounds, blendMode);
            }

            return lowerLayerRendering;
        }

        public WriteableBitmap RenderLayoutLayer(LayoutGrid layoutGrid, LayoutGrid.LayoutLayer layer, SymbolDetailsTemplate symbol, bool useLighting)
        {
            // Caller is responsible for ordering the rendering - so set terrain layer to "Ground"
            var renderingLayer = new RenderingLayoutLayer(layoutGrid.SelectLayer(layer), symbol, TerrainLayer.Ground);

            return Render(layoutGrid, new RenderingLayer[] { renderingLayer }, useLighting);
        }

        public WriteableBitmap RenderTerrainLayer(LayoutGrid layoutGrid, TerrainLayerTemplate terrainLayerTemplate, bool useLighting)
        {
            // Get the terrain layer map from the layout grid
            var terrainMap = layoutGrid.TerrainMaps.First(map => map.Name == terrainLayerTemplate.Name);

            // Caller is responsible for ordering the rendering - so set terrain layer to "Ground"
            var renderingLayer = new RenderingLayoutLayer(terrainMap, terrainLayerTemplate.FillSymbolDetails, terrainLayerTemplate.EdgeSymbolDetails, true, TerrainLayer.Ground);

            return Render(layoutGrid, new RenderingLayer[] { renderingLayer }, useLighting);
        }

        public WriteableBitmap RenderWallLightLayer(LayoutGrid layoutGrid, SymbolDetailsTemplate symbol, bool useLighting)
        {
            var renderingLayer = new RenderingContentLayer(layoutGrid.Bounds.Width,
                                                           layoutGrid.Bounds.Height,

                                                           // Wall Lights
                                                           (column, row) =>
                                                           {
                                                               var cell = layoutGrid[column, row];

                                                               if (cell != null &&
                                                                   cell.IsWallLight)
                                                                   return symbol;

                                                               return null;

                                                           }, TerrainLayer.Ground);

            return Render(layoutGrid, new RenderingLayer[] { renderingLayer }, useLighting);
        }

        private WriteableBitmap Render(LayoutGrid layoutGrid, IReadOnlyList<RenderingLayer> renderingLayers, bool useLighting)
        {
            // Create Rendering! :)
            return _scenarioRenderingService.Render(new RenderingSpecification(renderingLayers,
            (column, row) => true,                        // Is Visibile Callback                
            (column, row) => true,                        // Was Visible Callback                
            (column, row) => 1.0,                         // Effective Vision Callback                
            (column, row) => false,                       // Revealed Callback                
            (column, row) => true,                        // Explored Callback                
            (column, row) => useLighting ? layoutGrid[column, row].Lights : new Light[] { Light.White },  // Lighting Callback
            2.0D));
        }
    }
}
