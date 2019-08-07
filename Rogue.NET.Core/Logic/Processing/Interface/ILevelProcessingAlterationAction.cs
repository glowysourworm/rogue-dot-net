using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Logic.Processing.Interface
{
    public interface ILevelProcessingAlterationAction<TAlteration> : ILevelProcessingAction
    {
        TAlteration Alteration { get; set; }
    }
}
