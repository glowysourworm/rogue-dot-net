using Prism.Events;
using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.Core.Model.Enums;

namespace Rogue.NET.ScenarioEditor.Events
{
    public class AddAttackAttributeEventArgs : System.EventArgs
    {
        public string Name { get; set; }
        public ImageResources Icon { get; set; }
    }

    public class AddAttackAttributeEvent : RogueEvent<AddAttackAttributeEventArgs>
    {
    }
}
