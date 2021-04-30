using Rogue.NET.Common.Extension.Prism.EventAggregator;
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
using System.Text;
using System.Threading.Tasks;
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

        const int LAYOUT_BITMAP_DPI = 96;

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
                                                LAYOUT_BITMAP_DPI,
                                                LAYOUT_BITMAP_DPI,
                                                PixelFormats.Pbgra32, null);

            var renderingGrid = new List<DrawingImage>[cellWidth, cellHeight];

            using (var bitmapContext = rendering.GetBitmapContext())
            {
                for (int column = 0; column < cellWidth; column++)
                {
                    for (int row = 0; row < cellHeight; row++)
                    {
                        // Get topmost layer cell
                        SymbolDetailsTemplate topMostSymbol = null;

                        for (int index = specification.Layers.Count - 1;
                             index >= 0 &&
                             topMostSymbol == null;
                             index--)
                            topMostSymbol = specification.Layers[index].GetSymbol(column, row);

                        // Skip empty cells
                        if (topMostSymbol == null)
                            continue;

                        var isVisible = specification.IsVisibileCallback(column, row);
                        var wasVisible = specification.WasVisibileCallback(column, row);
                        var isRevealed = specification.RevealedCallback(column, row);
                        var isExplored = specification.ExploredCallback(column, row);

                        // Check to see that this location is either NEWLY VISIBLE or WAS VISIBLE LAST TURN
                        //
                        if (!isVisible &&
                            !wasVisible &&
                            !isRevealed)
                            continue;

                        if (!isVisible &&
                            !isExplored &&
                            !isRevealed)
                            continue;

                        // Calculate effective vision
                        var effectiveVision = specification.EffectiveVisionCallback(column, row);

                        // Create drawing image from symbol details
                        var image = GetSymbol(topMostSymbol,
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
    }
}
