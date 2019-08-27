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

                if (_effect != null &&
                    !ValidateEffectType())
                    throw new Exception("Invalid IAlterationEffect Type");
            }
        }
        public AlterationCost Cost { get; set; }
        public AnimationGroup AnimationGroup { get; set; }
        public AlterationBlockType BlockType { get; set; }

        public AlterationContainer()
        {
            this.Cost = new AlterationCost();
            this.AnimationGroup = new AnimationGroup();
        }

        protected abstract bool ValidateEffectType();
    }
}
