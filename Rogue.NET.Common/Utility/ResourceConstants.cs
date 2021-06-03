using System.IO;
using System.Reflection;

namespace Rogue.NET.Common.Utility
{
    public static class ResourceConstants
    {
        public enum ResourcePaths
        {
            RogueFileDatabase,
            SavedGameDirectory,
            ScenarioDirectory,
            TempDirectory,
            DebugOutputDirectory,
            EmbeddedScenarioDirectory
        }

        // Paths
        private const string RogueFileDatabase = "..\\.roguedb";
        private const string SavedGameDirectory = "..\\save";
        private const string ScenarioDirectory = "..\\scenarios";
        private const string TempDirectory = "..\\temp";
        private const string DebugOutputDirectory = "..\\debug-output";
        private const string EmbeddedScenarioDirectory = "..\\..\\..\\Rogue.NET.Common\\Resource\\Configuration";

        // Extensions
        public const string RogueFileDatabaseRecordExtension = "rdb";
        public const string ScenarioExtension = "rdn";
        public const string ScenarioConfigurationExtension = "rdns";

        public static string GetPath(ResourcePaths resource)
        {
            var fileName = Assembly.GetExecutingAssembly().Location;
            var offset = Path.GetDirectoryName(fileName);

            switch (resource)
            {
                case ResourcePaths.RogueFileDatabase:
                    return Path.Combine(offset, RogueFileDatabase);
                case ResourcePaths.SavedGameDirectory:
                    return Path.Combine(offset, SavedGameDirectory);
                case ResourcePaths.ScenarioDirectory:
                    return Path.Combine(offset, ScenarioDirectory);
                case ResourcePaths.TempDirectory:
                    return Path.Combine(offset, TempDirectory);
                case ResourcePaths.DebugOutputDirectory:
                    return Path.Combine(offset, DebugOutputDirectory);
                case ResourcePaths.EmbeddedScenarioDirectory:
                    return Path.Combine(offset, EmbeddedScenarioDirectory);
                default:
                    throw new System.Exception("Unhandled Resource Path:  ResourceConstants.cs");
            }
        }
    }
}
