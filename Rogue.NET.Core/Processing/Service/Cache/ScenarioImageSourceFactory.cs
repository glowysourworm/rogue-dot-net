﻿using Rogue.NET.Common.Extension;
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

        Dictionary<int, DrawingImage> _imageSourceCache;

        [ImportingConstructor]
        public ScenarioImageSourceFactory(ISvgCache svgCache, ISymbolEffectFilter symbolEffectFilter)
        {
            _svgCache = svgCache;
            _symbolEffectFilter = symbolEffectFilter;

            _imageSourceCache = new Dictionary<int, DrawingImage>();
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
                        _imageSourceCache[hash] = source;

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
                        foreach (var light in cacheImage.Lighting)
                            ApplyLighting(drawing, light);

                        ApplyIntensity(drawing, (cacheImage.EffectiveVision * cacheImage.Lighting.Max(light => light.Intensity)).LowLimit(ModelConstants.MinLightIntensity));

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
                        _symbolEffectFilter.ApplyEffect(drawing, new HslEffect(cacheImage.SymbolHue, cacheImage.SymbolSaturation, cacheImage.SymbolLightness, cacheImage.SymbolUseColorMask));
                    }
                    break;
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

            return ScenarioCacheImage.CreateHash(safeScale, template.SymbolType, 
                                                 template.SmileyExpression, template.SmileyBodyColor, 
                                                 template.SmileyLineColor, template.CharacterSymbol,
                                                 template.CharacterSymbolCategory, template.CharacterColor, 
                                                 template.CharacterScale, template.Symbol, 
                                                 template.SymbolHue, template.SymbolSaturation, 
                                                 template.SymbolLightness, template.SymbolScale, 
                                                 template.SymbolUseColorMask, template.GameSymbol, grayScale, 
                                                 effectiveVision, lighting);
        }
        private int CreateCacheHash(ScenarioImage scenarioImage, bool grayScale, double scale, double effectiveVision, Light[] lighting)
        {
            // Clip the scale to a safe number
            var safeScale = scale.Clip(1, 10);

            return ScenarioCacheImage.CreateHash(safeScale, scenarioImage.SymbolType,
                                                 scenarioImage.SmileyExpression, scenarioImage.SmileyBodyColor,
                                                 scenarioImage.SmileyLineColor, scenarioImage.CharacterSymbol,
                                                 scenarioImage.CharacterSymbolCategory, scenarioImage.CharacterColor,
                                                 scenarioImage.CharacterScale, scenarioImage.Symbol,
                                                 scenarioImage.SymbolHue, scenarioImage.SymbolSaturation,
                                                 scenarioImage.SymbolLightness, scenarioImage.SymbolScale,
                                                 scenarioImage.SymbolUseColorMask, scenarioImage.GameSymbol, grayScale, 
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
