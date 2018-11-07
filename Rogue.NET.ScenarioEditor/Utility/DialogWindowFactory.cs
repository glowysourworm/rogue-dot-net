using Rogue.NET.ScenarioEditor.Views.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Rogue.NET.ScenarioEditor.Utility
{
    /// <summary>
    /// Created to have consistent dialog window settings - theme wasn't applied to child window. This will load
    /// the window and show it with the supplied view - using a standard title bar.
    /// </summary>
    public static class DialogWindowFactory
    {
        public static bool Show(UserControl view, string title)
        {
            var window = new Window();
            window.Resources = Application.Current.MainWindow.Resources;
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            window.WindowStyle = WindowStyle.None;
            window.SizeToContent = SizeToContent.WidthAndHeight;
            window.ResizeMode = ResizeMode.NoResize;
            window.AllowsTransparency = true;
            window.Background = Brushes.Transparent;
            window.Title = title;

            var dialogView = new DialogView();
            dialogView.ContentPresenter.Content = view;
            dialogView.TitleTB.Text = title;

            window.Content = dialogView;

            return (bool)window.ShowDialog();
        }
    }
}
