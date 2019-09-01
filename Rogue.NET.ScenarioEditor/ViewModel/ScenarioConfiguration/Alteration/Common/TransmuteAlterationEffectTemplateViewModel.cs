using Rogue.NET.ScenarioEditor.ViewModel.Attribute;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Interface;
using Rogue.NET.ScenarioEditor.Views.Assets.SharedControl.AlterationControl.EffectControl;
using System;
using System.Collections.ObjectModel;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Common
{
    [Serializable]
    [UIType(DisplayName = "Transmute",
            Description = "Creates a randomly drawn item from input items",
            ViewType = typeof(TransmuteEffectParameters))]
    public class TransmuteAlterationEffectTemplateViewModel
        : TemplateViewModel, IConsumableAlterationEffectTemplateViewModel,
                             IDoodadAlterationEffectTemplateViewModel,
                             ISkillAlterationEffectTemplateViewModel
    {
        double _probabilityOfSuccess;

        public double ProbabilityOfSuccess
        {
            get { return _probabilityOfSuccess; }
            set { this.RaiseAndSetIfChanged(ref _probabilityOfSuccess, value); }
        }

        public ObservableCollection<TransmuteAlterationEffectItemTemplateViewModel> TransmuteItems { get; set; }

        public TransmuteAlterationEffectTemplateViewModel()
        {
            this.TransmuteItems = new ObservableCollection<TransmuteAlterationEffectItemTemplateViewModel>();
        }
    }
}
