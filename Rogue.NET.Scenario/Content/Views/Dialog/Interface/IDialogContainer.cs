using Rogue.NET.Common.Extension.Event;
using Rogue.NET.Core.Processing.Event.Backend.EventData;

namespace Rogue.NET.Scenario.Content.Views.Dialog.Interface
{
    /// <summary>
    /// Interface that specifies a way to load / send data to / from a user control
    /// from / to the primary scenario module. This is used during a dialog event sequence
    /// </summary>
    public interface IDialogContainer
    {
        event SimpleEventHandler<IDialogContainer, object> DialogFinishedEvent;

        /// <summary>
        /// Initializes the control with a dialog update (from the backend)
        /// </summary>
        void Initialize(DialogEventData eventData);
    }
}
