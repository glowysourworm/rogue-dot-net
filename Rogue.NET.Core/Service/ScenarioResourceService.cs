using Prism.Events;
using Rogue.NET.Common.Utility;
using Rogue.NET.Core.IO;
using Rogue.NET.Core.Model;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Model.ScenarioConfiguration;
using Rogue.NET.Core.Service.Interface;
using Rogue.NET.Core.View;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Design;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Rogue.NET.Core.Service
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(IScenarioResourceService))]
    public class ScenarioResourceService : IScenarioResourceService
    {
        const string SAVED_GAMES_DIR = "..\\save";
        const string CMD_PREFS_FILE = "CommandPreferences.rcp";
        const string SCENARIO_EXTENSION = "rdn";
        const string SCENARIO_CONFIG_EXTENSION = "rdns";
        const string SCENARIOS_DIR = "..\\scenarios";
        const int DPI = 96;

        IList<ScenarioConfigurationContainer> _scenarioConfigurations;

        Dictionary<SymbolTypes, Dictionary<string, BitmapSource>> _imageCache;

        [ImportingConstructor]
        public ScenarioResourceService(IEventAggregator eventAggregator)
        {
            if (!Directory.Exists(SAVED_GAMES_DIR))
                Directory.CreateDirectory(SAVED_GAMES_DIR);

            if (!Directory.Exists(SCENARIOS_DIR))
                Directory.CreateDirectory(SCENARIOS_DIR);

            _scenarioConfigurations = new List<ScenarioConfigurationContainer>();

            _imageCache = new Dictionary<SymbolTypes, Dictionary<string, BitmapSource>>()
            {
                { SymbolTypes.Character, new Dictionary<string, BitmapSource>() },
                { SymbolTypes.Image, new Dictionary<string, BitmapSource>() },
                { SymbolTypes.Smiley, new Dictionary<string, BitmapSource>() }
            };
        }

        public void LoadScenarioConfiguration(ConfigResources configResource)
        {
            _scenarioConfigurations.Add(GetEmbeddedScenarioConfiguration(configResource));
        }

        public IEnumerable<ScenarioConfigurationContainer> GetScenarioConfigurations()
        {
            return _scenarioConfigurations;
        }

        public IDictionary<string, ScenarioFileHeader> GetScenarioHeaders()
        {
            var scenarioFiles = Directory.GetFiles(SAVED_GAMES_DIR);
            var scenarioHeaders = new Dictionary<string, ScenarioFileHeader>();
            foreach (var file in scenarioFiles)
            {
                var header = ScenarioFile.OpenHeader(file);
                var name = Path.GetFileNameWithoutExtension(file);
                if (header != null)
                    scenarioHeaders.Add(name, header);
            }

            return scenarioHeaders;
        }

        public ScenarioConfigurationContainer GetEmbeddedScenarioConfiguration(ConfigResources configResource)
        {
            var name = configResource.ToString();
            var assembly = Assembly.GetAssembly(typeof(ZipEncoder));
            var location = "Rogue.NET.Common.Resource.Configuration." + name.ToString() + "." + SCENARIO_CONFIG_EXTENSION;
            using (var stream = assembly.GetManifestResourceStream(location))
            {
                var memoryStream = new MemoryStream();
                stream.CopyTo(memoryStream);
                return (ScenarioConfigurationContainer)Deserialize(memoryStream.GetBuffer());
            }
        }

        public ScenarioConfigurationContainer OpenScenarioConfigurationFile(string file)
        {
            var path = Path.Combine(SCENARIOS_DIR, file);
            return (ScenarioConfigurationContainer)DeserializeFromFile(path);
        }

        public void SaveConfig(string name, ScenarioConfigurationContainer config)
        {
            var file = Path.Combine(SCENARIOS_DIR, name) + "." + SCENARIO_CONFIG_EXTENSION;
            SerializeToFile(file, config);
        }

        public BitmapSource GetImageSource(ScenarioImage scenarioImage)
        {
            switch (scenarioImage.SymbolType)
            {
                case SymbolTypes.Character:
                    return GetCharacterSymbol(scenarioImage.RogueName, scenarioImage.CharacterSymbol, scenarioImage.CharacterColor);
                case SymbolTypes.Smiley:
                    return GetSmileySymbol(scenarioImage.RogueName, scenarioImage.SmileyMood, scenarioImage.SmileyBodyColor, scenarioImage.SmileyLineColor, scenarioImage.SmileyAuraColor);
                case SymbolTypes.Image:
                    return GetImageSymbol(scenarioImage.RogueName, scenarioImage.Icon);
                default:
                    throw new Exception("Unknown symbol type");
            }
        }

        #region Serialization
        private byte[] Serialize(object obj)
        {
            var formatter = new BinaryFormatter();
            using (var stream = new MemoryStream())
            {
                formatter.Serialize(stream, obj);
                return stream.GetBuffer();
            }
        }
        private bool SerializeToFile(string file, object obj)
        {
            try
            {
                if (File.Exists(file))
                    File.Delete(file);

                using (FileStream fileStream = File.OpenWrite(file))
                {
                    var bytes = Serialize(obj);
                    fileStream.Write(bytes, 0, bytes.Length);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        private object Deserialize(byte[] buffer)
        {
            var formatter = new BinaryFormatter();
            using (var stream = new MemoryStream(buffer))
            {
                return formatter.Deserialize(stream);
            }
        }
        private object DeserializeFromFile(string file)
        {
            try
            {
                var bytes = File.ReadAllBytes(file);
                return Deserialize(bytes);
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion

        #region ImageSource
        private BitmapSource GetImageSymbol(string rogueName, ImageResources imageResource)
        {
            if (_imageCache[SymbolTypes.Image].ContainsKey(rogueName))
                return _imageCache[SymbolTypes.Image][rogueName];

            var bitmapSource = GetImage(imageResource);

            _imageCache[SymbolTypes.Image][rogueName] = bitmapSource;
            return bitmapSource;
        }
        private BitmapSource GetCharacterSymbol(string rogueName, string characterSymbol, string characterSymbolColor)
        {
            if (_imageCache[SymbolTypes.Character].ContainsKey(rogueName))
                return _imageCache[SymbolTypes.Character][rogueName];

            var bitmapSource = GetImage(characterSymbol, characterSymbolColor);

            _imageCache[SymbolTypes.Character][rogueName] = bitmapSource;
            return bitmapSource;
        }
        private BitmapSource GetSmileySymbol(string rogueName, SmileyMoods mood, string bodyColor, string lineColor, string auraColor)
        {
            if (_imageCache[SymbolTypes.Smiley].ContainsKey(rogueName))
                return _imageCache[SymbolTypes.Smiley][rogueName];

            var bitmapSource = GetImage(mood, bodyColor, lineColor, auraColor);

            _imageCache[SymbolTypes.Smiley][rogueName] = bitmapSource;
            return bitmapSource;
        }
        public BitmapSource GetImage(ImageResources img)
        {
            if (img == ImageResources.WellYellowCopy)
                img = ImageResources.WellYellow;

            var path = "Rogue.NET.Common.Resource.Images.ScenarioObjects." + img.ToString() + ".png";
            var assembly = Assembly.GetAssembly(typeof(ZipEncoder));
            var stream = assembly.GetManifestResourceStream(path);
            var decoder = new PngBitmapDecoder(stream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
            return decoder.Frames[0];
        }
        private BitmapSource GetImage(string symbol, string symbolColor)
        {
            var text = new TextBlock();
            var foregroundColor = (Color)ColorConverter.ConvertFromString(symbolColor);

            text.Foreground = new SolidColorBrush(foregroundColor);
            text.Background = Brushes.Transparent;
            text.Text = symbol;
            text.FontSize = 12;
            text.FontFamily = Application.Current.MainWindow.FontFamily;
            text.Height = ModelConstants.CELLHEIGHT;
            text.Width = ModelConstants.CELLWIDTH;
            text.Measure(new Size(text.Width, text.Height));
            text.Arrange(new Rect(text.DesiredSize));

            var bmp = new RenderTargetBitmap((int)(ModelConstants.CELLWIDTH), (int)(ModelConstants.CELLHEIGHT), DPI, DPI, PixelFormats.Default);
            bmp.Render(text);
            return bmp;
        }
        private BitmapSource GetImage(SmileyMoods mood, string bodyColor, string lineColor, string auraColor)
        {
            var ctrl = new Smiley();
            ctrl.SmileyColor = (Color)ColorConverter.ConvertFromString(bodyColor);
            ctrl.SmileyLineColor = (Color)ColorConverter.ConvertFromString(lineColor);
            ctrl.SmileyMood = mood;
            ctrl.SmileyRadius = 2;
            ctrl.Width = ModelConstants.CELLWIDTH;
            ctrl.Height = ModelConstants.CELLHEIGHT;
            ctrl.Measure(new Size(ctrl.Width, ctrl.Height));
            ctrl.Arrange(new Rect(ctrl.DesiredSize));
            RenderOptions.SetBitmapScalingMode(ctrl, BitmapScalingMode.Fant);
            var bmp = new RenderTargetBitmap(ModelConstants.CELLWIDTH, ModelConstants.CELLHEIGHT, DPI, DPI, PixelFormats.Default);

            bmp.Render(ctrl);
            return bmp;
        }
        #endregion
    }
}
