using Rogue.NET.Core.Processing.Service.Cache;
using Rogue.NET.UnitTest.Extension;

using System.IO;

namespace Rogue.NET.UnitTest
{
    public static class TestInitialization
    {
        public static void Initialize()
        {
            // Create Game Directories (USE EXECUTING ASSEMBLY)
            if (!Directory.Exists(TestParameters.SavedGameDirectory))
                Directory.CreateDirectory(TestParameters.SavedGameDirectory);

            if (!Directory.Exists(TestParameters.ScenarioDirectory))
                Directory.CreateDirectory(TestParameters.ScenarioDirectory);

            if (!Directory.Exists(TestParameters.TempDirectory))
                Directory.CreateDirectory(TestParameters.TempDirectory);

            if (!Directory.Exists(TestParameters.DebugOutputDirectory))
                Directory.CreateDirectory(TestParameters.DebugOutputDirectory);

            // Load configurations from embedded resources
            ScenarioConfigurationCache.Load();

            var bootstrapper = new UnitTestBootstrapper();

            bootstrapper.Run();
        }

        public static void Cleanup()
        {
            Directory.Delete(TestParameters.SavedGameDirectory, true);
            Directory.Delete(TestParameters.ScenarioDirectory, true);
            Directory.Delete(TestParameters.TempDirectory, true);
            Directory.Delete(TestParameters.DebugOutputDirectory, true);
        }
    }
}
