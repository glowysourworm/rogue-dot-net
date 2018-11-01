using System.Windows;
using System.Windows.Controls;

namespace Rogue.NET.Common.Extension.Prism
{
    public class TransitionPresenter : ContentPresenter
    {
        public static readonly DependencyProperty TransitionProperty = DependencyProperty.Register("Transition", typeof(Transition), typeof(TransitionPresenter));

        public Transition Transition
        {
            get { return (Transition)GetValue(TransitionProperty); }
            set { SetValue(TransitionProperty, value); }
        }
    }
}