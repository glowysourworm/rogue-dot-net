using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using Rogue.NET.Core.Model.ScenarioConfiguration.Animation;
using Rogue.NET.Core.Model.ScenarioConfiguration.Content;
using System;
using System.Collections.Generic;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Alteration
{
    // TODO: REFACTOR NAME OF CLASS TO REMOVE "SPELL" (Change to Alteration Template)
    [Serializable]
    public class SpellTemplate : DungeonObjectTemplate
    {
        private AlterationCostTemplate _cost;
        private AlterationEffectTemplate _effect;
        private AlterationEffectTemplate _auraEffect;
        private AlterationType _type;
        private AlterationBlockType _blockType;
        private AlterationMagicEffectType _otherEffectType;
        private AlterationAttackAttributeType _attackAttributeType;
        private double _effectRange;
        private bool _stackable;
        private string _createMonsterEnemy;
        private string _displayName;

        public List<AnimationTemplate> Animations { get; set; }
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
        public AlterationEffectTemplate Effect
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
        public AlterationEffectTemplate AuraEffect
        {
            get { return _auraEffect; }
            set
            {
                if (_auraEffect != value)
                {
                    _auraEffect = value;
                    OnPropertyChanged("AuraEffect");
                }
            }
        }
        public AlterationType Type
        {
            get { return _type; }
            set
            {
                if (_type != value)
                {
                    _type = value;
                    OnPropertyChanged("Type");
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
        public AlterationMagicEffectType OtherEffectType
        {
            get { return _otherEffectType; }
            set
            {
                if (_otherEffectType != value)
                {
                    _otherEffectType = value;
                    OnPropertyChanged("OtherEffectType");
                }
            }
        }
        public AlterationAttackAttributeType AttackAttributeType
        {
            get { return _attackAttributeType; }
            set
            {
                if (_attackAttributeType != value)
                {
                    _attackAttributeType = value;
                    OnPropertyChanged("AttackAttributeType");
                }
            }
        }
        public double EffectRange
        {
            get { return _effectRange; }
            set
            {
                if (_effectRange != value)
                {
                    _effectRange = value;
                    OnPropertyChanged("EffectRange");
                }
            }
        }
        public bool Stackable
        {
            get { return _stackable; }
            set
            {
                if (_stackable != value)
                {
                    _stackable = value;
                    OnPropertyChanged("Stackable");
                }
            }
        }
        public string CreateMonsterEnemy
        {
            get { return _createMonsterEnemy; }
            set
            {
                if (_createMonsterEnemy != value)
                {
                    _createMonsterEnemy = value;
                    OnPropertyChanged("CreateMonsterEnemy");
                }
            }
        }
        public string DisplayName
        {
            get { return _displayName; }
            set
            {
                if (_displayName != value)
                {
                    _displayName = value;
                    OnPropertyChanged("DisplayName");
                }
            }
        }

        public SpellTemplate()
        {
            this.Animations = new List<AnimationTemplate>();
            this.Cost = new AlterationCostTemplate();
            this.Effect = new AlterationEffectTemplate();
            this.AuraEffect = new AlterationEffectTemplate();

            this.CreateMonsterEnemy = "";
            this.DisplayName = "";
        }
    }
}
