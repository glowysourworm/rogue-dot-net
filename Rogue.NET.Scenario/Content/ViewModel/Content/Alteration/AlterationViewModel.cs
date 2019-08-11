using Rogue.NET.Common.ViewModel;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.Core.Model.Scenario.Alteration.Effect;
using Rogue.NET.Core.Model.Scenario.Alteration.Extension;
using Rogue.NET.Core.Model.Scenario.Content.Skill;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Extension;
using Rogue.NET.Scenario.Content.ViewModel.Content.ScenarioMetaData;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Rogue.NET.Scenario.Content.ViewModel.Content.Alteration
{
    public class AlterationViewModel : RogueBaseViewModel
    {
        #region Backing Fields
        string _displayName;
        string _createMonsterEnemyName;
        bool _isStackable;
        bool _canSeeInvisibleCharacters;
        bool _isAlteredState;        

        ScenarioImageViewModel _alteredState;
        ScenarioImageViewModel _remediedState;
        ScenarioImageViewModel _auraAlteredState;

        string _eventTime;
        string _effectRange;
        string _displayType;

        AlterationCostType _alterationCostType;
        string _type;
        string _attackAttributeType;
        string _blockType;
        string _otherEffectType;
        #endregion

        #region (public) Properties
        public string DisplayName
        {
            get { return _displayName; }
            set { this.RaiseAndSetIfChanged(ref _displayName, value); }
        }
        public string CreateMonsterEnemyName
        {
            get { return _createMonsterEnemyName; }
            set { this.RaiseAndSetIfChanged(ref _createMonsterEnemyName, value); }
        }
        public bool IsStackable
        {
            get { return _isStackable; }
            set { this.RaiseAndSetIfChanged(ref _isStackable, value); }
        }

        // Alteration Cost (Other) Attributes
        public AlterationCostType AlterationCostType
        {
            get { return _alterationCostType; }
            set { this.RaiseAndSetIfChanged(ref _alterationCostType, value); }
        }

        // Alteration Effect (Other) Attributes
        public bool CanSeeInvisibleCharacters
        {
            get { return _canSeeInvisibleCharacters; }
            set { this.RaiseAndSetIfChanged(ref _canSeeInvisibleCharacters, value); }
        }
        public bool IsAlteredState
        {
            get { return _isAlteredState; }
            set { this.RaiseAndSetIfChanged(ref _isAlteredState, value); }
        }
        public string EventTime
        {
            get { return _eventTime; }
            set { this.RaiseAndSetIfChanged(ref _eventTime, value); }
        }
        public ScenarioImageViewModel AlteredState
        {
            get { return _alteredState; }
            set { this.RaiseAndSetIfChanged(ref _alteredState, value); }
        }
        public ScenarioImageViewModel RemediedState
        {
            get { return _remediedState; }
            set { this.RaiseAndSetIfChanged(ref _remediedState, value); }
        }

        // Alteration AuraEffect (Other) Attributes
        public ScenarioImageViewModel AuraAlteredState
        {
            get { return _auraAlteredState; }
            set { this.RaiseAndSetIfChanged(ref _auraAlteredState, value); }
        }
        public string EffectRange
        {
            get { return _effectRange; }
            set { this.RaiseAndSetIfChanged(ref _effectRange, value); }
        }

        // Alteration Designation
        public string DisplayType
        {
            get { return _displayType; }
            set { this.RaiseAndSetIfChanged(ref _displayType, value); }
        }

        // TODO:ALTERATION
        public string Type
        {
            get { return _type; }
            set { this.RaiseAndSetIfChanged(ref _type, value); }
        }
        // TODO:ALTERATION
        public string AttackAttributeType
        {
            get { return _attackAttributeType; }
            set { this.RaiseAndSetIfChanged(ref _attackAttributeType, value); }
        }
        public string BlockType
        {
            get { return _blockType; }
            set { this.RaiseAndSetIfChanged(ref _blockType, value); }
        }
        public string OtherEffectType
        {
            get { return _otherEffectType; }
            set { this.RaiseAndSetIfChanged(ref _otherEffectType, value); }
        }

        public ObservableCollection<AlterationAttributeViewModel> AlterationCostAttributes { get; set; }
        public ObservableCollection<AlterationAttributeViewModel> AlterationEffectAttributes { get; set; }
        public ObservableCollection<AlterationAttributeViewModel> AlterationAuraEffectAttributes { get; set; }
        public ObservableCollection<AttackAttributeViewModel> AlterationEffectAttackAttributes { get; set; }

        #endregion

        public AlterationViewModel(AlterationContainer alteration) : base(alteration)
        {
            this.Type = alteration.Effect.GetUITypeDescription();
            this.BlockType = alteration.SupportsBlocking() ? alteration.BlockType.ToString() : "N/A";
            this.OtherEffectType = alteration.Effect.GetUIOtherEffectType();
            this.AttackAttributeType = alteration.Effect.GetUIAttackAttributeType();

            // TODO:ALTERATION
            this.DisplayType = this.Type;

            // TODO:ALTERATION
            // For Auras
            //this.EffectRange = alteration.EffectRange.ToString("N0");
            //this.AuraAlteredState = alteration.AuraEffect.AlteredState == null ? null :
            //                        new ScenarioImageViewModel(alteration.AuraEffect.Guid, 
            //                                                   alteration.AuraEffect.Name,
            //                                                   alteration.AuraEffect.AlteredState.SymbolDetails);

            // TODO:ALTERATION
            // Effect
            //this.CanSeeInvisibleCharacters = alteration.Effect.CanSeeInvisibleCharacters;
            //this.IsAlteredState = alteration.Effect.AlteredState != null && alteration.Effect.AlteredState.BaseType != CharacterStateType.Normal;
            //this.EventTime = alteration.Effect.EventTime.Low == alteration.Effect.EventTime.High ? 
            //                    alteration.Effect.EventTime.Low.ToString("N0") :
            //                    alteration.Effect.EventTime.Low.ToString("N0") + " to " +
            //                    alteration.Effect.EventTime.High.ToString("N0");
            //this.AlteredState = this.IsAlteredState ? new ScenarioImageViewModel(
            //                                            alteration.Effect.AlteredState.Guid,
            //                                            alteration.Effect.AlteredState.Name,
            //                                            alteration.Effect.AlteredState.SymbolDetails) : null;
            //this.RemediedState = this.Type == AlterationType.Remedy ? new ScenarioImageViewModel(
            //                                            alteration.Effect.RemediedState.Guid,
            //                                            alteration.Effect.RemediedState.Name,
            //                                            alteration.Effect.RemediedState.SymbolDetails) : null;


            // Parameters
            //this.AlterationCostType = alteration.Cost.Type;
            //this.CreateMonsterEnemyName = alteration.CreateMonsterEnemyName;
            //this.DisplayName = alteration.DisplayName;

            //this.AlterationAuraEffectAttributes = new ObservableCollection<AlterationAttributeViewModel>(
            //    alteration.AuraEffect.GetUIAttributes().Select(x => new AlterationAttributeViewModel()
            //    {
            //        AttributeName = x.Key,
            //        AttributeValue = x.Value
            //    }));

            //this.AlterationEffectAttributes = new ObservableCollection<AlterationAttributeViewModel>(
            //    alteration.Effect.GetUIAttributes().Select(x => new AlterationAttributeViewModel()
            //    {
            //        AttributeName = x.Key,
            //        AttributeValue = x.Value
            //    }));

            //this.AlterationCostAttributes = new ObservableCollection<AlterationAttributeViewModel>(
            //    alteration.Cost.GetUIAttributes().Select(x => new AlterationAttributeViewModel()
            //    {
            //        AttributeName = x.Key,
            //        AttributeValue = x.Value.ToString("F2")
            //    }));

            //this.AlterationEffectAttackAttributes = new ObservableCollection<AttackAttributeViewModel>(
            //    alteration.Effect
            //              .AttackAttributes
            //              .Where(x => x.Attack.IsSet() || x.Resistance.IsSet() || x.Weakness.IsSet())
            //              .Select(x => new AttackAttributeViewModel(x)));
        }

    }
}
