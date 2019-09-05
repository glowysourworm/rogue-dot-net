using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Event.Scenario.Level.EventArgs
{
    public class PlayerAlterationEffectMultiItemCommandEventArgs : PlayerMultiItemCommandEventArgs
    {
        public IAlterationEffect Effect { get; set; }

        public PlayerAlterationEffectMultiItemCommandEventArgs(IAlterationEffect effect, PlayerMultiItemActionType action, string[] itemIds)
            : base(action, itemIds)
        {
            this.Effect = effect;
        }
    }
}
