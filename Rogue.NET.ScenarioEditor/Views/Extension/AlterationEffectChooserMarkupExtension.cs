using Microsoft.Practices.ServiceLocation;
using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Interface;
using Rogue.NET.ScenarioEditor.Views.Assets.SharedControl.AlterationControl.CommonControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace Rogue.NET.ScenarioEditor.Views.Extension
{
    /// <summary>
    /// Markup extension used to instantiate an AlterationEffectChooser with the 
    /// specified public property
    /// </summary>
    public class AlterationEffectChooserMarkupExtension : MarkupExtension
    {
        /// <summary>
        /// Alteration effect interface type
        /// </summary>
        public Type AlterationEffectInterface { get; set; }

        /// <summary>
        /// Alteration type (parent to the alteration effect)
        /// </summary>
        public Type AlterationType { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (!typeof(IAlterationEffectTemplateViewModel).IsAssignableFrom(this.AlterationEffectInterface))
                throw new Exception("AlterationEffectInterface must inherit from IAlterationEffectTemplateViewModel");

            var eventAggregator = ServiceLocator.Current.GetInstance<IRogueEventAggregator>();

            return new AlterationEffectChooser(eventAggregator, this.AlterationType, this.AlterationEffectInterface);
        }
    }
}
