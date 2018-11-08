using Prism.Events;
using Rogue.NET.Core.Model.Enums;

namespace Rogue.NET.ScenarioEditor.Events
{
    public class AddAttackAttributeEventArgs : System.EventArgs
    {
        public string Name { get; set; }
        public ImageResources Icon { get; set; }
    }

    public class AddAttackAttributeEvent : PubSubEvent<AddAttackAttributeEventArgs>
    {
    }
}
