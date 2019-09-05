using Rogue.NET.Common.Extension.Event;
using Rogue.NET.Core.Event.Scenario.Level.EventArgs;
using Rogue.NET.Core.Logic.Processing.Interface;

namespace Rogue.NET.Scenario.Content.Views.Dialog.Interface
{
    /// <summary>
    /// Interface that specifies a way to load / send data to / from a user control
    /// from / to the primary scenario module. This is used during a dialog event sequence
    /// </summary>
    public interface IDialogContainer
    {
        event SimpleEventHandler<IDialogContainer, UserCommandEventArgs> DialogFinishedEvent;

        /// <summary>
        /// Initializes the control with a dialog update (from the backend)
        /// </summary>
        void Initialize(IDialogUpdate update);
    }
}
