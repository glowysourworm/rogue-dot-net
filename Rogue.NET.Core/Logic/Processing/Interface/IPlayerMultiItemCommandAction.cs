using Rogue.NET.Core.Model.Enums;

using System.Collections.Generic;

namespace Rogue.NET.Core.Logic.Processing.Interface
{
    public interface IPlayerMultiItemCommandAction
    {
        PlayerMultiItemActionType Type { get; set; }

        /// <summary>
        /// Id's for the items selected
        /// </summary>
        IEnumerable<string> ItemIds { get; set; }
    }
}
