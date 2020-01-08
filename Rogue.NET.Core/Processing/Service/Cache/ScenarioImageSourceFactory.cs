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

        IDictionary<ScenarioCacheImage, DrawingImage> _imageSourceCache;

        [ImportingConstructor]
        public ScenarioImageSourceFactory(ISvgCache svgCache, ISymbolEffectFilter symbolEffectFilter)
        {
            _svgCache = svgCache;
            _symbolEffectFilter = symbolEffectFilter;

            _imageSourceCache = new Dictionary<ScenarioCacheImage, DrawingImage>();
        }
        public DrawingImage GetImageSource(SymbolDetailsTemplate symbolDetails, double scale, Light lighting)
        {
            var cacheImage = CreateCacheImage(symbolDetails, false, scale, lighting);

            // Check for cached image
            if (_imageSourceCache.ContainsKey(cacheImage))
                return _imageSourceCache[cacheImage];

            // Cache the result
            else
            {
                var result = GetImageSource(cacheImage);

                _imageSourceCache[cacheImage] = result;

                return result;
            }
        }
        public DrawingImage GetImageSource(ScenarioImage scenarioImage, double scale, Light lighting)
        {
            var cacheImage = CreateCacheImage(scenarioImage, false, scale, lighting);

            // Check for cached image
            if (_imageSourceCache.ContainsKey(cacheImage))
                return _imageSourceCache[cacheImage];

            // Cache the result
            else
            {
                var result = GetImageSource(cacheImage);

                _imageSourceCache[cacheImage] = result;

                return result;
            }
        }
        public DrawingImage GetDesaturatedImageSource(ScenarioImage scenarioImage, double scale, Light lighting)
        {
            var cacheImage = CreateCacheImage(scenarioImage, true, scale, lighting);

            // Check for cached image
            if (_imageSourceCache.ContainsKey(cacheImage))
                return _imageSourceCache[cacheImage];

            var source = GetImageSource(cacheImage);

            if (source is DrawingImage)
            {
                // Recurse drawing to desaturate colors
                _symbolEffectFilter.ApplyEffect((source as DrawingImage).Drawing as DrawingGroup, new HSLEffect(0, -1, 0, false));

                // Cache the gray-scale image
                _imageSourceCache[cacheImage] = source;

                return source;
            }
            else
                throw new Exception("Unhandled ImageSource type");
        }
        public FrameworkElement GetFrameworkElement(ScenarioImage scenarioImage, double scale, Light lighting)
        {
            var cacheImage = CreateCacheImage(scenarioImage, false, scale, lighting);

            // Check for cached FrameworkElement
            if (_imageSourceCache.ContainsKey(cacheImage))
            {
                // Using Clone() to create new framework elements
                var source = _imageSourceCache[cacheImage].Clone();

                return CreateScaledImage(source, scale);
            }

            switch (scenarioImage.SymbolType)
            {
                case SymbolType.Character:
                case SymbolType.Symbol:
                case SymbolType.OrientedSymbol:
                case SymbolType.Game:
                case SymbolType.Terrain:
                    {
                        // Fetch / Load the drawing from the cache
                        var drawing = _svgCache.GetDrawing(cacheImage);

                        // Apply Effects - Scale, ColorMap, HSL
                        ApplyEffects(drawing, cacheImage);

                        // Create the image source
                        var source = new DrawingImage(drawing);

                        // Cache the image source
                        _imageSourceCache[cacheImage] = source;

                        return CreateScaledImage(source, cacheImage.Scale);
                    }
                case SymbolType.Smiley: // No Cache
                    return GetSmileyElement(cacheImage);

                default:
                    throw new Exception("Unknown symbol type");
            }
        }

        #region (private) Image Methods
        private DrawingImage GetImageSource(ScenarioCacheImage cacheImage)
        {
            switch (cacheImage.Type)
            {
                case SymbolType.Character:
                case SymbolType.Symbol:
                case SymbolType.OrientedSymbol:
                case SymbolType.Game:
                case SymbolType.Terrain:
                    {
                        // Fetch / Load the drawing from the cache
                        var drawing = _svgCache.GetDrawing(cacheImage);

                        // Apply Effects - Scale, ColorMap, HSL
                        ApplyEffects(drawing, cacheImage);

                        // Apply Lighting
                        ApplyLighting(drawing, new Light(cacheImage.LightRed, 
                                                         cacheImage.LightGreen, 
                                                         cacheImage.LightBlue, 
                                                         cacheImage.LightIntensity));
                        // Create the image source
                        return new DrawingImage(drawing);
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
            ctrl.SmileyColor = LightOperations.ApplyLightingEffect((Color)System.Windows.Media.ColorConverter.ConvertFromString(cacheImage.SmileyBodyColor), 
                                                                    new Light(cacheImage.LightRed,
                                                                              cacheImage.LightGreen,
                                                                              cacheImage.LightBlue,
                                                                              cacheImage.LightIntensity));

            ctrl.SmileyLineColor = LightOperations.ApplyLightingEffect((Color)System.Windows.Media.ColorConverter.ConvertFromString(cacheImage.SmileyLineColor), 
                                                                        new Light(cacheImage.LightRed,
                                                                                  cacheImage.LightGreen,
                                                                                  cacheImage.LightBlue,
                                                                                  cacheImage.LightIntensity));
            ctrl.SmileyExpression = cacheImage.SmileyExpression;

            // TODO: fix the initialization problem
            ctrl.Initialize();

            return ctrl;
        }
        private void ApplyEffects(DrawingGroup drawing, ScenarioCacheImage cacheImage)
        {
            // Apply Effects - Scale, Color Clamp / ColorMap, HSL
            //

            switch (cacheImage.Type)
            {
                // Smiley symbol scaling is handled else-where
                case SymbolType.Smiley:
                default:
                    return;
                case SymbolType.Game:
                    {
                        // Apply base scale - requested by calling code
                        drawing.Transform = new ScaleTransform(cacheImage.Scale, cacheImage.Scale);
                    }
                    break;
                case SymbolType.Character:
                    {
                        // Additional user-input scale + additional offset
                        if (cacheImage.CharacterScale < 1)
                        {
                            // Calculate total scale factor
                            var scaleFactor = cacheImage.Scale * cacheImage.CharacterScale;

                            // Calculate additional offset
                            var offsetX = ((1.0 - cacheImage.CharacterScale) * ModelConstants.CellWidth) * 0.5;
                            var offsetY = ((1.0 - cacheImage.CharacterScale) * ModelConstants.CellHeight) * 0.5;

                            var transform = new TransformGroup();

                            transform.Children.Add(new TranslateTransform(offsetX, offsetY));
                            transform.Children.Add(new ScaleTransform(scaleFactor, scaleFactor));

                            drawing.Transform = transform;
                        }
                        // Additional user-input scale
                        else
                            drawing.Transform = new ScaleTransform(cacheImage.Scale, cacheImage.Scale);


                        // Apply Coloring
                        _symbolEffectFilter.ApplyEffect(drawing, new ClampEffect(cacheImage.CharacterColor));
                    }
                    break;
                case SymbolType.Symbol:
                case SymbolType.OrientedSymbol:
                case SymbolType.Terrain:
                    {
                        // Apply base scale - requested by calling code
                        drawing.Transform = new ScaleTransform(cacheImage.Scale, cacheImage.Scale);

                        // Apply HSL transform
                        _symbolEffectFilter.ApplyEffect(drawing, new HSLEffect(cacheImage.SymbolHue, cacheImage.SymbolSaturation, cacheImage.SymbolLightness, cacheImage.SymbolUseColorMask));
                    }
                    break;
            }
        }
        private void ApplyLighting(DrawingGroup drawing, Light lighting)
        {
            // Apply alpha-blend lighting based on the provided color - for the whole drawing
            _symbolEffectFilter.ApplyEffect(drawing, new LightingEffect(lighting));
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
        private ScenarioCacheImage CreateCacheImage(SymbolDetailsTemplate template, bool grayScale, double scale, Light lighting)
        {
            // Clip the scale to a safe number
            var safeScale = scale.Clip(1, 10);

            // Create cache image to retrieve cached ImageSource or to store it
            return new ScenarioCacheImage(template, grayScale, safeScale, lighting);
        }
        private ScenarioCacheImage CreateCacheImage(ScenarioImage scenarioImage, bool grayScale, double scale, Light lighting)
        {
            // Clip the scale to a safe number
            var safeScale = scale.Clip(1, 10);

            // Create cache image to retrieve cached ImageSource or to store it
            return new ScenarioCacheImage(scenarioImage, grayScale, safeScale, lighting);
        }
        #endregion
    }
}
