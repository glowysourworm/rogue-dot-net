using Rogue.NET.Common.Utility;
using Rogue.NET.Core.Processing.Event.Dialog.Enum;
using Rogue.NET.Core.Processing.Service.Cache;
using Rogue.NET.ModalDialog;
using System;
using System.IO;

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

            // Load all saved games
            ScenarioCache.Load();

            // Load SVG Resources
            SvgCache.Load();

            // Hide loading windowViewModel.Browser.ScenarioAssetViewModel
            loadingWindow.Hide();

            var application = new RogueApplication();

            application.Run();
        }
    }
}
