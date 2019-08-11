﻿using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Common;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Interface;
using Rogue.NET.Core.Model.ScenarioConfiguration.Animation;
using System;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Skill
{
    [Serializable]
    public class SkillAlterationTemplate : Template
    {
        private AnimationGroupTemplate _animationGroup;
        private AlterationCostTemplate _cost;
        private ISkillAlterationEffectTemplate _effect;
        private AlterationBlockType _blockType;
        private AuraSourceParametersTemplate _auraParameters;
        private AlterationTargetType _targetType;

        public AnimationGroupTemplate AnimationGroup
        {
            get { return _animationGroup; }
            set
            {
                if (_animationGroup != value)
                {
                    _animationGroup = value;
                    OnPropertyChanged("AnimationGroup");
                }
            }
        }
        public AlterationCostTemplate Cost
        {
            get { return _cost; }
            set
            {
                if (_cost != value)
                {
                    _cost = value;
                    OnPropertyChanged("Cost");
                }
            }
        }
        public ISkillAlterationEffectTemplate Effect
        {
            get { return _effect; }
            set
            {
                if (_effect != value)
                {
                    _effect = value;
                    OnPropertyChanged("Effect");
                }
            }
        }
        public AlterationBlockType BlockType
        {
            get { return _blockType; }
            set
            {
                if (_blockType != value)
                {
                    _blockType = value;
                    OnPropertyChanged("BlockType");
                }
            }
        }
        public AuraSourceParametersTemplate AuraParameters
        {
            get { return _auraParameters; }
            set
            {
                if (_auraParameters != value)
                {
                    _auraParameters = value;
                    OnPropertyChanged("BlockType");
                }
            }
        }
        public AlterationTargetType TargetType
        {
            get { return _targetType; }
            set
            {
                if (_targetType != value)
                {
                    _targetType = value;
                    OnPropertyChanged("TargetType");
                }
            }
        }

        public SkillAlterationTemplate()
        {
            this.AnimationGroup = new AnimationGroupTemplate();
            this.Cost = new AlterationCostTemplate();
            this.AuraParameters = new AuraSourceParametersTemplate();
        }
    }
}