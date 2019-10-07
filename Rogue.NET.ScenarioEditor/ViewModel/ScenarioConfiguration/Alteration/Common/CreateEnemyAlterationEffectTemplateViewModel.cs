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
    [UIType(DisplayName = "Create Enemy",
            Description = "Creates an Enemy Character",
            ViewType = typeof(CreateEnemyEffectParameters),
            BaseType = UITypeAttributeBaseType.Alteration)]
    public class CreateEnemyAlterationEffectTemplateViewModel : TemplateViewModel, IConsumableAlterationEffectTemplateViewModel,
                                                                                   IDoodadAlterationEffectTemplateViewModel,
                                                                                   IEnemyAlterationEffectTemplateViewModel,
                                                                                   ISkillAlterationEffectTemplateViewModel
    {
        AlterationRandomPlacementType _randomPlacementType;
        EnemyTemplateViewModel _enemy;
        int _range;

        public AlterationRandomPlacementType RandomPlacementType
        {
            get { return _randomPlacementType; }
            set { this.RaiseAndSetIfChanged(ref _randomPlacementType, value); }
        }
        public EnemyTemplateViewModel Enemy
        {
            get { return _enemy; }
            set { this.RaiseAndSetIfChanged(ref _enemy, value); }
        }
        public int Range
        {
            get { return _range; }
            set { this.RaiseAndSetIfChanged(ref _range, value); }
        }

        public CreateEnemyAlterationEffectTemplateViewModel() { }
    }
}
