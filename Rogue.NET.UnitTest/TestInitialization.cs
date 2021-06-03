using Rogue.NET.Common.Utility;
using Rogue.NET.Core.Processing.Service;
using Rogue.NET.Core.Processing.Service.Cache;
using Rogue.NET.UnitTest.Extension;

using System.IO;
using System.Reflection;

namespace Rogue.NET.UnitTest
{
    public static class TestInitialization
    {
        public static void Initialize()
        {
            // Create Game Directories (USE EXECUTING ASSEMBLY)
            if (!Directory.Exists(ResourceConstants.GetPath(ResourceConstants.ResourcePaths.SavedGameDirectory)))
                Directory.CreateDirectory(ResourceConstants.GetPath(ResourceConstants.ResourcePaths.SavedGameDirectory));

            if (!Directory.Exists(ResourceConstants.GetPath(ResourceConstants.ResourcePaths.ScenarioDirectory)))
                Directory.CreateDirectory(ResourceConstants.GetPath(ResourceConstants.ResourcePaths.ScenarioDirectory));

            if (!Directory.Exists(ResourceConstants.GetPath(ResourceConstants.ResourcePaths.TempDirectory)))
                Directory.CreateDirectory(ResourceConstants.GetPath(ResourceConstants.ResourcePaths.TempDirectory));

            if (!Directory.Exists(ResourceConstants.GetPath(ResourceConstants.ResourcePaths.DebugOutputDirectory)))
                Directory.CreateDirectory(ResourceConstants.GetPath(ResourceConstants.ResourcePaths.DebugOutputDirectory));

            // Load configurations from embedded resources
            ScenarioConfigurationCache.Load();

            var bootstrapper = new UnitTestBootstrapper();

            bootstrapper.Run();
        }

        public static void Cleanup()
        {
            Directory.Delete(ResourceConstants.GetPath(ResourceConstants.ResourcePaths.SavedGameDirectory), true);
            Directory.Delete(ResourceConstants.GetPath(ResourceConstants.ResourcePaths.ScenarioDirectory), true);
            Directory.Delete(ResourceConstants.GetPath(ResourceConstants.ResourcePaths.TempDirectory), true);
            Directory.Delete(ResourceConstants.GetPath(ResourceConstants.ResourcePaths.DebugOutputDirectory), true);
            File.Delete(ResourceConstants.GetPath(ResourceConstants.ResourcePaths.RogueFileDatabase));
        }
    }
}
