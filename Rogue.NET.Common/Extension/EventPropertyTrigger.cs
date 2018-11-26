using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interactivity;

namespace Rogue.NET.Common.Extension
{
    public class EventPropertyTrigger : System.Windows.Interactivity.TriggerAction<DependencyObject>
    {
        public static readonly DependencyProperty TargetWindowProperty =
            DependencyProperty.Register("TargetWindow", typeof(DependencyObject), typeof(EventPropertyTrigger));

        public static readonly DependencyProperty TargetDialogResultProperty =
            DependencyProperty.Register("TargetDialogResult", typeof(bool?), typeof(EventPropertyTrigger));


        public DependencyObject TargetWindow
        {
            get { return (DependencyObject)GetValue(TargetWindowProperty); }
            set { SetValue(TargetWindowProperty, value); }
        }
        public bool? TargetDialogResult
        {
            get { return (bool?)GetValue(TargetDialogResultProperty); }
            set { SetValue(TargetDialogResultProperty, value); }
        }

        protected override void Invoke(object parameter)
        {
            if (this.TargetWindow != null)
            {
                // Set using reflection
                var propertyInfo = this.TargetWindow
                                       .GetType()
                                       .GetProperty("DialogResult");
                propertyInfo.SetValue(this.TargetWindow, this.TargetDialogResult);
            }
        }
    }
}
