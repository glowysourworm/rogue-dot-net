using Microsoft.Practices.ServiceLocation;
using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.ScenarioEditor.Service.Interface;
using Rogue.NET.ScenarioEditor.Views.Assets.SharedControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.ScenarioEditor.Views.MarkupExtension
{
    [System.Windows.Markup.MarkupExtensionReturnType(typeof(AttackAttributeListControl))]
    public class AttackAttributeListControlConstructor : System.Windows.Markup.MarkupExtension
    {
        public int AttackAttributeCountLimit { get; set; }
        public bool ShowAttack { get; set; }
        public bool ShowResistance { get; set; }
        public bool ShowWeakness { get; set; }
        public bool ShowImmune { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var eventAggregator = ServiceLocator.Current.GetInstance<IRogueEventAggregator>();
            var scenarioCollectionProvider = ServiceLocator.Current.GetInstance<IScenarioCollectionProvider>();

            var control = new AttackAttributeListControl(eventAggregator, scenarioCollectionProvider);

            control.AttackAttributeCountLimit = this.AttackAttributeCountLimit;
            control.ShowAttack = this.ShowAttack;
            control.ShowImmune = this.ShowImmune;
            control.ShowResistance = this.ShowResistance;
            control.ShowWeakness = this.ShowWeakness;

            return control;
        }
    }
}
