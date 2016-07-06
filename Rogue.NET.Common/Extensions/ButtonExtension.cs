using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Rogue.NET.Common.Extensions
{
    public class ButtonExtension
    {
        public static readonly DependencyProperty DialogResultProperty = DependencyProperty.
            RegisterAttached(
                "DialogResult",
                typeof(bool?),
                typeof(ButtonExtension),
                new UIPropertyMetadata(null, new PropertyChangedCallback(OnDialogResultChanged)));

        public static void SetDialogResult(UIElement element, bool value)
        {
            element.SetValue(DialogResultProperty, value);
        }
        public static bool GetDialogResult(UIElement element)
        {
            return (bool)element.GetValue(DialogResultProperty);
        }
        private static void OnDialogResultChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var button = d as Button;
            if (button != null)
            {
                button.Loaded += (obj, ev) =>
                {
                    var window = Window.GetWindow(button);
                    if (window != null)
                    {
                        button.Click += (sender, eve) =>
                        {
                            window.DialogResult = GetDialogResult(button);
                        };
                    }
                };
            }
        }
    }
}
