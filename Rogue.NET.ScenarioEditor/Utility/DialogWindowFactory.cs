using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.Views.Controls;
using Rogue.NET.ScenarioEditor.Views.Controls.Symbol;
using System;
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
        const int MIN_WINDOW_HEIGHT = 250;
        const int MIN_WINDOW_WIDTH = 400;

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

        public static bool ShowSymbolEditor(SymbolDetailsTemplateViewModel symbolDetails)
        {
            var view = new SymbolEditor();
            var size = CalculateWindowSize();
            view.DataContext = symbolDetails;
            view.WindowMode = true;
            view.AllowSymbolRandomization = false;
            view.Width = size.Width;
            view.Height = size.Height;

            return Show(view, "Rogue Symbol Editor");
        }

        private static Size CalculateWindowSize()
        {
            return new Size(Math.Max(Application.Current.MainWindow.Width * 0.667, MIN_WINDOW_WIDTH),
                            Math.Max(Application.Current.MainWindow.Height * 0.667, MIN_WINDOW_HEIGHT));
        }
    }
}
