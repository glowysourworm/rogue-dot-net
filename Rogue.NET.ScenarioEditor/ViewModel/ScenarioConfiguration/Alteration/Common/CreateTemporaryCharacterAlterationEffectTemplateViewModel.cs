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
    [UIType(DisplayName = "Create Temporary Character",
            Description = "Creates an Temporary Character",
            ViewType = typeof(CreateTemporaryCharacterEffectParameters),
            BaseType = UITypeAttributeBaseType.Alteration)]
    public class CreateTemporaryCharacterAlterationEffectTemplateViewModel : TemplateViewModel, IConsumableAlterationEffectTemplateViewModel,
                                                                                                IDoodadAlterationEffectTemplateViewModel,
                                                                                                IEnemyAlterationEffectTemplateViewModel,
                                                                                                ISkillAlterationEffectTemplateViewModel
    {
        AlterationRandomPlacementType _randomPlacementType;
        TemporaryCharacterTemplateViewModel _temporaryCharacter;
        int _range;

        public AlterationRandomPlacementType RandomPlacementType
        {
            get { return _randomPlacementType; }
            set { this.RaiseAndSetIfChanged(ref _randomPlacementType, value); }
        }
        public TemporaryCharacterTemplateViewModel TemporaryCharacter
        {
            get { return _temporaryCharacter; }
            set { this.RaiseAndSetIfChanged(ref _temporaryCharacter, value); }
        }
        public int Range
        {
            get { return _range; }
            set { this.RaiseAndSetIfChanged(ref _range, value); }
        }

        public CreateTemporaryCharacterAlterationEffectTemplateViewModel()
        {
            this.TemporaryCharacter = new TemporaryCharacterTemplateViewModel();
        }
    }
}
