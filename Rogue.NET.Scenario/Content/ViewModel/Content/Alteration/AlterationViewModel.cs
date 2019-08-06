using Rogue.NET.Common.ViewModel;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration;
using Rogue.NET.Core.Model.Scenario.Alteration.Extension;
using Rogue.NET.Core.Model.Scenario.Dynamic;
using System;
using System.Linq;
using System.Collections.ObjectModel;
using Rogue.NET.Scenario.Content.ViewModel.Content.ScenarioMetaData;
using Rogue.NET.Core.Model.Scenario.Alteration.Common;

namespace Rogue.NET.Scenario.Content.ViewModel.Content.Alteration
{
    public class AlterationViewModel : NotifyViewModel
    {
        string _displayName;
        string _type;                    // Passive / Temporary / Temporary (Friendly) / Temporary (Malign)

        bool _isAlteredState;
        ScenarioImageViewModel _alteredCharacterState;

        public string DisplayName
        {
            get { return _displayName; }
            set { this.RaiseAndSetIfChanged(ref _displayName, value); }
        }
        public string Type
        {
            get { return _type; }
            set { this.RaiseAndSetIfChanged(ref _type, value); }
        }
        public bool IsAlteredState
        {
            get { return _isAlteredState; }
            set { this.RaiseAndSetIfChanged(ref _isAlteredState, value); }
        }
        public ScenarioImageViewModel AlteredCharacterState
        {
            get { return _alteredCharacterState; }
            set { this.RaiseAndSetIfChanged(ref _alteredCharacterState, value); }
        }

        /// <summary>
        /// These are calculated with pre-set display values
        /// </summary>
        public ObservableCollection<AlterationAttributeViewModel> AlterationCostAttributes { get; set; }

        /// <summary>
        /// These are calculated with pre-set display values
        /// </summary>
        public ObservableCollection<AlterationAttributeViewModel> AlterationEffectAttributes { get; set; }

        /// <summary>
        /// Attack Attributes for the alteration effect
        /// </summary>
        public ObservableCollection<AttackAttributeViewModel> AlterationEffectAttackAttributes { get; set; }

        public AlterationViewModel()
        {
            this.AlterationCostAttributes = new ObservableCollection<AlterationAttributeViewModel>();
            this.AlterationEffectAttributes = new ObservableCollection<AlterationAttributeViewModel>();
            this.AlterationEffectAttackAttributes = new ObservableCollection<AttackAttributeViewModel>();

            this.IsAlteredState = false;
        }

        /// <summary>
        /// NOT LIKED - But this constructor can use the Character Alteration.Get tuple to create
        /// an Aleration ViewModel. This should be refactored to create some other data class
        /// </summary>
        public AlterationViewModel(
                AlterationType type, 
                AlterationAttackAttributeType attackAttributeType, 
                AlterationCost alterationCost, 
                AlterationEffect alterationEffect)
        {
            this.DisplayName = alterationEffect.DisplayName;
            this.Type = type == AlterationType.PassiveAura ? "Aura" :
                        type == AlterationType.PassiveSource ? "Passive" :
                        type == AlterationType.TemporarySource ? "Temporary" :
                        type == AlterationType.AttackAttribute ?
                            attackAttributeType == AlterationAttackAttributeType.Passive ? "Passive" :
                            attackAttributeType == AlterationAttackAttributeType.TemporaryFriendlySource ? "Temporary (Friendly)" :
                            attackAttributeType == AlterationAttackAttributeType.TemporaryMalignSource ? "Temporary (Malign)" : "" : "";
            this.AlteredCharacterState = alterationEffect.State == null ? null : new ScenarioImageViewModel(alterationEffect.State);
            this.AlterationCostAttributes = alterationCost != null ?
                new ObservableCollection<AlterationAttributeViewModel>(
                    alterationCost.GetUIAttributes().Select(x => new AlterationAttributeViewModel()
                    {
                        AttributeName = x.Key,
                        AttributeValue = x.Value.ToString("F2")
                    })) :
                new ObservableCollection<AlterationAttributeViewModel>();

            this.AlterationEffectAttributes =
                new ObservableCollection<AlterationAttributeViewModel>(
                    alterationEffect.GetUIAttributes().Select(x => new AlterationAttributeViewModel()
                    {
                        AttributeName = x.Key,
                        AttributeValue = x.Value.ToString("F2")
                    }));

            this.AlterationEffectAttackAttributes =
                new ObservableCollection<AttackAttributeViewModel>(
                    alterationEffect.AttackAttributes
                                    .Where(x => x.Attack > 0 || x.Resistance > 0 || x.Weakness > 0)
                                    .Select(x => new AttackAttributeViewModel(x)));

            this.IsAlteredState = alterationEffect.State != null &&
                                  alterationEffect.State.BaseType != CharacterStateType.Normal;
        }
    }
}
