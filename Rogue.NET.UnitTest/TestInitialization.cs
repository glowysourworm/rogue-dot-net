using Rogue.NET.Common.Utility;

using System.IO;
using System.Reflection;

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
