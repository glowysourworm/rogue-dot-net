using System;
using System.IO;
using Rogue.NET.Common.Utility;
using Rogue.NET.Utility;

namespace Rogue.NET
{
    public static class Program
    {
        [STAThread]
        public static void Main()
        {
            // Initialize Express Mapper
            MapperInit.Initialize();

            // Create Game Directories
            if (!Directory.Exists(ResourceConstants.SavedGameDirectory))
                Directory.CreateDirectory(ResourceConstants.SavedGameDirectory);

            if (!Directory.Exists(ResourceConstants.ScenarioDirectory))
                Directory.CreateDirectory(ResourceConstants.ScenarioDirectory);

            var application = new RogueApplication();

            application.Run();
        }
    }
}
