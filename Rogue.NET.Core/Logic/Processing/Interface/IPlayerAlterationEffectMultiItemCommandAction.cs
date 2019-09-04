using Rogue.NET.Core.Model.Scenario.Alteration.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Logic.Processing.Interface
{
    public interface IPlayerAlterationEffectMultiItemCommandAction : IPlayerMultiItemCommandAction
    {
        IAlterationEffect Effect { get; set; }
    }
}
