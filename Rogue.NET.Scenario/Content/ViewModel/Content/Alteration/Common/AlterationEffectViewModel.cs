using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Model.Scenario;
using Rogue.NET.Core.Model.Scenario.Alteration.Effect;
using Rogue.NET.Core.Model.Scenario.Alteration.Interface;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Common;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Interface;
using Rogue.NET.Scenario.Content.ViewModel.Attribute;
using Rogue.NET.Scenario.Content.ViewModel.Content.Alteration.Effect;
using System;

namespace Rogue.NET.Scenario.Content.ViewModel.Content.Alteration.Common
{
    public abstract class AlterationEffectViewModel : RogueBaseViewModel
    {
        string _displayType;

        public string DisplayType
        {
            get { return _displayType; }
            set { this.RaiseAndSetIfChanged(ref _displayType, value); }
        }

        public AlterationEffectViewModel(RogueBase rogueBase) : base(rogueBase)
        {
            this.DisplayType = this.GetAttribute<UIDisplayAttribute>().Name;
        }

        public AlterationEffectViewModel(Template template) : base(template)
        {
            this.DisplayType = this.GetAttribute<UIDisplayAttribute>().Name;
        }


        /// <summary>
        /// Static constructor for AlterationEffectViewModel to handle type casting.
        /// </summary>
        public static AlterationEffectViewModel Create(IAlterationEffect effect)
        {
            if (effect is AttackAttributeAuraAlterationEffect)
                return new AttackAttributeAuraAlterationEffectViewModel(effect as AttackAttributeAuraAlterationEffect);

            else if (effect is AttackAttributeMeleeAlterationEffect)
                return new AttackAttributeMeleeAlterationEffectViewModel(effect as AttackAttributeMeleeAlterationEffect);

            else if (effect is AttackAttributePassiveAlterationEffect)
                return new AttackAttributePassiveAlterationEffectViewModel(effect as AttackAttributePassiveAlterationEffect);

            else if (effect is AttackAttributeTemporaryAlterationEffect)
                return new AttackAttributeTemporaryAlterationEffectViewModel(effect as AttackAttributeTemporaryAlterationEffect);

            else if (effect is AuraAlterationEffect)
                return new AuraAlterationEffectViewModel(effect as AuraAlterationEffect);

            else if (effect is ChangeLevelAlterationEffect)
                return new ChangeLevelAlterationEffectViewModel(effect as ChangeLevelAlterationEffect);

            else if (effect is CreateEnemyAlterationEffect)
                return new CreateEnemyAlterationEffectViewModel(effect as CreateEnemyAlterationEffect);

            else if (effect is CreateFriendlyAlterationEffect)
                return new CreateFriendlyAlterationEffectViewModel(effect as CreateFriendlyAlterationEffect);

            else if (effect is CreateTemporaryCharacterAlterationEffect)
                return new CreateTemporaryCharacterAlterationEffectViewModel(effect as CreateTemporaryCharacterAlterationEffect);

            else if (effect is DrainMeleeAlterationEffect)
                return new DrainMeleeAlterationEffectViewModel(effect as DrainMeleeAlterationEffect);

            else if (effect is EquipmentDamageAlterationEffect)
                return new EquipmentDamageAlterationEffectViewModel(effect as EquipmentDamageAlterationEffect);

            else if (effect is EquipmentEnhanceAlterationEffect)
                return new EquipmentEnhanceAlterationEffectViewModel(effect as EquipmentEnhanceAlterationEffect);

            else if (effect is OtherAlterationEffect)
                return new OtherAlterationEffectViewModel(effect as OtherAlterationEffect);

            else if (effect is PassiveAlterationEffect)
                return new PassiveAlterationEffectViewModel(effect as PassiveAlterationEffect);

            else if (effect is PermanentAlterationEffect)
                return new PermanentAlterationEffectViewModel(effect as PermanentAlterationEffect);

            else if (effect is RemedyAlterationEffect)
                return new RemedyAlterationEffectViewModel(effect as RemedyAlterationEffect);

            else if (effect is RevealAlterationEffect)
                return new RevealAlterationEffectViewModel(effect as RevealAlterationEffect);

            else if (effect is RunAwayAlterationEffect)
                return new RunAwayAlterationEffectViewModel(effect as RunAwayAlterationEffect);

            else if (effect is StealAlterationEffect)
                return new StealAlterationEffectViewModel(effect as StealAlterationEffect);

            else if (effect is TeleportRandomAlterationEffect)
                return new TeleportRandomAlterationEffectViewModel(effect as TeleportRandomAlterationEffect);

            else if (effect is TemporaryAlterationEffect)
                return new TemporaryAlterationEffectViewModel(effect as TemporaryAlterationEffect);

            else if (effect is TransmuteAlterationEffect)
                return new TransmuteAlterationEffectViewModel(effect as TransmuteAlterationEffect);

            else
                throw new Exception("Unknown IAlterationEffect AlterationViewModel");
        }

        /// <summary>
        /// Static constructor for AlterationEffectViewModel to handle type casting.
        /// </summary>
        public static AlterationEffectViewModel Create(IAlterationEffectTemplate effect)
        {
            if (effect is AttackAttributeAuraAlterationEffectTemplate)
                return new AttackAttributeAuraAlterationEffectViewModel(effect as AttackAttributeAuraAlterationEffectTemplate);

            else if (effect is AttackAttributeMeleeAlterationEffectTemplate)
                return new AttackAttributeMeleeAlterationEffectViewModel(effect as AttackAttributeMeleeAlterationEffectTemplate);

            else if (effect is AttackAttributePassiveAlterationEffectTemplate)
                return new AttackAttributePassiveAlterationEffectViewModel(effect as AttackAttributePassiveAlterationEffectTemplate);

            else if (effect is AttackAttributeTemporaryAlterationEffectTemplate)
                return new AttackAttributeTemporaryAlterationEffectViewModel(effect as AttackAttributeTemporaryAlterationEffectTemplate);

            else if (effect is AuraAlterationEffectTemplate)
                return new AuraAlterationEffectViewModel(effect as AuraAlterationEffectTemplate);

            else if (effect is ChangeLevelAlterationEffectTemplate)
                return new ChangeLevelAlterationEffectViewModel(effect as ChangeLevelAlterationEffectTemplate);

            else if (effect is CreateEnemyAlterationEffectTemplate)
                return new CreateEnemyAlterationEffectViewModel(effect as CreateEnemyAlterationEffectTemplate);

            else if (effect is CreateFriendlyAlterationEffectTemplate)
                return new CreateFriendlyAlterationEffectViewModel(effect as CreateFriendlyAlterationEffectTemplate);

            else if (effect is CreateTemporaryCharacterAlterationEffectTemplate)
                return new CreateTemporaryCharacterAlterationEffectViewModel(effect as CreateTemporaryCharacterAlterationEffectTemplate);

            else if (effect is DrainMeleeAlterationEffectTemplate)
                return new DrainMeleeAlterationEffectViewModel(effect as DrainMeleeAlterationEffectTemplate);

            else if (effect is EquipmentDamageAlterationEffectTemplate)
                return new EquipmentDamageAlterationEffectViewModel(effect as EquipmentDamageAlterationEffectTemplate);

            else if (effect is EquipmentEnhanceAlterationEffectTemplate)
                return new EquipmentEnhanceAlterationEffectViewModel(effect as EquipmentEnhanceAlterationEffectTemplate);

            else if (effect is OtherAlterationEffectTemplate)
                return new OtherAlterationEffectViewModel(effect as OtherAlterationEffectTemplate);

            else if (effect is PassiveAlterationEffectTemplate)
                return new PassiveAlterationEffectViewModel(effect as PassiveAlterationEffectTemplate);

            else if (effect is PermanentAlterationEffectTemplate)
                return new PermanentAlterationEffectViewModel(effect as PermanentAlterationEffectTemplate);

            else if (effect is RemedyAlterationEffectTemplate)
                return new RemedyAlterationEffectViewModel(effect as RemedyAlterationEffectTemplate);

            else if (effect is RevealAlterationEffectTemplate)
                return new RevealAlterationEffectViewModel(effect as RevealAlterationEffectTemplate);

            else if (effect is RunAwayAlterationEffectTemplate)
                return new RunAwayAlterationEffectViewModel(effect as RunAwayAlterationEffectTemplate);

            else if (effect is StealAlterationEffectTemplate)
                return new StealAlterationEffectViewModel(effect as StealAlterationEffectTemplate);

            else if (effect is TeleportRandomAlterationEffectTemplate)
                return new TeleportRandomAlterationEffectViewModel(effect as TeleportRandomAlterationEffectTemplate);

            else if (effect is TemporaryAlterationEffectTemplate)
                return new TemporaryAlterationEffectViewModel(effect as TemporaryAlterationEffectTemplate);

            else if (effect is TransmuteAlterationEffectTemplate)
                return new TransmuteAlterationEffectViewModel(effect as TransmuteAlterationEffectTemplate);

            else
                throw new Exception("Unknown IAlterationEffect AlterationViewModel");
        }
    }
}
