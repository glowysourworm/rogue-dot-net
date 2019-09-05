using Rogue.NET.Common.Extension.Event;
using System.Collections.Generic;

namespace Rogue.NET.Scenario.Content.Views.Dialog.Interface
{
    /// <summary>
    /// Interface that specifies a way to alert listeners that the primary dialog
    /// sequence is done
    /// </summary>
    public interface IDialogView
    {
        /// <summary>
        /// Fires event to notify listener that the dialog has finished
        /// </summary>
        event SimpleEventHandler<IDialogView> DialogViewFinishedEvent;

        /// <summary>
        /// Returns RogueBase.Id property of selected objects
        /// </summary>
        IEnumerable<string> GetSelectedItemIds();
    }
}
