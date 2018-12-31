using Prism.Events;
using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Content;

namespace Rogue.NET.ScenarioEditor.Events
{
    public class AddCombatAttributeEvent : RogueEvent<CombatAttributeTemplateViewModel>
    {
    }
}
