using Rogue.NET.Common.ViewModel;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content.Skill;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Extension;
using Rogue.NET.Scenario.Content.ViewModel.Content.ScenarioMetaData;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Rogue.NET.Scenario.Content.ViewModel.Content.Alteration
{
    public class SpellViewModel : RogueBaseViewModel
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
        AlterationType _type;
        AlterationBlockType _blockType;
        AlterationMagicEffectType _otherEffectType;
        AlterationAttackAttributeType _attackAttributeType;
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

        public AlterationType Type
        {
            get { return _type; }
            set { this.RaiseAndSetIfChanged(ref _type, value); }
        }
        public AlterationBlockType BlockType
        {
            get { return _blockType; }
            set { this.RaiseAndSetIfChanged(ref _blockType, value); }
        }
        public AlterationMagicEffectType OtherEffectType
        {
            get { return _otherEffectType; }
            set { this.RaiseAndSetIfChanged(ref _otherEffectType, value); }
        }
        public AlterationAttackAttributeType AttackAttributeType
        {
            get { return _attackAttributeType; }
            set { this.RaiseAndSetIfChanged(ref _attackAttributeType, value); }
        }

        public ObservableCollection<AlterationAttributeViewModel> AlterationCostAttributes { get; set; }
        public ObservableCollection<AlterationAttributeViewModel> AlterationEffectAttributes { get; set; }
        public ObservableCollection<AlterationAttributeViewModel> AlterationAuraEffectAttributes { get; set; }
        public ObservableCollection<AttackAttributeViewModel> AlterationEffectAttackAttributes { get; set; }

        #endregion

        public SpellViewModel(Spell alteration) : base(alteration)
        {
            this.Type = alteration.Type;
            this.BlockType = alteration.BlockType;
            this.OtherEffectType = alteration.OtherEffectType;
            this.AttackAttributeType = alteration.AttackAttributeType;

            this.DisplayType = CreateDisplayType(alteration.Type, alteration.OtherEffectType, alteration.AttackAttributeType);

            // For Auras
            this.EffectRange = alteration.EffectRange.ToString("N0");
            this.AuraAlteredState = alteration.AuraEffect.AlteredState == null ? null :
                                    new ScenarioImageViewModel(alteration.AuraEffect.Guid, 
                                                               alteration.AuraEffect.Name,
                                                               alteration.AuraEffect.AlteredState.SymbolDetails);

            // Effect
            this.CanSeeInvisibleCharacters = alteration.Effect.CanSeeInvisibleCharacters;
            this.IsAlteredState = alteration.Effect.AlteredState != null && alteration.Effect.AlteredState.BaseType != CharacterStateType.Normal;
            this.EventTime = alteration.Effect.EventTime.Low == alteration.Effect.EventTime.High ? 
                                alteration.Effect.EventTime.Low.ToString("N0") :
                                alteration.Effect.EventTime.Low.ToString("N0") + " to " +
                                alteration.Effect.EventTime.High.ToString("N0");
            this.AlteredState = this.IsAlteredState ? new ScenarioImageViewModel(
                                                        alteration.Effect.AlteredState.Guid,
                                                        alteration.Effect.AlteredState.Name,
                                                        alteration.Effect.AlteredState.SymbolDetails) : null;
            this.RemediedState = this.Type == AlterationType.Remedy ? new ScenarioImageViewModel(
                                                        alteration.Effect.RemediedState.Guid,
                                                        alteration.Effect.RemediedState.Name,
                                                        alteration.Effect.RemediedState.SymbolDetails) : null;

            // Parameters
            this.AlterationCostType = alteration.Cost.Type;
            this.CreateMonsterEnemyName = alteration.CreateMonsterEnemyName;
            this.DisplayName = alteration.DisplayName;

            this.AlterationAuraEffectAttributes = new ObservableCollection<AlterationAttributeViewModel>(
                alteration.AuraEffect.GetUIAttributes().Select(x => new AlterationAttributeViewModel()
                {
                    AttributeName = x.Key,
                    AttributeValue = x.Value
                }));

            this.AlterationEffectAttributes = new ObservableCollection<AlterationAttributeViewModel>(
                alteration.Effect.GetUIAttributes().Select(x => new AlterationAttributeViewModel()
                {
                    AttributeName = x.Key,
                    AttributeValue = x.Value
                }));

            this.AlterationCostAttributes = new ObservableCollection<AlterationAttributeViewModel>(
                alteration.Cost.GetUIAttributes().Select(x => new AlterationAttributeViewModel()
                {
                    AttributeName = x.Key,
                    AttributeValue = x.Value.ToString("F2")
                }));

            this.AlterationEffectAttackAttributes = new ObservableCollection<AttackAttributeViewModel>(
                alteration.Effect
                          .AttackAttributes
                          .Where(x => x.Attack.IsSet() || x.Resistance.IsSet() || x.Weakness.IsSet())
                          .Select(x => new AttackAttributeViewModel(x)));
        }

        private string CreateDisplayType(
                        AlterationType type, 
                        AlterationMagicEffectType otherEffectType, 
                        AlterationAttackAttributeType attackAttributeType)
        {
            switch (type)
            {
                case AlterationType.PassiveSource:
                    return "Passive";
                case AlterationType.PassiveAura:
                    return "Aura";
                case AlterationType.TemporarySource:                    
                case AlterationType.TemporaryTarget:
                case AlterationType.TemporaryAllTargets:
                case AlterationType.TemporaryAllInRange:
                case AlterationType.TemporaryAllInRangeExceptSource:
                    return "Temporary";
                case AlterationType.PermanentSource:
                case AlterationType.PermanentTarget:
                case AlterationType.PermanentAllTargets:
                case AlterationType.PermanentAllInRange:
                case AlterationType.PermanentAllInRangeExceptSource:
                    return "Permanent";
                case AlterationType.Steal:
                    return "Steal";
                case AlterationType.RunAway:
                    return "Escape";
                case AlterationType.TeleportSelf:
                case AlterationType.TeleportTarget:
                case AlterationType.TeleportAllTargets:
                case AlterationType.TeleportAllInRange:
                case AlterationType.TeleportAllInRangeExceptSource:
                    return "Teleport";
                case AlterationType.OtherMagicEffect:
                    switch (otherEffectType)
                    {
                        case AlterationMagicEffectType.None:
                            return "None";
                        case AlterationMagicEffectType.ChangeLevelRandomUp:
                            return "Random Scenario Level Up";
                        case AlterationMagicEffectType.ChangeLevelRandomDown:
                            return "Random Scenario Level Down";
                        case AlterationMagicEffectType.Identify:
                            return "Identify";
                        case AlterationMagicEffectType.Uncurse:
                            return "Uncurse";
                        case AlterationMagicEffectType.EnchantArmor:
                            return "Enchant Armor";
                        case AlterationMagicEffectType.EnchantWeapon:
                            return "Enchant Weapon";
                        case AlterationMagicEffectType.RevealItems:
                            return "Reveal Items";
                        case AlterationMagicEffectType.RevealMonsters:
                            return "Reveal Enemies";
                        case AlterationMagicEffectType.RevealSavePoint:
                            return "Reveal Save Point";
                        case AlterationMagicEffectType.RevealFood:
                            return "Reveal Food";
                        case AlterationMagicEffectType.RevealLevel:
                            return "Reveal Level";
                        case AlterationMagicEffectType.CreateMonster:
                            return "Create Monster";
                        case AlterationMagicEffectType.RenounceReligion:
                            return "Renounce Religion";
                        case AlterationMagicEffectType.AffiliateReligion:
                            return "Affiliate Religion";
                        default:
                            throw new Exception("Unknown Other Alteration Effect Type");
                    }
                case AlterationType.AttackAttribute:
                    switch (attackAttributeType)
                    {
                        case AlterationAttackAttributeType.ImbueArmor:
                            return "Imbue Armor";
                        case AlterationAttackAttributeType.ImbueWeapon:
                            return "Imbue Weapon";
                        case AlterationAttackAttributeType.Passive:
                            return "Passive";
                        case AlterationAttackAttributeType.TemporaryFriendlySource:
                        case AlterationAttackAttributeType.TemporaryFriendlyTarget:
                            return "Temporary (Friendly)";
                        case AlterationAttackAttributeType.TemporaryMalignSource:
                        case AlterationAttackAttributeType.TemporaryMalignTarget:
                        case AlterationAttackAttributeType.TemporaryMalignAllInRange:
                        case AlterationAttackAttributeType.TemporaryMalignAllInRangeExceptSource:
                            return "Temporary (Malign)";
                        case AlterationAttackAttributeType.MeleeTarget:
                        case AlterationAttackAttributeType.MeleeAllInRange:
                        case AlterationAttackAttributeType.MeleeAllInRangeExceptSource:
                            return "Attack";
                        default:
                            throw new Exception("Unknown Alteration Attack Attribute Type");
                    }
                case AlterationType.Remedy:
                    return "Remedy";
                default:
                    throw new Exception("Unknwon Alteration Type");
            }
        }
    }
}
