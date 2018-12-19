using Prism.Events;
using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;

namespace Rogue.NET.ScenarioEditor.Events
{
    public class AddAttackAttributeEventArgs : System.EventArgs
    {
        public string Name { get; set; }
        public SymbolDetailsTemplateViewModel SymbolDetails { get; set; }
    }

    public class AddAttackAttributeEvent : RogueEvent<AddAttackAttributeEventArgs>
    {
    }
}
