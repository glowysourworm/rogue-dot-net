using System.Windows;
using System.Windows.Controls;

namespace Rogue.NET.Common.Extension.Prism.RegionManager
{
    public abstract class Transition
    {
        public virtual void BeginTransition(RogueRegion transitionElement, FrameworkElement oldContent, FrameworkElement newContent)
        {

        }
        public virtual void EndTransition(RogueRegion transitionElement, FrameworkElement oldContent, FrameworkElement newContent)
        {

        }
    }
}
