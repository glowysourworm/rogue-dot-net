using Rogue.NET.Common.Utility;
using Rogue.NET.Common.ViewModel;
using Rogue.NET.Core.Model;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ResourceCache;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Model.ScenarioConfiguration;
using Rogue.NET.Core.Utility;
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

namespace Rogue.NET.Core.Processing.Service
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(IScenarioResourceService))]
    public class ScenarioResourceService : IScenarioResourceService
    {
        readonly IScenarioFileService _scenarioFileService;

        const int DPI = 96;

        IDictionary<string, ScenarioConfigurationContainer> _scenarioConfigurations;
        IDictionary<string, object> _scenarioImageCache;
        IEnumerable<ColorViewModel> _colors;

        [ImportingConstructor]
        public ScenarioResourceService(IScenarioFileService scenarioFileService)
        {
            _scenarioFileService = scenarioFileService;

            _scenarioConfigurations = new Dictionary<string, ScenarioConfigurationContainer>();
            _scenarioImageCache = new Dictionary<string, object>();
            _colors = ColorUtility.CreateColors();
        }

        #region (public) Methods
        public void LoadAllConfigurations()
        {
            var customScenarioConfigurationFiles = Directory.GetFiles(ResourceConstants.ScenarioDirectory, "*." + ResourceConstants.ScenarioConfigurationExtension, SearchOption.TopDirectoryOnly);

            // Load Custom Scenario Configurations
            foreach (var scenarioConfigurationName in customScenarioConfigurationFiles.Select(x => Path.GetFileNameWithoutExtension(x)))
            {
                var configuration = _scenarioFileService.OpenConfiguration(scenarioConfigurationName);
                if (configuration != null)
                    _scenarioConfigurations.Add(scenarioConfigurationName, configuration);
            }

            // Load Built-In Scenario Configurations
            GetScenarioConfiguration(ConfigResources.Fighter);
            GetScenarioConfiguration(ConfigResources.Paladin);
            GetScenarioConfiguration(ConfigResources.Witch);
            GetScenarioConfiguration(ConfigResources.Sorcerer);
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
        public BitmapSource GetImageSource(SymbolDetailsTemplate symbolDetails, double scale, bool bypassCache = false)
        {
            // Clip the scale to a safe number
            var safeScale = scale.Clip(1, 10);

            // Create cache image to retrieve cached BitmapSource or to store it
            var cacheImage = new ScenarioCacheImage(symbolDetails, false, safeScale);
            var cacheKey = cacheImage.ToFingerprint();

            // Check for cached image
            if (_scenarioImageCache.ContainsKey(cacheKey) && !bypassCache)
                return _scenarioImageCache[cacheKey] as BitmapSource;

            BitmapSource result;

            // Create a new BitmapSource
            switch (cacheImage.SymbolType)
            {
                case SymbolTypes.Character:
                    result = GetImage(cacheImage.CharacterSymbol, cacheImage.CharacterColor, cacheImage.Scale);
                    break;
                case SymbolTypes.Smiley:
                    result = GetImage(cacheImage.SmileyExpression, cacheImage.SmileyBodyColor, cacheImage.SmileyLineColor, cacheImage.SmileyAuraColor, cacheImage.Scale);
                    break;
                case SymbolTypes.Image:
                    result = GetImage(cacheImage.Icon);
                    break;
                case SymbolTypes.DisplayImage:
                    result = GetImage(cacheImage.DisplayIcon);
                    break;
                default:
                    throw new Exception("Unknown symbol type");
            }

            // Cache the result
            _scenarioImageCache[cacheKey] = result;

            return result;
        }
        public BitmapSource GetImageSource(ScenarioImage scenarioImage, double scale, bool bypassCache = false)
        {
            // Clip the scale to a safe number
            var safeScale = scale.Clip(1, 10);

            // Create cache image to retrieve cached BitmapSource or to store it
            var cacheImage = new ScenarioCacheImage(scenarioImage, ScenarioCacheImageType.ImageSource, false, safeScale);
            var cacheKey = cacheImage.ToFingerprint();
            
            // Check for cached image
            if (_scenarioImageCache.ContainsKey(cacheKey) && !bypassCache)
                return _scenarioImageCache[cacheKey] as BitmapSource;

            BitmapSource result;

            // Create a new BitmapSource
            switch (scenarioImage.SymbolType)
            {
                case SymbolTypes.Character:
                    result = GetImage(scenarioImage.CharacterSymbol, scenarioImage.CharacterColor, safeScale);
                    break;
                case SymbolTypes.Smiley:
                    result = GetImage(scenarioImage.SmileyExpression, scenarioImage.SmileyBodyColor, scenarioImage.SmileyLineColor, scenarioImage.SmileyLightRadiusColor, safeScale);
                    break;
                case SymbolTypes.Image:
                    result = GetImage(scenarioImage.Icon);
                    break;
                case SymbolTypes.DisplayImage:
                    result = GetImage(scenarioImage.DisplayIcon);
                    break;
                default:
                    throw new Exception("Unknown symbol type");
            }

            // Cache the result
            _scenarioImageCache[cacheKey] = result;

            return result;
        }
        public BitmapSource GetDesaturatedImageSource(ScenarioImage scenarioImage, double scale, bool bypassCache = false)
        {
            // Clip the scale to a safe number
            var safeScale = scale.Clip(1, 10);

            // Create cache image to retrieve cached GrayScale BitmapSource or to store it
            var cacheImage = new ScenarioCacheImage(scenarioImage, ScenarioCacheImageType.ImageSource, true, safeScale);
            var cacheKey = cacheImage.ToFingerprint();

            // Check for cached image
            if (_scenarioImageCache.ContainsKey(cacheKey) && !bypassCache)
                return _scenarioImageCache[cacheKey] as BitmapSource;

            // Create gray-scale image (also can use cache to get color image)
            // TODO: figure out why transparency doesn't work; and fix the gray scale image
            var bitmapSource = GetImageSource(scenarioImage, safeScale);
            var formatConvertedBitmap = new FormatConvertedBitmap(bitmapSource, PixelFormats.Pbgra32, BitmapPalettes.WebPaletteTransparent, 100);

            // Cache the gray-scale image
            _scenarioImageCache[cacheKey] = formatConvertedBitmap;

            return formatConvertedBitmap;
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
                case SymbolTypes.Character:
                    result = GetElement(scenarioImage.CharacterSymbol, scenarioImage.CharacterColor, safeScale);
                    break;
                case SymbolTypes.Smiley:
                    result = GetElement(scenarioImage.SmileyExpression, scenarioImage.SmileyBodyColor, scenarioImage.SmileyLineColor, scenarioImage.SmileyLightRadiusColor, safeScale);
                    break;
                case SymbolTypes.Image:
                    result = GetElement(scenarioImage.Icon, safeScale);
                    break;
                case SymbolTypes.DisplayImage:
                    result = GetElement(scenarioImage.DisplayIcon, safeScale);
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
        private BitmapSource GetImage(ImageResources img)
        {
            var path = "Rogue.NET.Common.Resource.Images.ScenarioObjects." + img.ToString() + ".png";
            var assembly = Assembly.GetAssembly(typeof(ZipEncoder));
            var stream = assembly.GetManifestResourceStream(path);
            var decoder = new PngBitmapDecoder(stream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
            return decoder.Frames[0];
        }
        private BitmapSource GetImage(DisplayImageResources displayImageResources)
        {
            var path = "Rogue.NET.Common.Resource.Images.DisplayImages." + displayImageResources.ToString() + ".png";
            var assembly = Assembly.GetAssembly(typeof(ZipEncoder));
            var stream = assembly.GetManifestResourceStream(path);
            var decoder = new PngBitmapDecoder(stream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
            return decoder.Frames[0];
        }
        private BitmapSource GetImage(string symbol, string symbolColor, double scale)
        {
            var text = GetElement(symbol, symbolColor, scale);
            text.Background = Brushes.Transparent;
            text.Measure(new Size(text.Width, text.Height));
            text.Arrange(new Rect(text.DesiredSize));

            var bmp = new RenderTargetBitmap((int)(ModelConstants.CellWidth * scale), (int)(ModelConstants.CellHeight * scale), DPI, DPI, PixelFormats.Default);
            bmp.Render(text);
            return bmp;
        }
        private BitmapSource GetImage(SmileyExpression expression, string bodyColor, string lineColor, string auraColor, double scale)
        {
            var ctrl = GetElement(expression, bodyColor, lineColor, auraColor, scale);
            ctrl.Background = Brushes.Transparent;
            ctrl.Measure(new Size(ctrl.Width, ctrl.Height));
            ctrl.Arrange(new Rect(0,0,ctrl.Width, ctrl.Height));
            RenderOptions.SetBitmapScalingMode(ctrl, BitmapScalingMode.Fant);
            var bmp = new RenderTargetBitmap((int)(ModelConstants.CellWidth * scale), (int)(ModelConstants.CellHeight * scale), DPI, DPI, PixelFormats.Default);

            bmp.Render(ctrl);
            return bmp;
        }

        private Image GetElement(ImageResources imageResource, double scale)
        {
            var result = new Image();
            result.Width = ModelConstants.CellWidth * scale;
            result.Height = ModelConstants.CellHeight * scale;
            result.Source = GetImage(imageResource);

            return result;
        }
        private Image GetElement(DisplayImageResources displayImageResource, double scale)
        {
            var result = new Image();
            result.Width = ModelConstants.CellWidth * scale;
            result.Height = ModelConstants.CellHeight * scale;
            result.Source = GetImage(displayImageResource);

            return result;
        }
        private TextBlock GetElement(string symbol, string symbolColor, double scale)
        {
            var text = new TextBlock();
            var foregroundColor = (Color)ColorConverter.ConvertFromString(symbolColor);

            text.Foreground = new SolidColorBrush(foregroundColor);
            text.Background = Brushes.Transparent;
            text.Text = symbol;
            text.FontSize = 12 * scale.Clip(1, 10);
            text.FontFamily = Application.Current.MainWindow.FontFamily;
            text.TextAlignment = TextAlignment.Center;
            text.Margin = new Thickness(0);
            text.Height = ModelConstants.CellHeight * scale;
            text.Width = ModelConstants.CellWidth * scale;

            return text;
        }
        private Smiley GetElement(SmileyExpression expression, string bodyColor, string lineColor, string auraColor, double scale)
        {
            var ctrl = new Smiley();
            ctrl.Width = ModelConstants.CellWidth * scale;
            ctrl.Height = ModelConstants.CellHeight * scale;
            ctrl.SmileyColor = (Color)ColorConverter.ConvertFromString(bodyColor);
            ctrl.SmileyLineColor = (Color)ColorConverter.ConvertFromString(lineColor);
            ctrl.SmileyExpression = expression;

            // TODO: fix the initialization problem. (OR) just get rid of images in favor of vector drawings...
            ctrl.Initialize();

            return ctrl;
        }
        #endregion
    }
}
