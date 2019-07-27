using Rogue.NET.Core.Model.Enums;

namespace Rogue.NET.Core.Event.Scenario.Level.EventArgs
{
    public class ViewCommandEventArgs : UserCommandEventArgs
    {
        public ViewActionType ViewAction { get; set; }

        public ViewCommandEventArgs(ViewActionType action)
        {
            this.ViewAction = action;
        }
    }
}
