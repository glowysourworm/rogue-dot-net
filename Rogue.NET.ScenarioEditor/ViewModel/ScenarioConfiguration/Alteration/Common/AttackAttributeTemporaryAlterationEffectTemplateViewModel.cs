using Rogue.NET.Core.Model.Enums;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Interface;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Content;
using System;
using System.Collections.Generic;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Common
{
    [Serializable]
    public class AttackAttributeTemporaryAlterationEffectTemplateViewModel 
        : TemplateViewModel, IConsumableAlterationEffectTemplateViewModel, 
                             IConsumableProjectileAlterationEffectTemplateViewModel,
                             IDoodadAlterationEffectTemplateViewModel,
                             IEnemyAlterationEffectTemplateViewModel,
                             ISkillAlterationEffectTemplateViewModel
    {
        AlterationTargetType _targetType;
        AlterationAttackAttributeCombatType _combatType;
        AlteredCharacterStateTemplateViewModel _alteredState;
        bool _isStackable;
        int _eventTime;

        public AlterationTargetType TargetType
        {
            get { return _targetType; }
            set { this.RaiseAndSetIfChanged(ref _targetType, value); }
        }
        public AlterationAttackAttributeCombatType CombatType
        {
            get { return _combatType; }
            set { this.RaiseAndSetIfChanged(ref _combatType, value); }
        }
        public AlteredCharacterStateTemplateViewModel AlteredState

        {
            get { return _alteredState; }
            set { this.RaiseAndSetIfChanged(ref _alteredState, value); }
        }
        public bool IsStackable
        {
            get { return _isStackable; }
            set { this.RaiseAndSetIfChanged(ref _isStackable, value); }
        }
        public int EventTime
        {
            get { return _eventTime; }
            set { this.RaiseAndSetIfChanged(ref _eventTime, value); }
        }

        public List<AttackAttributeTemplateViewModel> AttackAttributes { get; set; }

        public AttackAttributeTemporaryAlterationEffectTemplateViewModel()
        {
            this.AttackAttributes = new List<AttackAttributeTemplateViewModel>();
            this.AlteredState = new AlteredCharacterStateTemplateViewModel();
        }
    }
}
