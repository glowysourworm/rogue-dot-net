using Rogue.NET.Core.Model.Enums;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Interface;
using System;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Common
{
    [Serializable]
    public class CreateMonsterAlterationEffectTemplateViewModel 
        : TemplateViewModel, IConsumableAlterationEffectTemplateViewModel,
                             IDoodadAlterationEffectTemplateViewModel,
                             IEnemyAlterationEffectTemplateViewModel,
                             ISkillAlterationEffectTemplateViewModel
    {
        AlterationRandomPlacementType _randomPlacementType;
        string _createMonsterEnemy;
        int _range;

        public AlterationRandomPlacementType RandomPlacementType
        {
            get { return _randomPlacementType; }
            set { this.RaiseAndSetIfChanged(ref _randomPlacementType, value); }
        }
        public string CreateMonsterEnemy
        {
            get { return _createMonsterEnemy; }
            set { this.RaiseAndSetIfChanged(ref _createMonsterEnemy, value); }
        }
        public int Range
        {
            get { return _range; }
            set { this.RaiseAndSetIfChanged(ref _range, value); }
        }

        public CreateMonsterAlterationEffectTemplateViewModel() { }
    }
}
