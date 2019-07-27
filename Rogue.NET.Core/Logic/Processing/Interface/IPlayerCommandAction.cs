using Rogue.NET.Core.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Logic.Processing.Interface
{
    public interface IPlayerCommandAction
    {
        PlayerActionType Type { get; set; }

        /// <summary>
        /// Id of the involved skill or item
        /// </summary>
        string Id { get; set; }
    }
}
