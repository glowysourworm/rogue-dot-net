using Microsoft.Practices.ServiceLocation;
using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.ScenarioEditor.Views.Assets.SharedControl;
using System;

namespace Rogue.NET.ScenarioEditor.Views.MarkupExtension
{
    [System.Windows.Markup.MarkupExtensionReturnType(typeof(MetaDataControl))]
    public class MetaDataControlConstructor : System.Windows.Markup.MarkupExtension
    {
        public bool HasCursedSetting { get; set; }
        public bool HasUniqueSetting { get; set; }
        public bool HasObjectiveSetting { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var eventAggregator = ServiceLocator.Current.GetInstance<IRogueEventAggregator>();

            if (eventAggregator == null)
                throw new Exception("MetaDataControlConstructor initialization failed - event aggregator not instantiated");

            return new MetaDataControl(eventAggregator)
            {
                HasCursedSetting = this.HasCursedSetting,
                HasObjectiveSetting = this.HasObjectiveSetting,
                HasUniqueSetting = this.HasUniqueSetting
            };
        }
    }
}
