using Rogue.NET.Core.Model.Enums;
using Rogue.NET.ScenarioEditor.ViewModel.Attribute;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Interface;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Content;
using Rogue.NET.ScenarioEditor.Views.Assets.SharedControl.AlterationControl.EffectControl;
using System;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Common
{
    [Serializable]
    [UIType(DisplayName = "Create Friendly",
            Description = "Creates a Friendly Character",
            ViewType = typeof(CreateFriendlyEffectParameters),
            BaseType = UITypeAttributeBaseType.Alteration)]
    public class CreateFriendlyAlterationEffectTemplateViewModel : TemplateViewModel, IConsumableAlterationEffectTemplateViewModel,
                                                                                      IDoodadAlterationEffectTemplateViewModel,
                                                                                      IEnemyAlterationEffectTemplateViewModel,
                                                                                      ISkillAlterationEffectTemplateViewModel
    {
        AlterationRandomPlacementType _randomPlacementType;
        FriendlyTemplateViewModel _friendly;
        int _range;

        public AlterationRandomPlacementType RandomPlacementType
        {
            get { return _randomPlacementType; }
            set { this.RaiseAndSetIfChanged(ref _randomPlacementType, value); }
        }
        public FriendlyTemplateViewModel Friendly
        {
            get { return _friendly; }
            set { this.RaiseAndSetIfChanged(ref _friendly, value); }
        }
        public int Range
        {
            get { return _range; }
            set { this.RaiseAndSetIfChanged(ref _range, value); }
        }

        public CreateFriendlyAlterationEffectTemplateViewModel() { }
    }
}
