using Rogue.NET.Common.Extension;
using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.Core.Math.Geometry;
using Rogue.NET.Core.Model;
using Rogue.NET.Core.Model.Scenario.Character.Extension;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
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

        WriteableBitmap _rendering;
        bool _firstRendering = true;

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
                // Reset the writeable bitmap
                _rendering = null;
                _firstRendering = true;

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
            if (_rendering == null)
            {
                _rendering = new WriteableBitmap((int)(_modelService.Level.Grid.Bounds.Width * ModelConstants.CellWidth * _modelService.ZoomFactor),
                                                 (int)(_modelService.Level.Grid.Bounds.Height * ModelConstants.CellHeight * _modelService.ZoomFactor),
                                                 LAYOUT_BITMAP_DPI,
                                                 LAYOUT_BITMAP_DPI,
                                                 PixelFormats.Pbgra32, null);

                this.Source = _rendering;
            }

            using (var bitmapContext = _rendering.GetBitmapContext())
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
                        var wasVisible = _modelService.Level.Movement.WasVisible(column, row);

                        // For visible cells - don't render the layout if there is anything on top
                        if (isVisible && _modelService.Level.Content[column, row].Any())
                            continue;

                        // Check to see that this location is either NEWLY VISIBLE or WAS VISIBLE LAST TURN
                        //
                        // TODO: STORE REVEALED-LAST-TURN TO IMPROVE PERFORMANCE
                        //
                        if (!isVisible &&
                            !wasVisible &&
                            !_firstRendering &&
                            !cell.IsRevealed)
                            continue;

                        if (!isVisible &&
                            !cell.IsExplored &&
                            !cell.IsRevealed)
                            continue;

                        // Calculate effective vision
                        var effectiveVision = _modelService.Level.Movement.GetEffectiveVision(column, row);

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
                                                               .Select(layer =>
                                                               {
                                                                   var map = terrainMaps.First(map => map.Name == layer.Name);

                                                                   // Check for region edge
                                                                   if (map[column, row].IsEdge(column, row))
                                                                       return layer.TerrainLayer.EdgeSymbolDetails;

                                                                   else
                                                                       return layer.TerrainLayer.FillSymbolDetails;
                                                               })
                                                               .Actualize();

                            // Fetch images for the terrain
                            terrainImages = terrainSymbols.Select(symbol => GetSymbol(symbol, isVisible, cell.IsExplored, cell.IsRevealed, effectiveVision, cell.Lights))
                                                          .Actualize();
                        }

                        // Doors
                        else if (cell.IsDoor)
                            cellImage = GetSymbol(layoutTemplate.DoorSymbol, isVisible, cell.IsExplored, cell.IsRevealed, effectiveVision, cell.Lights);

                        // Wall Lights
                        else if (cell.IsWallLight)
                            cellImage = GetSymbol(layoutTemplate.WallLightSymbol, isVisible, cell.IsExplored, cell.IsRevealed, effectiveVision, cell.Lights);

                        // Walls
                        else if (cell.IsWall)
                            cellImage = GetSymbol(layoutTemplate.WallSymbol, isVisible, cell.IsExplored, cell.IsRevealed, effectiveVision, cell.Lights);

                        // Walkable Cells
                        else
                            cellImage = GetSymbol(layoutTemplate.CellSymbol, isVisible, cell.IsExplored, cell.IsRevealed, effectiveVision, cell.Lights);

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

            _firstRendering = false;
        }

        private DrawingImage GetSymbol(SymbolDetailsTemplate symbol, bool isVisible, bool isExplored, bool isRevealed, double effectiveVision, params Light[] lighting)
        {
            // Visible
            if (isVisible)
                return _scenarioResourceService.GetImageSource(symbol, 1.0, effectiveVision, lighting);

            // Revealed
            else if (isRevealed)
                return _scenarioResourceService.GetDesaturatedImageSource(symbol, 1.0, 1.0, Light.WhiteRevealed);

            // Explored
            else if (isExplored)
                return _scenarioResourceService.GetImageSource(symbol, 1.0, 1.0, Light.WhiteExplored);

            else
                throw new Exception("Unhandled Exception LevelLayoutImage.GetSymbol");
        }

        private Light CalculateVisibleIntensity(Light effectiveLighting, GridLocation location, GridLocation playerLocation)
        {
            // Intensity falls off linearly as the vision
            var distance = Metric.EuclideanDistance(location, playerLocation);
            var distanceRatio = distance / ModelConstants.MaxVisibileRadius;

            // USE ROUNDING TO PREVENT CACHE OVERLOAD
            var intensity = System.Math.Round(((1 - distanceRatio) * effectiveLighting.Intensity)
                                       .Clip(ModelConstants.MinLightIntensity, 1), 1);

            return new Light(effectiveLighting, intensity);
        }
    }
}
