using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Rogue.NET.Common.Utility
{
    public static class WpfHelper
    {
        public static T FindAncestor<T>(DependencyObject dependencyObject) where T : DependencyObject
        {
            DependencyObject parent = null, control = dependencyObject;

            while ((control = VisualTreeHelper.GetParent(control)) != null)
            {
                if (control is T)
                {
                    parent = control;
                    break;
                }
            }

            return (T)parent;
        }
    }
}
