using Rogue.NET.Core.Model.Scenario.Alteration.Interface;

namespace Rogue.NET.Core.Processing.Event.Backend.EventData
{
    public class DialogAlterationEffectEventData : DialogEventData
    {
        public IAlterationEffect Effect { get; set; }
    }
}
