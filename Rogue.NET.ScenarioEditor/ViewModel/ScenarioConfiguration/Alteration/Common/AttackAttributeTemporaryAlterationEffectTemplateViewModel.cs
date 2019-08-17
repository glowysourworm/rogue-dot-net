﻿using Rogue.NET.Core.Model.Enums;
using Rogue.NET.ScenarioEditor.ViewModel.Attribute;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Interface;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Content;
using Rogue.NET.ScenarioEditor.Views.Assets.SharedControl.AlterationControl.EffectControl;
using System;
using System.Collections.ObjectModel;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Common
{
    [Serializable]
    [UIType(DisplayName = "Attack Attribute (Temporary)",
            Description = "Creates a timed (Friendly or Malign) contribution to the affected character(s) Attack Attributes",
            ViewType = typeof(AttackAttributeTemporaryEffectParameters))]
    public class AttackAttributeTemporaryAlterationEffectTemplateViewModel 
        : TemplateViewModel, IConsumableAlterationEffectTemplateViewModel, 
                             IConsumableProjectileAlterationEffectTemplateViewModel,
                             IDoodadAlterationEffectTemplateViewModel,
                             IEnemyAlterationEffectTemplateViewModel,
                             ISkillAlterationEffectTemplateViewModel
    {
        AlterationAttackAttributeCombatType _combatType;
        AlteredCharacterStateTemplateViewModel _alteredState;
        SymbolDeltaTemplateViewModel _symbolAlteration;
        bool _isStackable;
        RangeViewModel<int> _eventTime;

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
        public SymbolDeltaTemplateViewModel SymbolAlteration
        {
            get { return _symbolAlteration; }
            set { this.RaiseAndSetIfChanged(ref _symbolAlteration, value); }
        }
        public bool IsStackable
        {
            get { return _isStackable; }
            set { this.RaiseAndSetIfChanged(ref _isStackable, value); }
        }

        public RangeViewModel<int> EventTime
        {
            get { return _eventTime; }
            set { this.RaiseAndSetIfChanged(ref _eventTime, value); }
        }

        public ObservableCollection<AttackAttributeTemplateViewModel> AttackAttributes { get; set; }

        public AttackAttributeTemporaryAlterationEffectTemplateViewModel()
        {
            this.AttackAttributes = new ObservableCollection<AttackAttributeTemplateViewModel>();
            this.AlteredState = new AlteredCharacterStateTemplateViewModel();
            this.EventTime = new RangeViewModel<int>(1, 20, 30, 1000);
            this.SymbolAlteration = new SymbolDeltaTemplateViewModel();
        }
    }
}
 