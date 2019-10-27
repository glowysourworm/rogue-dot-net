using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Media.SymbolEffect;
using Rogue.NET.Core.Media.SymbolEffect.Utility;
using Rogue.NET.Core.Model;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content;
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

        IDictionary<string, DrawingImage> _imageSourceCache;

        [ImportingConstructor]
        public ScenarioImageSourceFactory(ISvgCache svgCache)
        {
            _svgCache = svgCache;

            _imageSourceCache = new Dictionary<string, DrawingImage>();
        }
        public DrawingImage GetImageSource(SymbolDetailsTemplate symbolDetails, double scale)
        {
            var cacheImage = CreateCacheImage(symbolDetails, false, scale);
            var cacheKey = cacheImage.ToFingerprint();

            // Check for cached image
            if (_imageSourceCache.ContainsKey(cacheKey))
                return _imageSourceCache[cacheKey];

            // Cache the result
            else
            {
                var result = GetImageSource(cacheImage);

                _imageSourceCache[cacheKey] = result;

                return result;
            }
        }
        public DrawingImage GetImageSource(ScenarioImage scenarioImage, double scale)
        {
            var cacheImage = CreateCacheImage(scenarioImage, false, scale);
            var cacheKey = cacheImage.ToFingerprint();

            // Check for cached image
            if (_imageSourceCache.ContainsKey(cacheKey))
                return _imageSourceCache[cacheKey];

            // Cache the result
            else
            {
                var result = GetImageSource(cacheImage);

                _imageSourceCache[cacheKey] = result;

                return result;
            }
        }
        public DrawingImage GetDesaturatedImageSource(ScenarioImage scenarioImage, double scale)
        {
            var cacheImage = CreateCacheImage(scenarioImage, true, scale);
            var cacheKey = cacheImage.ToFingerprint();

            // Check for cached image
            if (_imageSourceCache.ContainsKey(cacheKey))
                return _imageSourceCache[cacheKey];

            var source = GetImageSource(cacheImage);

            if (source is DrawingImage)
            {
                // Recurse drawing to desaturate colors
                DrawingFilter.ApplyEffect((source as DrawingImage).Drawing as DrawingGroup, new HSLEffect(0, -1, 0, false));

                // Cache the gray-scale image
                _imageSourceCache[cacheKey] = source;

                return source;
            }
            else
                throw new Exception("Unhandled ImageSource type");
        }
        public FrameworkElement GetFrameworkElement(ScenarioImage scenarioImage, double scale)
        {
            var cacheImage = CreateCacheImage(scenarioImage, false, scale);
            var cacheKey = cacheImage.ToFingerprint();

            // Check for cached FrameworkElement
            if (_imageSourceCache.ContainsKey(cacheKey))
            {
                // Using Clone() to create new framework elements
                var source = _imageSourceCache[cacheKey].Clone();

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
                        _imageSourceCache[cacheKey] = source;

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

                        // Create the image source
                        return new DrawingImage(drawing);
                    }
                case SymbolType.Smiley:
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
            ctrl.SmileyColor = (Color)ColorConverter.ConvertFromString(cacheImage.SmileyBodyColor);
            ctrl.SmileyLineColor = (Color)ColorConverter.ConvertFromString(cacheImage.SmileyLineColor);
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
                        DrawingFilter.ApplyEffect(drawing, new ClampEffect(cacheImage.CharacterColor));
                    }
                    break;
                case SymbolType.Symbol:
                case SymbolType.OrientedSymbol:
                case SymbolType.Terrain:
                    {
                        // Apply base scale - requested by calling code
                        drawing.Transform = new ScaleTransform(cacheImage.Scale, cacheImage.Scale);

                        // Apply HSL transform
                        DrawingFilter.ApplyEffect(drawing, new HSLEffect(cacheImage.SymbolHue, cacheImage.SymbolSaturation, cacheImage.SymbolLightness, cacheImage.SymbolUseColorMask));
                    }
                    break;
            }
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
        private ScenarioCacheImage CreateCacheImage(SymbolDetailsTemplate template, bool grayScale, double scale)
        {
            // Clip the scale to a safe number
            var safeScale = scale.Clip(1, 10);

            // Create cache image to retrieve cached ImageSource or to store it
            return new ScenarioCacheImage(template, grayScale, safeScale);
        }
        private ScenarioCacheImage CreateCacheImage(ScenarioImage scenarioImage, bool grayScale, double scale)
        {
            // Clip the scale to a safe number
            var safeScale = scale.Clip(1, 10);

            // Create cache image to retrieve cached ImageSource or to store it
            return new ScenarioCacheImage(scenarioImage, grayScale, safeScale);
        }
        #endregion
    }
}
