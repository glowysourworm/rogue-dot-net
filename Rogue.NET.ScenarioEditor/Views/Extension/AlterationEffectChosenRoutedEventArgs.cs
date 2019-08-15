using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Interface;
using System.Windows;

namespace Rogue.NET.ScenarioEditor.Views.Extension
{
    public class AlterationEffectChosenRoutedEventArgs : RoutedEventArgs
    {
        public IAlterationEffectTemplateViewModel Effect { get; set; }

        public AlterationEffectChosenRoutedEventArgs(IAlterationEffectTemplateViewModel effect, RoutedEvent routedEvent, object source)
            : base(routedEvent, source)
        {
            this.Effect = effect;
        }
    }
}
