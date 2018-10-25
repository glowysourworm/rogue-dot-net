using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration;
using Rogue.NET.Core.Service.Interface;
using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Design;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;

namespace Rogue.NET.Core.Service
{
    [Export(typeof(IResourceService))]
    public class ScenarioResourceService : IScenarioResourceService
    {
        const string SAVED_GAMES_DIR = "..\\save";
        const string CMD_PREFS_FILE = "CommandPreferences.rcp";
        const string SCENARIO_EXTENSION = "rdn";
        const string SCENARIO_CONFIG_EXTENSION = "rdns";
        const string SCENARIOS_DIR = "..\\scenarios";

        public ScenarioConfigurationContainer GetEmbeddedScenarioConfiguration(ConfigResources configResource)
        {
            var name = configResource.ToString();
            var assembly = Assembly.GetAssembly(typeof(ScenarioResourceService));
            var location = "Rogue.NET.Core.Resource.Configuration." + name.ToString() + "." + SCENARIO_CONFIG_EXTENSION;
            using (var stream = assembly.GetManifestResourceStream(location))
            {
                var memoryStream = new MemoryStream();
                stream.CopyTo(memoryStream);
                return (ScenarioConfigurationContainer)Deserialize(memoryStream.GetBuffer());
            }
        }
        public void SaveConfig(string name, ScenarioConfigurationContainer config)
        {
            var file = Path.Combine(SCENARIOS_DIR, name) + "." + SCENARIO_CONFIG_EXTENSION;
            SerializeToFile(file, config);
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
        #endregion
    }
}
