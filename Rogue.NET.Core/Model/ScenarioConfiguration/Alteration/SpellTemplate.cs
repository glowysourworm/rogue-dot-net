using ProtoBuf;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using Rogue.NET.Core.Model.ScenarioConfiguration.Animation;
using System;
using System.Collections.Generic;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Alteration
{
    [Serializable]
    [ProtoContract(AsReferenceDefault = true, SkipConstructor = true)]
    public class SpellTemplate : DungeonObjectTemplate
    {
        private AlterationCostTemplate _cost;
        private AlterationEffectTemplate _effect;
        private AlterationEffectTemplate _auraEffect;
        private AlterationType _type;
        private AlterationMagicEffectType _otherEffectType;
        private AlterationAttackAttributeType _attackAttributeType;
        private double _effectRange;
        private bool _stackable;
        private bool _scaledByIntelligence;
        private string _createMonsterEnemy;
        private string _displayName;

        [ProtoMember(1, AsReference = true)]
        public List<AnimationTemplate> Animations { get; set; }
        [ProtoMember(2, AsReference = true)]
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
        [ProtoMember(3, AsReference = true)]
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
        [ProtoMember(4, AsReference = true)]
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
        [ProtoMember(5)]
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
        [ProtoMember(6)]
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
        [ProtoMember(7)]
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
        [ProtoMember(8)]
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
        [ProtoMember(9)]
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
        [ProtoMember(10)]
        public bool ScaledByIntelligence
        {
            get { return _scaledByIntelligence; }
            set
            {
                if (_scaledByIntelligence != value)
                {
                    _scaledByIntelligence = value;
                    OnPropertyChanged("ScaledByIntelligence");
                }
            }
        }
        [ProtoMember(11)]
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
        [ProtoMember(12)]
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
