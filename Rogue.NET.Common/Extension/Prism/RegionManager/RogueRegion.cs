using System.Windows;
using System.Windows.Controls;

namespace Rogue.NET.Common.Extension.Prism.RegionManager
{
    /// <summary>
    /// Base class to identify a region for the region manager to collect and manage.
    /// </summary>
    public class RogueRegion : ContentControl
    {
        public static readonly DependencyProperty TransitionProperty = 
            DependencyProperty.Register("Transition", typeof(Transition), typeof(RogueRegion));

        public Transition Transition
        {
            get { return (Transition)GetValue(TransitionProperty); }
            set { SetValue(TransitionProperty, value); }
        }

        public void TransitionTo(FrameworkElement content)
        {
            if (this.Content == content)
                return;

            if (this.Content == null ||
                this.Transition == null)
                this.Content = content;

            else
                this.Transition.BeginTransition(this, this.Content as FrameworkElement, content);
        }

        public override string ToString()
        {
            return this.Name ?? "" + " " + this.GetHashCode().ToString();
        }
    }
}
