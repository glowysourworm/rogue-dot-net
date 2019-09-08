using Rogue.NET.Core.Model.Enums;

namespace Rogue.NET.Core.Processing.Command.View.CommandData
{
    public class ViewCommandData
    {
        public ViewActionType ViewAction { get; set; }

        public ViewCommandData(ViewActionType action)
        {
            this.ViewAction = action;
        }
    }
}
