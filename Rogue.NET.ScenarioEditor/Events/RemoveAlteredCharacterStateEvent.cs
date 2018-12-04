using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.ScenarioEditor.Events
{
    public class RemoveAlteredCharacterStateEvent : RogueEvent<AlteredCharacterStateTemplateViewModel>
    {
    }
}
