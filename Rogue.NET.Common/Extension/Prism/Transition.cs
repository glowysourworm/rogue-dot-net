using System.Windows.Controls;

namespace Rogue.NET.Common.Extension.Prism
{
    public abstract class Transition
    {
        public virtual void BeginTransition(TransitionPresenter transitionElement, UserControl oldContent, UserControl newContent)
        {

        }
        public virtual void EndTransition(TransitionPresenter transitionElement, UserControl oldContent, UserControl newContent)
        {

        }
    }
}
