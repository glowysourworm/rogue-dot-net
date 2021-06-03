using Rogue.NET.Common.Utility;
using Rogue.NET.Core.Processing.Event.Dialog.Enum;
using Rogue.NET.Core.Processing.Service.Cache;
using Rogue.NET.ModalDialog;
using System;
using System.IO;
using System.Windows;

namespace Rogue.NET
{
    public static class Program
    {
        [STAThread]
        public static void Main()
        {
            // Create Game Directories
            if (!Directory.Exists(ResourceConstants.GetPath(ResourceConstants.ResourcePaths.SavedGameDirectory)))
                Directory.CreateDirectory(ResourceConstants.GetPath(ResourceConstants.ResourcePaths.SavedGameDirectory));

            if (!Directory.Exists(ResourceConstants.GetPath(ResourceConstants.ResourcePaths.DebugOutputDirectory)))
                Directory.CreateDirectory(ResourceConstants.GetPath(ResourceConstants.ResourcePaths.DebugOutputDirectory));

            if (!Directory.Exists(ResourceConstants.GetPath(ResourceConstants.ResourcePaths.ScenarioDirectory)))
                Directory.CreateDirectory(ResourceConstants.GetPath(ResourceConstants.ResourcePaths.ScenarioDirectory));

            if (!Directory.Exists(ResourceConstants.GetPath(ResourceConstants.ResourcePaths.TempDirectory)))
                Directory.CreateDirectory(ResourceConstants.GetPath(ResourceConstants.ResourcePaths.TempDirectory));

            var loadingWindow = SplashWindowFactory.CreatePopupWindow(SplashEventType.Loading);

            // Show loading window - allow primary thread to process
            loadingWindow.Show();

            // Load all configurations prior to running application
            try
            {
                ScenarioConfigurationCache.Load();
            }
            catch (Exception)
            {
                MessageBox.Show("There was an error loading Scenario Configurations!");
                return;
            }

            // Load all saved games
            try
            {
                ScenarioCache.Load();
            }
            catch (Exception)
            {
                MessageBox.Show("There was an error loading saved games!");
                return;
            }


            // Load SVG Resources - These are all embedded resources - Let it crash here if necessary
            SvgCache.Load();

            // Hide loading windowViewModel.Browser.ScenarioAssetViewModel
            loadingWindow.Hide();

            var application = new RogueApplication();

            application.Run();
        }
    }
}
