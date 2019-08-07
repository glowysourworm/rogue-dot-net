using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration.Consumable;
using Rogue.NET.Core.Model.Scenario.Alteration.Doodad;
using Rogue.NET.Core.Model.Scenario.Alteration.Effect;
using Rogue.NET.Core.Model.Scenario.Alteration.Enemy;
using Rogue.NET.Core.Model.Scenario.Alteration.Equipment;
using Rogue.NET.Core.Model.Scenario.Alteration.Interface;
using Rogue.NET.Core.Model.Scenario.Alteration.Skill;
using Rogue.NET.Core.Model.Scenario.Animation;
using System;

namespace Rogue.NET.Core.Model.Scenario.Alteration.Common
{
    /// <summary>
    /// Common base class for alterations
    /// </summary>
    [Serializable]
    public abstract class AlterationBase : RogueBase
    {
        private IAlterationEffect _effect;

        /// <summary>
        /// Primary Alteration Effect for the AlterationBase container. This should be overridden
        /// by the child class; but referenced by the appropriate interface handle.
        /// </summary>
        public IAlterationEffect Effect
        {
            get { return _effect; }
            set
            {
                _effect = value;

                if (!ValidateEffectInterfaceType())
                    throw new Exception("Invalid IAlterationEffect Type!");
            }
        }

        public AlterationCost Cost { get; set; }
        public AnimationContainer AnimationGroup { get; set; }
        public AlterationBlockType BlockType { get; set; }

        /// <summary>
        /// Calculated cost type based on alteration type
        /// </summary>
        public AlterationCostType CostType
        {
            get
            {
                // Use once -> pay once
                if (this is ConsumableAlteration)
                    return AlterationCostType.OneTime;

                // I'm just throwing it!
                else if (this is ConsumableProjectileAlteration)
                    return AlterationCostType.None;

                // Thought about it.. simplified by saying No.
                else if (this is DoodadAlteration)
                    return AlterationCostType.None;

                // This one is more involved.. probably one time. Not supporting
                // Aura / Passive types for Enemy skills; but they can be wearing
                // equipment that DOES have those things. So, probably just one-time
                // costs.
                else if (this is EnemyAlteration)
                    // Went with one-time based on what effects are supported
                    return AlterationCostType.OneTime;

                // Pew! Pew! Pew!
                else if (this is EquipmentAttackAlteration)
                    return AlterationCostType.OneTime;

                // Passive / Aura
                else if (this is EquipmentEquipAlteration)
                    return AlterationCostType.PerStep;

                // You can't make me pay for a curse.. That's just stupid...
                else if (this is EquipmentCurseAlteration)
                    return AlterationCostType.None;

                // Anything and Everything a chap can unload...
                else if (this is SkillAlteration)
                {
                    if (this.Effect is AttackAttributeAuraAlterationEffect)
                        return AlterationCostType.PerStep;

                    else if (this.Effect is AttackAttributeMeleeAlterationEffect)
                        return AlterationCostType.OneTime;

                    else if (this.Effect is AttackAttributePassiveAlterationEffect)
                        return AlterationCostType.PerStep;

                    else if (this.Effect is AttackAttributeTemporaryAlterationEffect)
                        return AlterationCostType.OneTime;

                    else if (this.Effect is AuraAlterationEffect)
                        return AlterationCostType.PerStep;

                    else if (this.Effect is ChangeLevelAlterationEffect)
                        return AlterationCostType.OneTime;

                    else if (this.Effect is CreateMonsterAlterationEffect)
                        return AlterationCostType.OneTime;

                    else if (this.Effect is EquipmentModifyAlterationEffect)
                        return AlterationCostType.OneTime;

                    else if (this.Effect is OtherAlterationEffect)
                        return AlterationCostType.OneTime;

                    else if (this.Effect is PassiveAlterationEffect)
                        return AlterationCostType.PerStep;

                    else if (this.Effect is PermanentAlterationEffect)
                        return AlterationCostType.OneTime;

                    else if (this.Effect is RemedyAlterationEffect)
                        return AlterationCostType.OneTime;

                    else if (this.Effect is RevealAlterationEffect)
                        return AlterationCostType.OneTime;

                    else if (this.Effect is RunAwayAlterationEffect)
                        return AlterationCostType.None;

                    else if (this.Effect is StealAlterationEffect)
                        return AlterationCostType.OneTime;

                    else if (this.Effect is TeleportAlterationEffect)
                        return AlterationCostType.OneTime;

                    else if (this.Effect is TemporaryAlterationEffect)
                        return AlterationCostType.OneTime;

                    else
                        throw new Exception("Unhandled Alteration Effect type");
                }

                else
                    throw new Exception("Unhandled Alteration type");
            }
        }

        /// <summary>
        /// Calculated support for animations based on alteration type
        /// </summary>
        public bool SupportsAnimations
        {
            get
            {
                // If it's supported, then what about un-equipping? Or overlapping
                // animations with cursed equipment? Or un-equipping-but-cursed?
                // probably too much to support with overaps. Plus, the equipment
                // screen makes this an issue.
                if ((this is EquipmentCurseAlteration) ||
                    (this is EquipmentEquipAlteration))
                    return false;

                return true;
            }
        }

        /// <summary>
        /// Calculated support for blocking based on alteration / alteration effect type
        /// </summary>
        public bool SupportsBlocking
        {
            get
            {
                if (this is ConsumableAlteration)
                    return false;

                else if (this is ConsumableProjectileAlteration)
                    return false;

                else if (this is DoodadAlteration)
                    return false;

                // Temporary, Melee, or Equipment Modify (can block)
                else if (this is EnemyAlteration)
                    return true;

                // Allow character to block
                else if (this is EquipmentAttackAlteration)
                    return true;

                else if (this is EquipmentEquipAlteration)
                    return false;

                else if (this is EquipmentCurseAlteration)
                    return false;

                else if (this is SkillAlteration)
                {
                    if (this.Effect is AttackAttributeAuraAlterationEffect)
                        return false;

                    else if (this.Effect is AttackAttributeMeleeAlterationEffect)
                        return true;

                    else if (this.Effect is AttackAttributePassiveAlterationEffect)
                        return false;

                    else if (this.Effect is AttackAttributeTemporaryAlterationEffect)
                        return true;

                    else if (this.Effect is AuraAlterationEffect)
                        return false;

                    // Nah...
                    else if (this.Effect is ChangeLevelAlterationEffect)
                        return false;

                    // Could be - but usually no.. better to keep it simple
                    else if (this.Effect is CreateMonsterAlterationEffect)
                        return false;

                    // Yes, because using these on other characters involves some kind of
                    // negative modification (like, "acid eats my armor"). So, need a blocking type
                    else if (this.Effect is EquipmentModifyAlterationEffect)
                        return true;

                    else if (this.Effect is OtherAlterationEffect)
                    {
                        switch ((this.Effect as OtherAlterationEffect).Type)
                        {
                            case AlterationOtherEffectType.Identify:
                            case AlterationOtherEffectType.Uncurse:
                                return false;
                            default:
                                throw new Exception("Unhandled AlterationOtherEffectType");
                        }
                    }

                    else if (this.Effect is PassiveAlterationEffect)
                        return false;

                    else if (this.Effect is PermanentAlterationEffect)
                        return true;

                    else if (this.Effect is RemedyAlterationEffect)
                        return false;

                    else if (this.Effect is RevealAlterationEffect)
                        return false;

                    else if (this.Effect is RunAwayAlterationEffect)
                        return false;

                    else if (this.Effect is StealAlterationEffect)
                        return true;

                    else if (this.Effect is TeleportAlterationEffect)
                        return true;

                    else if (this.Effect is TemporaryAlterationEffect)
                        return true;

                    else
                        throw new Exception("Unhandled Alteration Effect type");
                }

                else
                    throw new Exception("Unhandled Alteration Type");
            }
        }

        public AlterationBase()
        {
            this.Cost = new AlterationCost();
            this.AnimationGroup = new AnimationContainer();
        }


        /// <summary>
        /// This method should validate the interface type for the Effect public property of the 
        /// derived class
        /// </summary>
        protected abstract bool ValidateEffectInterfaceType();
    }
}
