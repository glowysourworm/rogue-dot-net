using System.Windows.Controls;

namespace Rogue.NET.PrismExtension
{
    public abstract class Transition
    {
        protected virtual void BeginTransition(TransitionPresenter transitionElement, ContentPresenter oldContent, ContentPresenter newContent)
        {

        }
        protected virtual void EndTransition(TransitionPresenter transitionElement, ContentPresenter oldContent, ContentPresenter newContent)
        {

        }

    }
}
