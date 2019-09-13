using Rogue.NET.Core.Processing.Event.Dialog.Enum;
using Rogue.NET.View;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Rogue.NET.ModalDialog
{
    public static class SplashWindowFactory
    {
        public static Window CreatePopupWindow(SplashEventType type)
        {
            return new Window()
            {
                Content = CreateSplashView(type),

                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                SizeToContent = SizeToContent.WidthAndHeight,
                AllowsTransparency = true,
                WindowStyle = WindowStyle.None,
                ResizeMode = ResizeMode.NoResize,
                Background = Brushes.Transparent,
                BorderBrush = Brushes.Transparent,
                BorderThickness = new Thickness(0),
                Margin = new Thickness(0),
                FontFamily = new FontFamily(new Uri(@"pack://application:,,,/Rogue.NET.Common;Component/Resource/Fonts/CENTAUR.TTF#Centaur"), "Centaur"),
                Topmost = true
            };
        }

        private static UserControl CreateSplashView(SplashEventType type)
        {
            // Passing these in to take care of dependency injection. Another way is to create
            // multiple region managers and have a separate Shell window.
            switch (type)
            {
                case SplashEventType.Loading:
                    return new LoadingView();
                default:
                    throw new Exception("Unknwon Splash View Type");
            }
        }
    }
}
