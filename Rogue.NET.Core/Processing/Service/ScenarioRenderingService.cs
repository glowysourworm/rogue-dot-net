using Rogue.NET.Core.Model;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using Rogue.NET.Core.Processing.Service.Cache.Interface;
using Rogue.NET.Core.Processing.Service.Interface;
using Rogue.NET.Core.Processing.Service.Rendering;

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Rogue.NET.Core.Processing.Service
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(IScenarioRenderingService))]
    public class ScenarioRenderingService : IScenarioRenderingService
    {
        readonly IScenarioResourceService _scenarioResourceService;
        readonly IScenarioBitmapSourceFactory _scenarioBitmapSourceFactory;        

        [ImportingConstructor]
        public ScenarioRenderingService(IScenarioResourceService scenarioResourceService,
                                        IScenarioBitmapSourceFactory scenarioBitmapSourceFactory)
        {
            _scenarioResourceService = scenarioResourceService;
            _scenarioBitmapSourceFactory = scenarioBitmapSourceFactory;
        }

        public WriteableBitmap Render(RenderingSpecification specification)
        {
            var cellWidth = specification.Layers.Max(layer => layer.CellWidth);
            var cellHeight = specification.Layers.Max(layer => layer.CellHeight);

            // Render layout layer to writeable bitmap
            var rendering = new WriteableBitmap((int)(cellWidth * ModelConstants.CellWidth * specification.ZoomFactor),
                                                (int)(cellHeight * ModelConstants.CellHeight * specification.ZoomFactor),
                                                ModelConstants.RenderingDPI,
                                                ModelConstants.RenderingDPI,
                                                PixelFormats.Pbgra32, null);

            var renderingGrid = new List<DrawingImage>[cellWidth, cellHeight];

            // Procedure
            //
            // 1) Order rendering layers by the specified enum 
            // 2) Render all layers in order from bottom-to-top. No alpha blending is to be selected
            // 

            // Order layers
            var orderedLayers = specification.Layers
                                             .OrderBy(layer => layer.RenderingOrder)
                                             .ToList();

            // Create array of symbols to avoid memory allocation during procedure
            var symbolArray = new SymbolDetailsTemplate[orderedLayers.Count];

            // Use symbol count while iterating to know how many are present at the grid location
            var symbolCount = 0;

            using (var bitmapContext = rendering.GetBitmapContext())
            {
                for (int column = 0; column < cellWidth; column++)
                {
                    for (int row = 0; row < cellHeight; row++)
                    {
                        // Reset the symbol count
                        symbolCount = 0;

                        for (int index = 0; index < orderedLayers.Count; index++)
                        {
                            // Retrieve the symbol for this layer
                            var symbol = orderedLayers[index].GetSymbol(column, row);

                            // If there is anything there - add it to the array
                            if (symbol != null)
                            {
                                // Increment indexer / counter
                                symbolArray[symbolCount++] = symbol;
                            }
                        }

                        // Skip empty cells
                        if (symbolCount == 0)
                            continue;

                        var isVisible = specification.IsVisibileCallback(column, row);
                        // var wasVisible = specification.WasVisibileCallback(column, row);
                        var isRevealed = specification.RevealedCallback(column, row);
                        var isExplored = specification.ExploredCallback(column, row);

                        // TODO: Figure out how to deal with invalidation
                        //// Check to see that this location is either NEWLY VISIBLE or WAS VISIBLE LAST TURN
                        ////
                        //if (!isVisible &&
                        //    // !wasVisible &&
                        //    !isRevealed)
                        //    continue;

                        if (!isVisible &&
                            !isExplored &&
                            !isRevealed)
                            continue;

                        // Calculate effective vision
                        var effectiveVision = specification.EffectiveVisionCallback(column, row);

                        // Render the contents of the symbol array
                        for (int index = 0; index < symbolCount; index++)
                        {
                            // Create drawing image from symbol details
                            var image = GetSymbol(symbolArray[index],
                                                    isVisible,
                                                    isExplored,
                                                    isRevealed,
                                                    effectiveVision,
                                                    specification.EffectiveLightingCallback(column, row));

                            // Fetch bitmap from cache
                            var bitmap = _scenarioBitmapSourceFactory.GetImageSource(image, specification.ZoomFactor);

                            // Calculate the rectangle in which to render the image
                            var renderRect = new Rect(column * ModelConstants.CellWidth * specification.ZoomFactor,
                                                      row * ModelConstants.CellHeight * specification.ZoomFactor,
                                                      ModelConstants.CellWidth * specification.ZoomFactor,
                                                      ModelConstants.CellHeight * specification.ZoomFactor);

                            // https://stackoverflow.com/questions/15540996/performance-of-writeablebitmapex
                            using (bitmap.GetBitmapContext())
                            {
                                // Use WriteableBitmapEx extension method to overwrite pixels on the target
                                bitmapContext.WriteableBitmap.Blit(renderRect,
                                                                   bitmap,
                                                                   new Rect(new Size(bitmap.Width, bitmap.Height)),
                                                                   WriteableBitmapExtensions.BlendMode.None);
                            }
                        }
                    }
                }
            }

            return rendering;
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
        /*
        private Light CalculateVisibleIntensity(Light effectiveLighting, GridLocation location, GridLocation playerLocation)
        {
            // Intensity falls off linearly as the vision
            var distance = Metric.EuclideanDistance(location, playerLocation);
            var distanceRatio = distance / ModelConstants.MaxVisibileRadiusPlayer;

            // USE ROUNDING TO PREVENT CACHE OVERLOAD
            var intensity = System.Math.Round(((1 - distanceRatio) * effectiveLighting.Intensity)
                                       .Clip(ModelConstants.MinLightIntensity, 1), 1);

            return new Light(effectiveLighting, intensity);
        }
        */
    }
}
