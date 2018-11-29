using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interactivity;

namespace Rogue.NET.Common.Extension
{
    public class CustomDialogResultTrigger : System.Windows.Interactivity.TriggerAction<DependencyObject>
    {
        public static readonly DependencyProperty TargetWindowProperty =
            DependencyProperty.Register("TargetWindow", typeof(DependencyObject), typeof(CustomDialogResultTrigger));

        public static readonly DependencyProperty TargetDialogResultProperty =
            DependencyProperty.Register("TargetDialogResult", typeof(bool?), typeof(CustomDialogResultTrigger));


        public DependencyObject TargetWindow
        {
            get { return (DependencyObject)GetValue(TargetWindowProperty); }
            set { SetValue(TargetWindowProperty, value); }
        }

        protected override void Invoke(object parameter)
        {
            if (this.TargetWindow != null)
            {
                var window = this.TargetWindow as Window;
                window.Hide();

                // NOTE*** THIS IS SET BY ANOTHER PART OF THE APPLICATION. THIS WAS NECESSARY.....
                Application.Current.MainWindow.IsEnabled = true;
            }
        }
    }
}
