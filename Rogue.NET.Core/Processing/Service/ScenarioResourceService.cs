using Rogue.NET.Common.Utility;
using Rogue.NET.Common.ViewModel;
using Rogue.NET.Core.Model;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ResourceCache;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Model.ScenarioConfiguration;
using Rogue.NET.Core.View;
using Rogue.NET.Core.Extension;
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Processing.Service.Interface;
using Rogue.NET.Core.Model.ResourceCache.Interface;
using SharpVectors.Converters;
using SharpVectors.Renderers.Wpf;
using Rogue.NET.Common.Constant;
using Rogue.NET.Core.Media.SymbolEffect.Utility;
using Rogue.NET.Core.Media.SymbolEffect;

namespace Rogue.NET.Core.Processing.Service
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(IScenarioResourceService))]
    public class ScenarioResourceService : IScenarioResourceService
    {
        // Default symbol for a failed SVG load
        readonly static DrawingGroup DEFAULT_DRAWING;

        readonly IScenarioFileService _scenarioFileService;

        // These are paths relative to the Svg folder in Rogue.NET.Common
        const string SVG_PATH_GAME = "Game";
        const string SVG_PATH_SCENARIO_CHARACTER = "Scenario.Character";
        const string SVG_PATH_SCENARIO_SYMBOL = "Scenario.Symbol";

        IDictionary<string, ScenarioConfigurationContainer> _scenarioConfigurations;
        IDictionary<string, object> _scenarioImageCache;
        IEnumerable<ColorViewModel> _colors;

        // Static constructor for loading default symbol
        static ScenarioResourceService()
        {
            DEFAULT_DRAWING = LoadSVG(SVG_PATH_GAME, "", GameSymbol.Consume);
        }

        [ImportingConstructor]
        public ScenarioResourceService(
                IScenarioFileService scenarioFileService,
                IScenarioConfigurationCache scenarioConfigurationCache)
        {
            _scenarioFileService = scenarioFileService;

            _scenarioConfigurations = scenarioConfigurationCache.EmbeddedConfigurations
                                                                .ToDictionary(x => x.DungeonTemplate.Name, x => x);

            _scenarioImageCache = new Dictionary<string, object>();
            _colors = ColorFilter.CreateColors();
        }

        #region (public) Methods
        public void LoadCustomConfigurations()
        {
            var customScenarioConfigurationFiles = Directory.GetFiles(ResourceConstants.ScenarioDirectory, "*." + ResourceConstants.ScenarioConfigurationExtension, SearchOption.TopDirectoryOnly);

            // Load Custom Scenario Configurations
            foreach (var scenarioConfigurationName in customScenarioConfigurationFiles.Select(x => Path.GetFileNameWithoutExtension(x)))
            {
                var configuration = _scenarioFileService.OpenConfiguration(scenarioConfigurationName);
                if (configuration != null)
                    _scenarioConfigurations.Add(scenarioConfigurationName, configuration);
            }
        }
        public IEnumerable<ScenarioConfigurationContainer> GetScenarioConfigurations()
        {
            return _scenarioConfigurations.Values;
        }
        public ScenarioConfigurationContainer GetScenarioConfiguration(ConfigResources configResource)
        {
            if (_scenarioConfigurations.ContainsKey(configResource.ToString()))
                return _scenarioConfigurations[configResource.ToString()];

            var name = configResource.ToString();
            var assembly = Assembly.GetAssembly(typeof(ZipEncoder));
            var location = "Rogue.NET.Common.Resource.Configuration." + name.ToString() + "." + ResourceConstants.ScenarioConfigurationExtension;
            using (var stream = assembly.GetManifestResourceStream(location))
            {
                var memoryStream = new MemoryStream();
                stream.CopyTo(memoryStream);

                var configuration = (ScenarioConfigurationContainer)BinarySerializer.Deserialize(memoryStream.GetBuffer());

                _scenarioConfigurations.Add(configResource.ToString(), configuration);
            }

            // Have to copy configuration because of the HasBeenGenerated flags in memory
            return _scenarioConfigurations[configResource.ToString()].DeepClone();
        }
        public ScenarioConfigurationContainer GetScenarioConfiguration(string configurationName)
        {
            if (!_scenarioConfigurations.ContainsKey(configurationName))
                throw new Exception("Configuration not found - " + configurationName);

            // Have to copy configuration because of the HasBeenGenerated flags in memory
            return _scenarioConfigurations[configurationName].DeepClone();
        }
        public ImageSource GetImageSource(SymbolDetailsTemplate symbolDetails, double scale, bool bypassCache = false)
        {
            // Clip the scale to a safe number
            var safeScale = scale.Clip(1, 10);

            // Create cache image to retrieve cached BitmapSource or to store it
            var cacheImage = new ScenarioCacheImage(symbolDetails, false, safeScale);
            var cacheKey = cacheImage.ToFingerprint();

            // Check for cached image
            if (_scenarioImageCache.ContainsKey(cacheKey) && !bypassCache)
                return _scenarioImageCache[cacheKey] as ImageSource;

            // Cache the result
            else
            {
                var result = GetImageSource(cacheImage);
                
                _scenarioImageCache[cacheKey] = result;

                return result;
            }
        }
        public ImageSource GetImageSource(ScenarioImage scenarioImage, double scale, bool bypassCache = false)
        {
            // Clip the scale to a safe number
            var safeScale = scale.Clip(1, 10);

            // Create cache image to retrieve cached BitmapSource or to store it
            var cacheImage = new ScenarioCacheImage(scenarioImage, ScenarioCacheImageType.ImageSource, false, safeScale);
            var cacheKey = cacheImage.ToFingerprint();

            // Check for cached image
            if (_scenarioImageCache.ContainsKey(cacheKey) && !bypassCache)
                return _scenarioImageCache[cacheKey] as ImageSource;

            // Cache the result
            else
            {
                var result = GetImageSource(cacheImage);

                _scenarioImageCache[cacheKey] = result;

                return result;
            }
        }
        protected ImageSource GetImageSource(ScenarioCacheImage cacheImage)
        {
            ImageSource result;

            // Create a new BitmapSource
            switch (cacheImage.Type)
            {
                case SymbolType.Character:
                    result = GetImage(cacheImage.CharacterSymbol, cacheImage.CharacterSymbolCategory, cacheImage.CharacterColor, cacheImage.Scale);
                    break;
                case SymbolType.Smiley:
                    result = GetImage(cacheImage.SmileyExpression, cacheImage.SmileyBodyColor, cacheImage.SmileyLineColor, cacheImage.SmileyAuraColor, cacheImage.Scale);
                    break;
                case SymbolType.Symbol:
                    result = GetImage(cacheImage.Symbol, cacheImage.SymbolHue, cacheImage.SymbolSaturation, cacheImage.SymbolLightness, cacheImage.Scale);
                    break;
                case SymbolType.Game:
                    result = GetImage(cacheImage.GameSymbol, cacheImage.Scale);
                    break;
                default:
                    throw new Exception("Unknown symbol type");
            }

            return result;
        }
        public ImageSource GetDesaturatedImageSource(ScenarioImage scenarioImage, double scale, bool bypassCache = false)
        {
            // Clip the scale to a safe number
            var safeScale = scale.Clip(1, 10);

            // Create cache image to retrieve cached GrayScale BitmapSource or to store it
            var cacheImage = new ScenarioCacheImage(scenarioImage, ScenarioCacheImageType.ImageSource, true, safeScale);
            var cacheKey = cacheImage.ToFingerprint();

            // Check for cached image
            if (_scenarioImageCache.ContainsKey(cacheKey) && !bypassCache)
                return _scenarioImageCache[cacheKey] as BitmapSource;

            var source = GetImageSource(cacheImage);

            if (source is DrawingImage)
            {
                // Recurse drawing to desaturate colors
                DrawingFilter.ApplyEffect((source as DrawingImage).Drawing as DrawingGroup, new SaturationEffect(0));

                // Cache the gray-scale image
                _scenarioImageCache[cacheKey] = source;

                return source;
            }
            else
                throw new Exception("Unhandled ImageSource type");
        }
        public FrameworkElement GetFrameworkElement(ScenarioImage scenarioImage, double scale, bool bypassCache = false)
        {
            // Clip the scale to a safe number
            var safeScale = scale.Clip(1, 10);

            // Create cache image to retrieve cached FrameworkElement or to store it
            var cacheImage = new ScenarioCacheImage(scenarioImage, ScenarioCacheImageType.FrameworkElement, false, safeScale);
            var cacheKey = cacheImage.ToFingerprint();

            // Check for cached FrameworkElement
            if (_scenarioImageCache.ContainsKey(cacheKey) && !bypassCache)
                return _scenarioImageCache[cacheKey] as FrameworkElement;

            FrameworkElement result;

            switch (scenarioImage.SymbolType)
            {
                case SymbolType.Character:
                    result = GetElement(scenarioImage.CharacterSymbol, scenarioImage.CharacterSymbolCategory, scenarioImage.CharacterColor, safeScale);
                    break;
                case SymbolType.Smiley:
                    result = GetElement(scenarioImage.SmileyExpression, scenarioImage.SmileyBodyColor, scenarioImage.SmileyLineColor, scenarioImage.SmileyLightRadiusColor, safeScale);
                    break;
                case SymbolType.Symbol:
                    result = GetElement(cacheImage.Symbol, cacheImage.SymbolHue, cacheImage.SymbolSaturation, cacheImage.SymbolLightness, safeScale);
                    break;
                case SymbolType.Game:
                    result = GetElement(cacheImage.GameSymbol, cacheImage.Scale);
                    break;
                default:
                    throw new Exception("Unknown symbol type");
            }

            // Store the FrameworkElement
            _scenarioImageCache[cacheKey] = result;

            return result;
        }
        public IEnumerable<ColorViewModel> GetColors()
        {
            return _colors;
        }
        #endregion

        #region (private) Methods
        private ImageSource GetImage(string symbol, double symbolHue, double symbolSaturation, double symbolLightness, double scale)
        {
            // Load SVG drawing
            var drawingGroup = LoadSVG(SVG_PATH_SCENARIO_SYMBOL, "", symbol);

            // Apply Transform
            ApplyDrawingTransform(drawingGroup, scale);

            // Apply coloring to SVG - TODO:SYMBOL:  Figure out how to apply a general HSL shift
            if (symbolHue > 0)
                DrawingFilter.ApplyEffect(drawingGroup, new HueShiftEffect(symbolHue));

            // Return image source
            return new DrawingImage(drawingGroup);
        }
        private ImageSource GetImage(string gameSymbol, double scale)
        {
            // Load SVG drawing
            var drawingGroup = LoadSVG(SVG_PATH_GAME, "", gameSymbol);

            // Apply Transform
            ApplyDrawingTransform(drawingGroup, scale);

            // Apply coloring to SVG

            // Return image source
            return new DrawingImage(drawingGroup);
        }
        private ImageSource GetImage(string character, string characterCategory, string characterColor, double scale)
        {
            // Load SVG drawing
            var drawingGroup = LoadSVG(SVG_PATH_SCENARIO_CHARACTER, characterCategory, character);

            // Apply Transform
            ApplyDrawingTransform(drawingGroup, scale);

            // Apply coloring to SVG
            DrawingFilter.ApplyEffect(drawingGroup, new ClampEffect(characterColor));

            // Return image source
            return new DrawingImage(drawingGroup);
        }
        private ImageSource GetImage(SmileyExpression expression, string bodyColor, string lineColor, string auraColor, double scale)
        {
            // Create a smiley control
            var control = GetElement(expression, bodyColor, lineColor, auraColor, scale);

            // Smiley control rendering doesn't seem to need a transform applied

            // Create a drawing to represent the paths (includes coloring)
            var drawing = control.CreateDrawing();

            // Return image source
            return new DrawingImage(drawing);
        }
        private Image GetElement(string symbol, double symbolHue, double symbolSaturation, double symbolLightness, double scale)
        {
            // Return scaled image
            var image = new Image();
            image.Width = ModelConstants.CellWidth * scale;
            image.Height = ModelConstants.CellHeight * scale;
            image.Source = GetImage(symbol, symbolHue, symbolSaturation, symbolLightness, scale);

            return image;
        }

        private Image GetElement(string gameSymbol, double scale)
        {
            // Return scaled image
            var image = new Image();
            image.Width = ModelConstants.CellWidth * scale;
            image.Height = ModelConstants.CellHeight * scale;
            image.Source = GetImage(gameSymbol, scale);

            return image;
        }
        private Image GetElement(string character, string characterCategory, string characterColor, double scale)
        {
            // Return scaled image
            var image = new Image();
            image.Width = ModelConstants.CellWidth * scale;
            image.Height = ModelConstants.CellHeight * scale;
            image.Source = GetImage(character, characterCategory, characterColor, scale);

            return image;
        }
        private Smiley GetElement(SmileyExpression expression, string bodyColor, string lineColor, string auraColor, double scale)
        {
            var ctrl = new Smiley();
            ctrl.Width = ModelConstants.CellWidth * scale;
            ctrl.Height = ModelConstants.CellHeight * scale;
            ctrl.SmileyColor = (Color)ColorConverter.ConvertFromString(bodyColor);
            ctrl.SmileyLineColor = (Color)ColorConverter.ConvertFromString(lineColor);
            ctrl.SmileyExpression = expression;

            // TODO: fix the initialization problem
            ctrl.Initialize();

            return ctrl;
        }

        public ScenarioImage GetRandomSmileyCharacter()
        {
            var characterSymbols = _scenarioConfigurations.Values
                                                          .SelectMany(x => x.PlayerTemplates.Select(z => z.SymbolDetails));

            return characterSymbols.Any() ? new ScenarioImage(characterSymbols.PickRandom()) : new ScenarioImage()
            {
                SymbolType = SymbolType.Smiley,
                SmileyBodyColor = Colors.Yellow.ToString(),
                SmileyLineColor = Colors.Black.ToString(),
                SmileyExpression = SmileyExpression.Happy
            };
        }
        #endregion

        #region (private) SVG Methods

        /// <summary>
        /// SEE SVG_PATH_* ABOVE!!! SubPath is the symbol category (points to sub-folder for symbols)
        /// </summary>
        /// <param name="svgPath">Path to primary SVG folder (See SVG_PATH_*) { Game, Scenario -> Character, Scenario -> Symbol }</param>
        /// <param name="subPath">This is the character category (</param>
        /// <param name="svgName">This is the NAME of the svg file (WITHOUT THE EXTENSION)</param>
        /// <returns></returns>
        private static DrawingGroup LoadSVG(string svgPath, string subPath, string svgName)
        {
            var assembly = Assembly.GetAssembly(typeof(ZipEncoder));
            var settings = new WpfDrawingSettings();
            settings.IncludeRuntime = true;
            settings.TextAsGeometry = true;
            settings.OptimizePath = false;

            var basePath = "Rogue.NET.Common.Resource.Svg";
            var path = !string.IsNullOrEmpty(subPath) ? string.Join(".", basePath, svgPath, subPath, svgName, "svg")
                                                      : string.Join(".", basePath, svgPath, svgName, "svg");

            try
            {
                using (var stream = assembly.GetManifestResourceStream(path))
                {
                    using (var reader = new FileSvgReader(settings))
                    {
                        return reader.Read(stream);
                    }
                }
            }
            catch (Exception)
            {
                // TODO: REMOVE THIS TRY / CATCH
                return DEFAULT_DRAWING;
            }
        }

        /// <summary>
        /// Applies a transform to the drawing group to fit it to a bounding box with the supplied scale
        /// </summary>
        /// <param name="group">The drawing</param>
        /// <param name="scale">relative scale</param>
        private void ApplyDrawingTransform(DrawingGroup group, double scale)
        {
            // TODO:SYMBOL - Figure out best way to apply this transform without modifying image sources 
            //               too much. Also have to manage cache... So, there's issues with this method.
            var transform = new TransformGroup();

            // HAVE TO MAINTAIN THE SYMBOL'S ASPECT RATIO WHILE FITTING IT TO OUR BOUNDING BOX
            //

            var scaleFactor = Math.Min((ModelConstants.CellWidth / group.Bounds.Width) * scale,
                                       (ModelConstants.CellHeight / group.Bounds.Height) * scale);

            transform.Children.Add(new TranslateTransform(group.Bounds.X * -1, group.Bounds.Y * -1));
            transform.Children.Add(new ScaleTransform(scaleFactor, scaleFactor));

            group.Transform = transform;
        }
        #endregion
    }
}
