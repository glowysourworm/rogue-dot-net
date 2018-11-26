using System.Windows;
using System.Windows.Controls;

namespace Rogue.NET.Common.Extension
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
                    if (window != null && System.Windows.Interop.ComponentDispatcher.IsThreadModal)
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