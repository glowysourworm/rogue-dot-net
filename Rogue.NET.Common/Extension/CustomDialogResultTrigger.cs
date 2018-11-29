using Rogue.NET.Common.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using System.Windows.Media;

namespace Rogue.NET.Common.Extension
{
    public class CustomDialogResultTrigger : System.Windows.Interactivity.TriggerAction<DependencyObject>
    {
        public static readonly DependencyProperty DialogResultProperty = DependencyProperty.
            Register(
                "DialogResult",
                typeof(bool?),
                typeof(CustomDialogResultTrigger));

        public static readonly DependencyProperty IsDialogResultEnabledProperty = DependencyProperty.
            Register(
                "IsDialogResultEnabled",
                typeof(bool),
                typeof(CustomDialogResultTrigger));

        public bool? DialogResult
        {
            get { return (bool?)GetValue(DialogResultProperty); }
            set { SetValue(DialogResultProperty, value); }
        }
        public bool IsDialogResultEnabled
        {
            get { return (bool)GetValue(IsDialogResultEnabledProperty); }
            set { SetValue(IsDialogResultEnabledProperty, value); }
        }
        protected override void Invoke(object parameter)
        {
            // NOTE*** THIS IS SET BY ANOTHER PART OF THE APPLICATION. THIS WAS NECESSARY.....
            var window = Application.Current.MainWindow.Tag as Window;
            window.DialogResult = this.DialogResult;
        }
    }
}
