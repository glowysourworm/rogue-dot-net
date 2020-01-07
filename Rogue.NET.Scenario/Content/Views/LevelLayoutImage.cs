using Rogue.NET.Common.Extension;
using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.Core.Model;
using Rogue.NET.Core.Processing.Event.Level;
using Rogue.NET.Core.Processing.Service.Cache.Interface;
using Rogue.NET.Core.Processing.Service.Interface;
using Rogue.NET.Scenario.Content.ViewModel.LevelCanvas.Inteface;
using Rogue.NET.Scenario.Processing.Event.Content;
using Rogue.NET.Scenario.Processing.Service.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Rogue.NET.Scenario.Content.Views
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export]
    public class LevelLayoutImage : Image
    {
        readonly IModelService _modelService;
        readonly IScenarioUIService _scenarioUIService;
        readonly IScenarioResourceService _scenarioResourceService;
        readonly IScenarioBitmapSourceFactory _scenarioBitmapSourceFactory;

        const int LAYOUT_BITMAP_DPI = 96;

        [ImportingConstructor]
        public LevelLayoutImage(IModelService modelService,                 
                                ILevelCanvasViewModel levelCanvasViewModel,
                                IScenarioUIService scenarioUIService,
                                IRogueEventAggregator eventAggregator,
                                IScenarioResourceService scenarioResourceService, 
                                IScenarioBitmapSourceFactory scenarioBitmapSourceFactory)
        {
            _modelService = modelService;
            _scenarioUIService = scenarioUIService;
            _scenarioResourceService = scenarioResourceService;
            _scenarioBitmapSourceFactory = scenarioBitmapSourceFactory;

            eventAggregator.GetEvent<LevelLoadedEvent>().Subscribe(() =>
            {
                RenderLayout();
            });

            eventAggregator.GetEvent<ZoomEvent>().Subscribe(eventData =>
            {
                RenderLayout();
            });

            levelCanvasViewModel.VisibilityUpdated += RenderLayout;
        }

        public void RenderLayout()
        {
            var layoutTemplate = _modelService.GetLayoutTemplate();
            var layoutGrid = _modelService.Level.Grid;

            // Render layout layer to writeable bitmap
            var layoutBitmap = new WriteableBitmap((int)(_modelService.Level.Grid.Bounds.Width * ModelConstants.CellWidth * _modelService.ZoomFactor),
                                                   (int)(_modelService.Level.Grid.Bounds.Height * ModelConstants.CellHeight * _modelService.ZoomFactor),
                                                   LAYOUT_BITMAP_DPI,
                                                   LAYOUT_BITMAP_DPI,
                                                   PixelFormats.Pbgra32, null);

            using (var bitmapContext = layoutBitmap.GetBitmapContext())
            {
                for (int column = 0; column < _modelService.Level.Grid.Bounds.Width; column++)
                {
                    for (int row = 0; row < _modelService.Level.Grid.Bounds.Width; row++)
                    {
                        // Skip empty cells
                        if (layoutGrid[column, row] == null)
                            continue;

                        var cell = layoutGrid[column, row];
                        var isVisible = _modelService.Level.Movement.IsVisible(column, row);

                        // For visible cells - don't render the layout if there is anything on top
                        if (isVisible && _modelService.Level.Content[column, row].Any())
                            continue;

                        var lighting = isVisible ? cell.EffectiveLighting :
                                                   cell.IsRevealed ?
                                                   _scenarioUIService.CreateRevealedLight(cell.BaseLight) :
                                                   _scenarioUIService.CreateExploredLight(cell.BaseLight);

                        DrawingImage cellImage = null;
                        IEnumerable<DrawingImage> terrainImages = null;

                        // Terrain - Render using the terrain template
                        if (layoutGrid.TerrainMaps.Any(map => map[column, row] != null))
                        {
                            // Get the terrain layer names
                            var terrainMaps = layoutGrid.TerrainMaps.Where(map => map[column, row] != null);
                            var layerNames = terrainMaps.Select(map => map.Name);

                            // Select the layer templates from the configuration data
                            var layerTemplates = layoutTemplate.TerrainLayers.Where(layer => layerNames.Contains(layer.TerrainLayer.Name));

                            // ORDER LAYERS BY Z-INDEX (see TerrainLayer enum)
                            var terrainSymbols = layerTemplates.OrderBy(x => x.TerrainLayer.Layer)
                                                               .Select(layer => layer.TerrainLayer.SymbolDetails)
                                                               .Actualize();

                            // Fetch images for the terrain
                            terrainImages = terrainSymbols.Select(symbol => _scenarioResourceService.GetImageSource(symbol, 1.0, lighting));
                        }

                        // Doors
                        else if (cell.IsDoor)
                            cellImage = _scenarioResourceService.GetImageSource(layoutTemplate.DoorSymbol, 1.0, lighting);

                        // Wall Lights
                        else if (cell.IsWallLight)
                            cellImage = _scenarioResourceService.GetImageSource(layoutTemplate.DoorSymbol, 1.0, lighting);

                        // Walls
                        else if (cell.IsWall)
                            cellImage = _scenarioResourceService.GetImageSource(layoutTemplate.WallSymbol, 1.0, lighting);

                        // Walkable Cells
                        else
                            cellImage = _scenarioResourceService.GetImageSource(layoutTemplate.CellSymbol, 1.0, lighting);

                        // Render the DrawingImage to a bitmap (from cache) and copy pixels to the rendering target
                        var cellImages = terrainImages ?? new DrawingImage[] { cellImage };

                        if (terrainImages != null || 
                            cellImage != null)
                        {
                            // USE ALPHA BLENDING TO RENDER ALL LAYERS OF THE LAYOUT TOGETHER
                            foreach (var image in cellImages)
                            {
                                // Fetch bitmap from cache
                                var bitmap = _scenarioBitmapSourceFactory.GetImageSource(image, _modelService.ZoomFactor);

                                // Calculate the rectangle in which to render the image
                                var renderRect = new Rect(column * ModelConstants.CellWidth * _modelService.ZoomFactor,
                                                          row * ModelConstants.CellHeight * _modelService.ZoomFactor,
                                                          ModelConstants.CellWidth * _modelService.ZoomFactor,
                                                          ModelConstants.CellHeight * _modelService.ZoomFactor);

                                // Use WriteableBitmapEx extension method to overwrite pixels on the target
                                bitmapContext.WriteableBitmap.Blit(renderRect,
                                                                   bitmap,
                                                                   new Rect(new Size(bitmap.Width, bitmap.Height)), WriteableBitmapExtensions.BlendMode.Alpha);
                            }
                        }
                    }
                }
            }

            this.Source = layoutBitmap;
        }
    }
}
