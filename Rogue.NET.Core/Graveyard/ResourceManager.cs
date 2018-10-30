//using System;
//using System.IO;
//using System.Reflection;
//using System.Windows.Media.Imaging;
//using System.Windows.Controls;
//using System.Windows.Media;
//using System.Windows;
//using System.Runtime.Serialization;
//using System.Runtime.Serialization.Formatters.Binary;
//using System.Globalization;
//using System.IO.Compression;
//using System.Management;
//using System.Security.Cryptography;
//using System.Xml.Serialization;
//using System.Windows.Markup;
//using Rogue.NET.Scenario.Model;
//using Rogue.NET.Common;
//using Rogue.NET.Common.Views;
//using Rogue.NET.Model;

//namespace Rogue.NET.Model
//{
//    public static class ResourceManager
//    {
//        static double DPI = 96;

//        readonly static ISerializer _serializer;

//        static ResourceManager()
//        {
//            _serializer = new BinarySerializer();
//        }

//        public static ScenarioConfiguration GetEmbeddedScenarioConfiguration(ConfigResources res)
//        {
//            var name = res.ToString();
//            var assembly = Assembly.GetAssembly(typeof(Constants));
//            var location = "Rogue.NET.Common.Resources.Configuration." + name.ToString() + "." + Constants.SCENARIO_CONFIG_EXTENSION;
//            using (var stream = assembly.GetManifestResourceStream(location))
//            {
//                var memoryStream = new MemoryStream();
//                stream.CopyTo(memoryStream);
//                return (ScenarioConfiguration)_serializer.Deserialize(memoryStream.GetBuffer());
//            }
//        }
//        public static ScenarioConfiguration OpenScenarioConfigurationFile(string name)
//        {
//            var file = Path.Combine(Constants.SCENARIOS_DIR, name) + "." + Constants.SCENARIO_CONFIG_EXTENSION;
//            return (ScenarioConfiguration)DeserializeFromFile(file);
//        }

//        public static void SaveConfig(string name, ScenarioConfiguration config)
//        {
//            var file = Path.Combine(Constants.SCENARIOS_DIR, name) + "." + Constants.SCENARIO_CONFIG_EXTENSION;
//            SerializeToFile(file, config, true);
//        }

//        public static CommandPreferences GetCommandPreferences()
//        {
//            if (!File.Exists(Constants.CMD_PREFS_FILE))
//                return CommandPreferences.GetDefaults();

//            else
//                return DeserializeFromXaml<CommandPreferences>(Constants.CMD_PREFS_FILE);
//        }
//        public static void SaveCommandPreferences(CommandPreferences commandPrefs)
//        {
//            SerializeToXaml(Constants.CMD_PREFS_FILE, commandPrefs);
//        }

//        public static BitmapSource GetScenarioObjectImage(ImageResources img)
//        {
//            try
//            {
//                if (img == ImageResources.WellYellowCopy)
//                    img = ImageResources.WellYellow;

//                var path = "Rogue.NET.Common.Resources.Images.ScenarioObjects." + img.ToString() + ".png";
//                var a = Assembly.GetAssembly(typeof(Constants));
//                var s = a.GetManifestResourceStream(path);
//                var decoder = new PngBitmapDecoder(s, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
//                return decoder.Frames[0];
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show(ex.ToString());
//                throw;
//            }
//        }
//        public static BitmapSource GetScenarioObjectImage(ImageResources img, double scale)
//        {
//            var path = "Rogue.NET.Common.Resources.Images.ScenarioObjects." + img.ToString() + ".png";
//            var a = Assembly.GetAssembly(typeof(Constants));
//            var s = a.GetManifestResourceStream(path);
//            var decoder = new PngBitmapDecoder(s, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
//            var scaledBitmap = new TransformedBitmap(decoder.Frames[0], new ScaleTransform(scale, scale));
//            return scaledBitmap;
//        }
//        public static BitmapSource GetScenarioObjectImage(Color foregroundColor, string symbol, double scale)
//        {
//            TextBlock text = new TextBlock();

//            text.Foreground = new SolidColorBrush(foregroundColor);
//            text.Background = Brushes.Transparent;
//            text.Text = symbol;
//            text.FontSize = 12 * scale;
//            text.FontFamily = Application.Current.MainWindow.FontFamily;
//            text.Height = ScenarioConfiguration.CELLHEIGHT * scale;
//            text.Width = ScenarioConfiguration.CELLWIDTH * scale;
//            text.Measure(new Size(text.Width, text.Height));
//            text.Arrange(new Rect(text.DesiredSize));

//            RenderTargetBitmap bmp = new RenderTargetBitmap((int)(ScenarioConfiguration.CELLWIDTH * scale), (int)(ScenarioConfiguration.CELLHEIGHT * scale), DPI, DPI, PixelFormats.Default);
//            bmp.Render(text);
//            return bmp;
//        }
//        public static BitmapSource GetSmileyImage(Color bodyColor, Color lineColor, SmileyMoods moods, double scale)
//        {
//            Smiley ctrl = new Smiley();
//            ctrl.SmileyColor = bodyColor;
//            ctrl.SmileyLineColor = lineColor;
//            ctrl.SmileyMood = moods;
//            ctrl.SmileyRadius = 2;
//            ctrl.Width = ScenarioConfiguration.CELLWIDTH * scale;
//            ctrl.Height = ScenarioConfiguration.CELLHEIGHT * scale;
//            ctrl.Measure(new Size(ctrl.Width, ctrl.Height));
//            ctrl.Arrange(new Rect(ctrl.DesiredSize));
//            RenderOptions.SetBitmapScalingMode(ctrl, BitmapScalingMode.Fant);
//            //RenderOptions.SetEdgeMode(ctrl, EdgeMode.Unspecified);
//            RenderTargetBitmap bmp = new RenderTargetBitmap((int)(ScenarioConfiguration.CELLWIDTH * scale), (int)(ScenarioConfiguration.CELLHEIGHT * scale), DPI, DPI, PixelFormats.Default);
            
//            bmp.Render(ctrl);
//            return bmp;
//        }
//        public static BitmapSource GetSmileyImage(string bodyColor, string lineColor, SmileyMoods moods, double scale)
//        {
//            return GetSmileyImage((Color)ColorConverter.ConvertFromString(bodyColor), (Color)ColorConverter.ConvertFromString(lineColor), moods, scale);
//        }
//        public static FontFamily GetApplicationFontFamily()
//        {
//            return Application.Current.MainWindow.FontFamily;
//        }

//        #region Serialization
//        private static void SerializeToFile(string file, object obj, bool useCopy)
//        {
//            try
//            {
//                if (File.Exists(file))
//                    File.Delete(file);

//                using (FileStream fs = File.OpenWrite(file))
//                {
//                   // if (useCopy)
//                   //     obj = _oxSerializer.Copy(obj);

//                    var bytes = _serializer.Serialize(obj);
//                    fs.Write(bytes, 0, bytes.Length);
//                }
                
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show(ex.ToString());
//                throw;
//            }
//        }
//        private static object DeserializeFromFile(string file)
//        {
//            try
//            {
//                using (var fileStream = File.OpenRead(file))
//                {
//                    var memoryStream = new MemoryStream();
//                    fileStream.CopyTo(memoryStream);
//                    return _serializer.Deserialize(memoryStream.GetBuffer());
//                }
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show(ex.ToString());
//                throw;
//            }
//        }

//        public static byte[] SerializeToCompressedBuffer(object obj)
//        {
//            try
//            {
//                return _serializer.SerializeAndCompress(obj);
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show(ex.ToString());
//                throw;
//            }
//        }
//        public static T DeserializeCompressedBuffer<T>(byte[] buffer)
//        {
//            try
//            {
//                return (T)_serializer.DeserializeAndDecompress(buffer);
//            }
                 
//            catch (Exception ex)
//            {
//                MessageBox.Show(ex.ToString());
//                throw;
//            }
//        }

//        private static bool SerializeToXaml(string file, object obj)
//        {
//            try
//            {
//                if (File.Exists(file))
//                    File.Delete(file);

//                using (FileStream fs = File.OpenWrite(file))
//                {
//                    XamlWriter.Save(obj, fs);
//                }
//                return true;
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show(ex.ToString());
//                throw;
//            }
//        }
//        private static T DeserializeFromXaml<T>(string file)
//        {
//            try
//            {
//                using (FileStream fs = File.OpenRead(file))
//                {
//                    return (T)XamlReader.Load(fs);
//                }
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show(ex.ToString());
//                throw;
//            }
//        }

//        public static object CreateDeepCopy(object source)
//        {
//            try
//            {
//                var buffer = _serializer.Serialize(source);
//                return _serializer.Deserialize(buffer);
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show(ex.ToString());
//                throw;
//            }
//        }
//        #endregion
//    }
//}
