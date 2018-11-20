using Prism.Events;
using Rogue.NET.Common.Utility;
using Rogue.NET.Common.ViewModel;
using Rogue.NET.Core.IO;
using Rogue.NET.Core.Model;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Model.ScenarioConfiguration;
using Rogue.NET.Core.Service.Interface;
using Rogue.NET.Core.Utility;
using Rogue.NET.Core.View;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
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
        const string EMBEDDED_SCENARIOS_DIR = "..\\..\\..\\Rogue.NET.Common\\Resource\\Configuration";
        const int DPI = 96;

        IList<ScenarioConfigurationContainer> _scenarioConfigurations;

        [ImportingConstructor]
        public ScenarioResourceService(IEventAggregator eventAggregator)
        {
            if (!Directory.Exists(SAVED_GAMES_DIR))
                Directory.CreateDirectory(SAVED_GAMES_DIR);

            if (!Directory.Exists(SCENARIOS_DIR))
                Directory.CreateDirectory(SCENARIOS_DIR);

            _scenarioConfigurations = new List<ScenarioConfigurationContainer>();
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
                var header = ScenarioFile.OpenHeader(File.ReadAllBytes(file));
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
        public void EmbedConfig(ConfigResources configResource, ScenarioConfigurationContainer config)
        {
            var file = Path.Combine(EMBEDDED_SCENARIOS_DIR, configResource.ToString()) + "." + SCENARIO_CONFIG_EXTENSION;
            SerializeToFile(file, config);
        }
        public BitmapSource GetImageSource(ScenarioImage scenarioImage)
        {
            switch (scenarioImage.SymbolType)
            {
                case SymbolTypes.Character:
                    return GetImage(scenarioImage.CharacterSymbol, scenarioImage.CharacterColor);
                case SymbolTypes.Smiley:
                    return GetImage(scenarioImage.SmileyMood, scenarioImage.SmileyBodyColor, scenarioImage.SmileyLineColor, scenarioImage.SmileyAuraColor);
                case SymbolTypes.Image:
                    return GetImage(scenarioImage.Icon);
                default:
                    throw new Exception("Unknown symbol type");
            }
        }
        public BitmapSource GetImageSource(
            string rogueName,
            string symbol, 
            string symbolColor, 
            ImageResources icon, 
            SmileyMoods smileyMood, 
            string smileyBodyColor,
            string smileyLineColor,
            string smileyAuraColor,
            SymbolTypes type)
        {
            switch (type)
            {
                case SymbolTypes.Character:
                    return GetImage(symbol, symbolColor);
                case SymbolTypes.Smiley:
                    return GetImage(smileyMood, smileyBodyColor, smileyLineColor, smileyAuraColor);
                case SymbolTypes.Image:
                    return GetImage(icon);
                default:
                    throw new Exception("Unknown symbol type");
            }
        }

        public void SaveScenarioFile(ScenarioFile scenarioFile, string playerName)
        {
            var buffer = scenarioFile.Save();

            File.WriteAllBytes(SAVED_GAMES_DIR + "\\" + playerName + "." + SCENARIO_EXTENSION, buffer);
        }
        public ScenarioFile OpenScenarioFile(string playerName)
        {
            var buffer = File.ReadAllBytes(SAVED_GAMES_DIR + "\\" + playerName + "." + SCENARIO_EXTENSION);

            return ScenarioFile.Open(buffer);
        }

        public void DeleteScenario(string name)
        {
            var path = Path.Combine(SAVED_GAMES_DIR, name + "." + SCENARIO_EXTENSION);

            if (File.Exists(path))
                File.Delete(path);
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

        #region Image and Color Resources
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
        public BitmapSource GetImage(string symbol, string symbolColor)
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

        public IEnumerable<ColorViewModel> GetColors()
        {
            return ColorUtility.CreateColors();
        }
        #endregion
    }
}
