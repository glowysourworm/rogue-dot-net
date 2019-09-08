using Rogue.NET.Common.Extension.Event;

namespace Rogue.NET.Scenario.Content.Views.Dialog.Interface
{
    /// <summary>
    /// Interface that specifies a way to alert listeners that the primary dialog
    /// sequence is done
    /// </summary>
    public interface IDialogView
    {
        /// <summary>
        /// Fires event to notify listener that the dialog has finished - object represents dialog view data
        /// </summary>
        event SimpleEventHandler<IDialogView, object> DialogViewFinishedEvent;
    }
}
