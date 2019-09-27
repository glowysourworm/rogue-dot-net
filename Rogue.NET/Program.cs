using System;
using System.IO;
using Rogue.NET.Common.Utility;
using Rogue.NET.Core.Model.ResourceCache;
using Rogue.NET.ModalDialog;
using Rogue.NET.Core.Processing.Event.Dialog.Enum;
using Rogue.NET.Core.Processing.Symbol;

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

            var loadingWindow = SplashWindowFactory.CreatePopupWindow(SplashEventType.Loading);

            // Show loading window - allow primary thread to process
            loadingWindow.Show();

            // Load all configurations prior to running application
            ScenarioConfigurationCache.Load();

            // Load SVG Resources
            SvgCache.Load();

            // Hide loading window
            loadingWindow.Hide();

            var application = new RogueApplication();

            application.Run();
        }
    }
}
