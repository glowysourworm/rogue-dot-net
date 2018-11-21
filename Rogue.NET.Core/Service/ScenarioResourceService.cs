﻿using Rogue.NET.Common.Utility;
using Rogue.NET.Common.ViewModel;
using Rogue.NET.Core.IO;
using Rogue.NET.Core.Model;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ResourceCache;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Model.ScenarioConfiguration;
using Rogue.NET.Core.Service.Interface;
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

namespace Rogue.NET.Core.Service
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
            var customScenarioConfigurations = Directory.GetFiles(ResourceConstants.ScenarioDirectory, "*." + ResourceConstants.ScenarioConfigurationExtension, SearchOption.TopDirectoryOnly);

            // Load Custom Scenario Configurations
            foreach (var file in customScenarioConfigurations)
            {
                var configuration = _scenarioFileService.OpenConfiguration(Path.GetFileName(file));
                if (configuration != null)
                    _scenarioConfigurations.Add(Path.GetFileNameWithoutExtension(file), configuration);
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
                _scenarioConfigurations.Add(configResource.ToString(), (ScenarioConfigurationContainer)BinarySerializer.Deserialize(memoryStream.GetBuffer()));
            }

            return _scenarioConfigurations[configResource.ToString()];
        }
        public ScenarioConfigurationContainer GetScenarioConfiguration(string configurationName)
        {
            if (!_scenarioConfigurations.ContainsKey(configurationName))
                throw new Exception("Configuration not found - " + configurationName);

            return _scenarioConfigurations[configurationName];
        }
        public BitmapSource GetImageSource(SymbolDetailsTemplate symbolDetails)
        {
            // Create cache image to retrieve cached BitmapSource or to store it
            var cacheImage = new ScenarioCacheImage(symbolDetails);
            var cacheKey = cacheImage.ToFingerprint();

            // Check for cached image
            if (_scenarioImageCache.ContainsKey(cacheKey))
                return _scenarioImageCache[cacheKey] as BitmapSource;

            BitmapSource result;

            // Create a new BitmapSource
            switch (cacheImage.SymbolType)
            {
                case SymbolTypes.Character:
                    result = GetImage(cacheImage.CharacterSymbol, cacheImage.CharacterColor, false);
                    break;
                case SymbolTypes.Smiley:
                    result = GetImage(cacheImage.SmileyMood, cacheImage.SmileyBodyColor, cacheImage.SmileyLineColor, cacheImage.SmileyAuraColor, false);
                    break;
                case SymbolTypes.Image:
                    result = GetImage(cacheImage.Icon);
                    break;
                default:
                    throw new Exception("Unknown symbol type");
            }

            // Cache the result
            _scenarioImageCache[cacheKey] = result;

            return result;
        }
        public BitmapSource GetImageSource(ScenarioImage scenarioImage, bool blackBackground)
        {
            // Create cache image to retrieve cached BitmapSource or to store it
            var cacheImage = new ScenarioCacheImage(scenarioImage, blackBackground, ScenarioCacheImageType.ImageSource);
            var cacheKey = cacheImage.ToFingerprint();
            
            // Check for cached image
            if (_scenarioImageCache.ContainsKey(cacheKey))
                return _scenarioImageCache[cacheKey] as BitmapSource;

            BitmapSource result;

            // Create a new BitmapSource
            switch (scenarioImage.SymbolType)
            {
                case SymbolTypes.Character:
                    result = GetImage(scenarioImage.CharacterSymbol, scenarioImage.CharacterColor, blackBackground);
                    break;
                case SymbolTypes.Smiley:
                    result = GetImage(scenarioImage.SmileyMood, scenarioImage.SmileyBodyColor, scenarioImage.SmileyLineColor, scenarioImage.SmileyAuraColor, blackBackground);
                    break;
                case SymbolTypes.Image:
                    result = GetImage(scenarioImage.Icon);
                    break;
                default:
                    throw new Exception("Unknown symbol type");
            }

            // Cache the result
            _scenarioImageCache[cacheKey] = result;

            return result;
        }
        public FrameworkElement GetFrameworkElement(ScenarioImage scenarioImage)
        {
            // Create cache image to retrieve cached FrameworkElement or to store it
            var cacheImage = new ScenarioCacheImage(scenarioImage, false, ScenarioCacheImageType.FrameworkElement);
            var cacheKey = cacheImage.ToFingerprint();

            // Check for cached FrameworkElement
            if (_scenarioImageCache.ContainsKey(cacheKey))
                return _scenarioImageCache[cacheKey] as FrameworkElement;

            FrameworkElement result;

            switch (scenarioImage.SymbolType)
            {
                case SymbolTypes.Character:
                    result = GetElement(scenarioImage.CharacterSymbol, scenarioImage.CharacterColor);
                    break;
                case SymbolTypes.Smiley:
                    result = GetElement(scenarioImage.SmileyMood, scenarioImage.SmileyBodyColor, scenarioImage.SmileyLineColor, scenarioImage.SmileyAuraColor);
                    break;
                case SymbolTypes.Image:
                    result = GetElement(scenarioImage.Icon);
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
            if (img == ImageResources.WellYellowCopy)
                img = ImageResources.WellYellow;

            var path = "Rogue.NET.Common.Resource.Images.ScenarioObjects." + img.ToString() + ".png";
            var assembly = Assembly.GetAssembly(typeof(ZipEncoder));
            var stream = assembly.GetManifestResourceStream(path);
            var decoder = new PngBitmapDecoder(stream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
            return decoder.Frames[0];
        }
        private BitmapSource GetImage(string symbol, string symbolColor, bool blackBackground)
        {
            var text = GetElement(symbol, symbolColor);
            text.Background = blackBackground ? new SolidColorBrush(Color.FromArgb(100, 0, 0, 0)) : Brushes.Transparent;
            text.Measure(new Size(text.Width, text.Height));
            text.Arrange(new Rect(text.DesiredSize));

            var bmp = new RenderTargetBitmap((int)(ModelConstants.CELLWIDTH), (int)(ModelConstants.CELLHEIGHT), DPI, DPI, PixelFormats.Default);
            bmp.Render(text);
            return bmp;
        }
        private BitmapSource GetImage(SmileyMoods mood, string bodyColor, string lineColor, string auraColor, bool blackBackground)
        {
            var ctrl = GetElement(mood, bodyColor, lineColor, auraColor);
            ctrl.Background = blackBackground ? new SolidColorBrush(Color.FromArgb(100, 0, 0, 0)) : Brushes.Transparent;
            ctrl.Measure(new Size(ctrl.Width, ctrl.Height));
            ctrl.Arrange(new Rect(ctrl.DesiredSize));
            RenderOptions.SetBitmapScalingMode(ctrl, BitmapScalingMode.Fant);
            var bmp = new RenderTargetBitmap(ModelConstants.CELLWIDTH, ModelConstants.CELLHEIGHT, DPI, DPI, PixelFormats.Default);

            bmp.Render(ctrl);
            return bmp;
        }

        private Image GetElement(ImageResources imageResource)
        {
            if (imageResource == ImageResources.WellYellowCopy)
                imageResource = ImageResources.WellYellow;

            var result = new Image();
            result.Width = ModelConstants.CELLWIDTH;
            result.Height = ModelConstants.CELLHEIGHT;
            result.Source = GetImage(imageResource);

            return result;
        }
        private TextBlock GetElement(string symbol, string symbolColor)
        {
            var text = new TextBlock();
            var foregroundColor = (Color)ColorConverter.ConvertFromString(symbolColor);

            text.Foreground = new SolidColorBrush(foregroundColor);
            text.Background = Brushes.Transparent;
            text.Text = symbol;
            text.FontSize = 12;
            text.FontFamily = Application.Current.MainWindow.FontFamily;
            text.TextAlignment = TextAlignment.Center;
            text.Margin = new Thickness(0);
            text.Height = ModelConstants.CELLHEIGHT;
            text.Width = ModelConstants.CELLWIDTH;

            return text;
        }
        private Smiley GetElement(SmileyMoods mood, string bodyColor, string lineColor, string auraColor)
        {
            var ctrl = new Smiley();
            ctrl.SmileyColor = (Color)ColorConverter.ConvertFromString(bodyColor);
            ctrl.SmileyLineColor = (Color)ColorConverter.ConvertFromString(lineColor);
            ctrl.SmileyMood = mood;
            ctrl.SmileyRadius = 2;
            ctrl.Width = ModelConstants.CELLWIDTH;
            ctrl.Height = ModelConstants.CELLHEIGHT;
            return ctrl;
        }
        #endregion
    }
}
