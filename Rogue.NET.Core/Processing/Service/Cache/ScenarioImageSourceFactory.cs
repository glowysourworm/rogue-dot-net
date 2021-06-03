using Rogue.NET.Common.Collection;
using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Media.SymbolEffect;
using Rogue.NET.Core.Media.SymbolEffect.Interface;
using Rogue.NET.Core.Media.SymbolEffect.Utility;
using Rogue.NET.Core.Model;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using Rogue.NET.Core.Processing.Service.Cache.Interface;
using Rogue.NET.Core.View;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Rogue.NET.Core.Processing.Service.Cache
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(IScenarioImageSourceFactory))]
    public class ScenarioImageSourceFactory : IScenarioImageSourceFactory
    {
        // SVG Cache is the base cache for supplying DrawingGroup instances from SVG resources. The 
        // image source cache supplies drawing images built from either the SVG cache (clones) or 
        // the smiley control.
        //
        readonly ISvgCache _svgCache;
        readonly ISymbolEffectFilter _symbolEffectFilter;

        SimpleDictionary<int, DrawingImage> _imageSourceCache;

        [ImportingConstructor]
        public ScenarioImageSourceFactory(ISvgCache svgCache, ISymbolEffectFilter symbolEffectFilter)
        {
            _svgCache = svgCache;
            _symbolEffectFilter = symbolEffectFilter;

            _imageSourceCache = new SimpleDictionary<int, DrawingImage>();
        }

        public DrawingImage GetImageSource(SymbolDetailsTemplate symbolDetails, double scale, double effectiveVision, Light[] lighting)
        {
            // PERFORMANCE - HASH CODE ONLY
            var hash = CreateCacheHash(symbolDetails, false, scale, effectiveVision, lighting);

            // Check for cached image
            if (_imageSourceCache.ContainsKey(hash))
                return _imageSourceCache[hash];            

            // Cache the result
            else
            {
                var cacheImage = CreateCacheImage(symbolDetails, false, scale, effectiveVision, lighting);
                var result = GetImageSource(cacheImage);

                _imageSourceCache[hash] = result;

                return result;
            }
        }
        public DrawingImage GetImageSource(ScenarioImage scenarioImage, double scale, double effectiveVision, Light[] lighting)
        {
            // PERFORMANCE - HASH CODE ONLY
            var hash = CreateCacheHash(scenarioImage, false, scale, effectiveVision, lighting);

            // Check for cached image
            if (_imageSourceCache.ContainsKey(hash))
                return _imageSourceCache[hash];
           
            // Cache the result
            else
            {
                var cacheImage = CreateCacheImage(scenarioImage, false, scale, effectiveVision, lighting);
                var result = GetImageSource(cacheImage);

                _imageSourceCache[hash] = result;

                return result;
            }
        }
        public DrawingImage GetDesaturatedImageSource(ScenarioImage scenarioImage, double scale, double effectiveVision, Light[] lighting)
        {
            // PERFORMANCE - HASH CODE ONLY
            var hash = CreateCacheHash(scenarioImage, true, scale, effectiveVision, lighting);

            // Check for cached image
            if (_imageSourceCache.ContainsKey(hash))
                return _imageSourceCache[hash];

            var cacheImage = CreateCacheImage(scenarioImage, true, scale, effectiveVision, lighting);

            var source = GetImageSource(cacheImage);

            if (source is DrawingImage)
            {
                // Recurse drawing to desaturate colors
                _symbolEffectFilter.ApplyEffect((source as DrawingImage).Drawing as DrawingGroup, new HslEffect(0, -1, 0, false));

                // Cache the gray-scale image
                _imageSourceCache[hash] = source;

                return source;
            }
            else
                throw new Exception("Unhandled ImageSource type");
        }
        public DrawingImage GetDesaturatedImageSource(SymbolDetailsTemplate symbolDetails, double scale, double effectiveVision, Light[] lighting)
        {
            // PERFORMANCE - HASH CODE ONLY
            var hash = CreateCacheHash(symbolDetails, true, scale, effectiveVision, lighting);

            // Check for cached image
            if (_imageSourceCache.ContainsKey(hash))
                return _imageSourceCache[hash];

            var cacheImage = CreateCacheImage(symbolDetails, true, scale, effectiveVision, lighting);

            var source = GetImageSource(cacheImage);

            if (source is DrawingImage)
            {
                // Recurse drawing to desaturate colors
                _symbolEffectFilter.ApplyEffect((source as DrawingImage).Drawing as DrawingGroup, new HslEffect(0, -1, 0, false));

                // Cache the gray-scale image
                _imageSourceCache[hash] = source;

                return source;
            }
            else
                throw new Exception("Unhandled ImageSource type");
        }
        public FrameworkElement GetFrameworkElement(ScenarioImage scenarioImage, double scale, double effectiveVision, Light[] lighting)
        {
            // PERFORMANCE - HASH CODE ONLY
            var hash = CreateCacheHash(scenarioImage, false, scale, effectiveVision, lighting);

            // Check for cached FrameworkElement
            if (_imageSourceCache.ContainsKey(hash))
            {
                // Using Clone() to create new framework elements
                var source = _imageSourceCache[hash].Clone();

                return CreateScaledImage(source, scale);
            }

            var cacheImage = CreateCacheImage(scenarioImage, false, scale, effectiveVision, lighting);
            
            var imageSource = GetImageSource(cacheImage);

            // Cache the image source
            _imageSourceCache[hash] = imageSource;

            return CreateScaledImage(imageSource, scale);


            //switch (scenarioImage.SymbolType)
            //{
            //    case SymbolType.Character:
            //    case SymbolType.Symbol:
            //    case SymbolType.OrientedSymbol:
            //    case SymbolType.Game:
            //    case SymbolType.Terrain:
            //        {
            //            // Fetch / Load the drawing from the cache
            //            var drawing = _svgCache.GetDrawing(cacheImage);

            //            // Apply Effects - Scale, ColorMap, HSL
            //            var completedDrawing = ApplyEffects(drawing, cacheImage);

            //            // Create the image source
            //            var source = new DrawingImage(completedDrawing);

            //            // Cache the image source
            //            _imageSourceCache[hash] = source;

            //            return CreateScaledImage(source, cacheImage.Scale);
            //        }
            //    case SymbolType.Smiley: // No Cache
            //        return GetSmileyElement(cacheImage);

            //    default:
            //        throw new Exception("Unknown symbol type");
            //}
        }

        #region (private) Image Methods
        private DrawingImage GetImageSource(ScenarioCacheImage cacheImage)
        {
            switch (cacheImage.SymbolType)
            {
                case SymbolType.Character:
                case SymbolType.Symbol:
                case SymbolType.OrientedSymbol:
                case SymbolType.Game:
                case SymbolType.Terrain:
                    {
                        // Fetch / Load the drawing from the cache
                        var drawing = _svgCache.GetDrawing(cacheImage);

                        // Apply Effects - Scale, ColorClamp, HSL
                        var completedDrawing = ApplyEffects(drawing, cacheImage);

                        // Apply Lighting
                        foreach (var light in cacheImage.Lighting)
                            ApplyLighting(completedDrawing, light);

                        ApplyIntensity(completedDrawing, (cacheImage.EffectiveVision * cacheImage.Lighting.Max(light => light.Intensity)).LowLimit(ModelConstants.MinLightIntensity));

                        // Create the image source
                        return new DrawingImage(completedDrawing);
                    }
                case SymbolType.Smiley:
                    // GetSmileyElement() -> ApplyLighting()
                    return GetSmileyImage(cacheImage);
                default:
                    throw new Exception("Unknown symbol type");
            }
        }
        private DrawingImage GetSmileyImage(ScenarioCacheImage cacheImage)
        {
            // Create a smiley control
            var control = GetSmileyElement(cacheImage);

            // Create a drawing to represent the paths (includes coloring)
            var drawing = control.CreateDrawing();

            // Create image source
            return new DrawingImage(drawing);
        }
        private Smiley GetSmileyElement(ScenarioCacheImage cacheImage)
        {
            var ctrl = new Smiley();
            ctrl.Width = ModelConstants.CellWidth * cacheImage.Scale;
            ctrl.Height = ModelConstants.CellHeight * cacheImage.Scale;

            var smileyColor = ColorOperations.Convert(cacheImage.SmileyBodyColor);
            var smileyLineColor = ColorOperations.Convert(cacheImage.SmileyLineColor);

            // Apply lighting
            foreach (var light in cacheImage.Lighting)
            {
                smileyColor = LightOperations.ApplyLightingEffect(smileyColor, light);
                smileyLineColor = LightOperations.ApplyLightingEffect(smileyLineColor, light);
            }

            smileyColor = LightOperations.ApplyLightIntensity(smileyColor, (cacheImage.EffectiveVision * cacheImage.Lighting.Max(light => light.Intensity)).LowLimit(ModelConstants.MinLightIntensity));
            smileyLineColor = LightOperations.ApplyLightIntensity(smileyLineColor, (cacheImage.EffectiveVision * cacheImage.Lighting.Max(light => light.Intensity)).LowLimit(ModelConstants.MinLightIntensity));

            ctrl.SmileyColor = smileyColor;
            ctrl.SmileyLineColor = smileyLineColor;
            ctrl.SmileyExpression = cacheImage.SmileyExpression;

            // TODO: fix the initialization problem
            ctrl.Initialize();

            return ctrl;
        }
        private DrawingGroup ApplyEffects(DrawingGroup drawing, ScenarioCacheImage cacheImage)
        {
            // Apply Effects - Scale, Color Clamp / ColorMap, HSL
            //

            switch (cacheImage.SymbolType)
            {
                // Smiley symbol scaling is handled else-where
                case SymbolType.Smiley:
                default:
                    return drawing;
                case SymbolType.Game:
                case SymbolType.Terrain:
                case SymbolType.Character:
                case SymbolType.Symbol:
                case SymbolType.OrientedSymbol:
                    {
                        // Apply Coloring
                        switch (cacheImage.SymbolEffectType)
                        {
                            case CharacterSymbolEffectType.None:
                                break;
                            case CharacterSymbolEffectType.ColorClamp:
                                _symbolEffectFilter.ApplyEffect(drawing, new ClampEffect(cacheImage.SymbolClampColor));
                                break;
                            case CharacterSymbolEffectType.HslShift:
                                _symbolEffectFilter.ApplyEffect(drawing, new HslEffect(cacheImage.SymbolHue, cacheImage.SymbolSaturation, cacheImage.SymbolLightness, false));
                                break;
                            case CharacterSymbolEffectType.HslShiftColorMask:
                                _symbolEffectFilter.ApplyEffect(drawing, new HslEffect(cacheImage.SymbolHue, cacheImage.SymbolSaturation, cacheImage.SymbolLightness, true));
                                break;
                            default:
                                throw new Exception("Unhandled Symbol Effect Type ScenarioImageSouceFactory");
                        }

                        // Apply Background Coloring
                        var fullDrawing = CreateBackground(drawing, cacheImage.BackgroundColor, cacheImage.Scale);

                        // Create scaled / size transform JUST FOR THE PRIMARY DRAWING
                        drawing.Transform = CreateSymbolSizeTransform(drawing, cacheImage.SymbolSize, cacheImage.Scale);

                        return fullDrawing;
                    }
            }
        }
        private void ApplyLighting(DrawingGroup drawing, Light lighting)
        {
            _symbolEffectFilter.ApplyEffect(drawing, new LightingEffect(lighting));
        }
        private void ApplyIntensity(DrawingGroup drawing, double intensity)
        {
            _symbolEffectFilter.ApplyEffect(drawing, new LightIntensityEffect(intensity));
        }
        private Transform CreateSymbolSizeTransform(DrawingGroup drawing, CharacterSymbolSize symbolSize, double scale)
        {
            // Scale the drawing relative to a margin specified by the symbol size enumeration. Create a sub-boundary
            // within the larger bounding rectangle; and fit the drawing to this sub-boundary.
            //
            var boundsWidth = ModelConstants.CellWidth * scale;
            var boundsHeight = ModelConstants.CellHeight * scale;
            var margin = 0.0;

            // SET DRAWING TRANSFORM TO IDENTITY TO ACCESS THE BOUNDS (DON'T KNOW HOW IT'S GETTING SET)
            drawing.Transform = Transform.Identity;

            // Calculate the margin
            switch (symbolSize)
            {
                case CharacterSymbolSize.Small:
                    margin = 2 * scale;
                    break;
                case CharacterSymbolSize.Medium:
                    margin = 1 * scale;
                    break;
                case CharacterSymbolSize.Large:
                    margin = 0;
                    break;
                default:
                    throw new Exception("Unhandled Character Size ScenarioImageSourceFactory");
            }

            // Calculate the transform based on the content bounds - CENTER CONTENT

            // Optimum dimensions
            var width = boundsWidth - (2 * margin);
            var height = boundsHeight - (2 * margin);

            // Change from the drawing content
            var relativeScaleX = width / drawing.Bounds.Width;
            var relativeScaleY = height / drawing.Bounds.Height;

            var relativeScale = 1.0;
            var offsetX = 0.0;
            var offsetY = 0.0;

            // Select smallest change based on the dimension (SMALLER NEGATIVE ALSO TO SHRINK TO THE MARGIN)
            if (relativeScaleX < relativeScaleY)
            {
                // Fit to the margin based on the width (largest dimension)
                relativeScale = relativeScaleX;
                offsetX = margin;
                offsetY = margin + (0.5 * (height - (drawing.Bounds.Height * relativeScale)));
            }

            else
            {
                // Fit to the margin based on the height (largest dimension)
                relativeScale = relativeScaleY;
                offsetX = margin + (0.5 * (width - (drawing.Bounds.Width * relativeScale)));
                offsetY = margin;
            }
            
            var scaleTransform = new ScaleTransform(relativeScale, relativeScale);
            var translateTransform = new TranslateTransform(offsetX, offsetY);
            var transform = new TransformGroup();

            transform.Children.Add(scaleTransform);
            transform.Children.Add(translateTransform);

            return transform;
        }
        private DrawingGroup CreateBackground(DrawingGroup drawing, string backgroundColor, double scale)
        {
            var cellBoundary = new Rect(new Size(ModelConstants.CellWidth * scale, ModelConstants.CellHeight * scale));

            var brush = new SolidColorBrush(ColorOperations.Convert(backgroundColor));
            var pen = new Pen(Brushes.Transparent, 0.0);

            var background = new GeometryDrawing(brush, pen, new RectangleGeometry(cellBoundary));

            // Re-draw the drawing with the background first
            var fullDrawing = new DrawingGroup();

            fullDrawing.Children.Add(background);
            fullDrawing.Children.Add(drawing);

            return fullDrawing;
        }
        private Image CreateScaledImage(ImageSource source, double scale)
        {
            // Return scaled image
            var image = new Image();
            image.Width = ModelConstants.CellWidth * scale;
            image.Height = ModelConstants.CellHeight * scale;
            image.Source = source;

            return image;
        }
        private int CreateCacheHash(SymbolDetailsTemplate template, bool grayScale, double scale, double effectiveVision, Light[] lighting)
        {
            // Clip the scale to a safe number
            var safeScale = scale.Clip(1, 10);

            return ScenarioCacheImage.CreateHash(safeScale, template.SymbolType, template.SymbolSize,
                                                 template.SymbolEffectType,
                                                 template.SmileyExpression, template.SmileyBodyColor, 
                                                 template.SmileyLineColor, template.SymbolPath,
                                                 template.SymbolHue, template.SymbolSaturation, 
                                                 template.SymbolLightness, template.SymbolClampColor,
                                                 template.BackgroundColor, grayScale,
                                                 effectiveVision, lighting);
        }
        private int CreateCacheHash(ScenarioImage scenarioImage, bool grayScale, double scale, double effectiveVision, Light[] lighting)
        {
            // Clip the scale to a safe number
            var safeScale = scale.Clip(1, 10);

            return ScenarioCacheImage.CreateHash(safeScale, scenarioImage.SymbolType, scenarioImage.SymbolSize,
                                                 scenarioImage.SymbolEffectType,
                                                 scenarioImage.SmileyExpression, scenarioImage.SmileyBodyColor,
                                                 scenarioImage.SmileyLineColor, scenarioImage.SymbolPath,
                                                 scenarioImage.SymbolHue, scenarioImage.SymbolSaturation,
                                                 scenarioImage.SymbolLightness, scenarioImage.SymbolClampColor,
                                                 scenarioImage.BackgroundColor, grayScale,
                                                 effectiveVision, lighting);
        }
        private ScenarioCacheImage CreateCacheImage(SymbolDetailsTemplate template, bool grayScale, double scale, double effectiveVision, Light[] lighting)
        {
            // Clip the scale to a safe number
            var safeScale = scale.Clip(1, 10);

            // Create cache image to retrieve cached ImageSource or to store it
            return new ScenarioCacheImage(template, grayScale, safeScale, effectiveVision, lighting);
        }
        private ScenarioCacheImage CreateCacheImage(ScenarioImage scenarioImage, bool grayScale, double scale, double effectiveVision, Light[] lighting)
        {
            // Clip the scale to a safe number
            var safeScale = scale.Clip(1, 10);

            // Create cache image to retrieve cached ImageSource or to store it
            return new ScenarioCacheImage(scenarioImage, grayScale, safeScale, effectiveVision, lighting);
        }
        #endregion
    }
}
