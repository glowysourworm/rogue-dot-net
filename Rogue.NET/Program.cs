using System;
using System.IO;
using Rogue.NET.Common.Utility;

namespace Rogue.NET
{
    public static class Program
    {
        [STAThread]
        public static void Main()
        {
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
