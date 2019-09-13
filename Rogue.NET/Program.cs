using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Rogue.NET.Common.Utility;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ResourceCache;
using Rogue.NET.Core.Model.ScenarioConfiguration;
using Rogue.NET.Core.Processing.Event.Dialog.Enum;
using Rogue.NET.ModalDialog;

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

            // Hide loading window
            loadingWindow.Hide();

            var application = new RogueApplication();

            application.Run();
        }
    }
}
