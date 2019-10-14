using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration.Interface;
using Rogue.NET.Core.Model.Scenario.Animation;
using System;

namespace Rogue.NET.Core.Model.Scenario.Alteration.Common
{
    [Serializable]
    public abstract class AlterationContainer : RogueBase
    {
        IAlterationEffect _effect;

        /// <summary>
        /// Primary Alteration Effect for the AlterationBase container. This should be overridden
        /// by the child class; but referenced by the appropriate interface handle.
        /// </summary>
        public IAlterationEffect Effect
        {
            get { return _effect; }
            set
            {
                if (_effect != value)
                    _effect = value;

                // Must validate IAlterationEffect to support the supplied
                // inherited interface from the AlterationContainer (implementation).
                //
                // This effectively creates a grouping of IAlterationEffect types
                // around the AlterationContainer classes. In other words, it constrains
                // the alteration effects to be used only by specific alteration containers.
                //
                // NOTE*** This was a design problem of trying to use a single container class
                //         to work with strongly typed alteration effect classes. Trying to use
                //         a class template would create several different container classes - 
                //         (one per template type) - which defeated the purpose of centralized
                //         processing.
                //
                //         So, the trick to get around it was to supply the child classes with
                //         a way to validate the type of alteration effect supplied - so that it
                //         worked with their functional purpose ("I'm a consumable projectile")
                //
                if (_effect != null &&
                    !this.EffectInterfaceType
                         .IsAssignableFrom(_effect.GetType()))
                    throw new Exception("Invalid IAlterationEffect Type");
            }
        }
        public AlterationCost Cost { get; set; }
        public AnimationSequence Animation { get; set; }
        public AlterationBlockType BlockType { get; set; }
        public AlterationCategory Category { get; set; }

        public AlterationContainer()
        {
            this.Cost = new AlterationCost();
            this.Animation = new AnimationSequence();
            this.Category = new AlterationCategory();
        }

        /// <summary>
        /// This is the Effect Grouping for the Alteration Container Type.
        /// 
        /// Example: ISkillAlterationEffect is a GROUP of IAlterationEffects that are supported by
        ///          the SkillAlteration container. 
        /// </summary>
        public abstract Type EffectInterfaceType { get; }
    }
}
