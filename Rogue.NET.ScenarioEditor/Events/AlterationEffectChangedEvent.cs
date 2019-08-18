using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.ScenarioEditor.Events
{
    public class AlterationEffectChangedEventArgs : EventArgs
    {
        /// <summary>
        /// The new IAlterationEffectTemplateViewModel instance
        /// </summary>
        public IAlterationEffectTemplateViewModel Effect { get; set; }

        /// <summary>
        /// Alteration that is the container for the alteration IAlterationEffectTemplateViewModel effect
        /// </summary>
        public object Alteration { get; set; }
    }

    /// <summary>
    /// Event to signal change of IAlterationEffectTemplateViewModel to the alteration UI holder
    /// </summary>
    public class AlterationEffectChangedEvent : RogueEvent<AlterationEffectChangedEventArgs>
    {
    }
}
