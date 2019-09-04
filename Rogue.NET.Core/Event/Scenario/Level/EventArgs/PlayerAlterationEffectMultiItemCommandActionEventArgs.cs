using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Event.Scenario.Level.EventArgs
{
    public class PlayerAlterationEffectMultiItemCommandActionEventArgs : PlayerMultiItemCommandEventArgs
    {
        public IAlterationEffect Effect { get; set; }

        public PlayerAlterationEffectMultiItemCommandActionEventArgs(IAlterationEffect effect, PlayerMultiItemActionType action, params string[] itemIds)
            : base(action, itemIds)
        {
            this.Effect = effect;
        }
    }
}
