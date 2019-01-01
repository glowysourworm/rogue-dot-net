using ProtoBuf;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration;
using System;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Content
{
    [Serializable]
    [ProtoContract(AsReferenceDefault = true, SkipConstructor = true)]
    public class DoodadTemplate : DungeonObjectTemplate
    {
        private SpellTemplate _automaticMagicSpellTemplate;
        private SpellTemplate _invokedMagicSpellTemplate;
        private bool _isAutomatic;
        private bool _isVisible;
        private bool _isInvoked;
        private bool _isOneUse;

        [ProtoMember(1, AsReference = true)]
        public SpellTemplate AutomaticMagicSpellTemplate
        {
            get { return _automaticMagicSpellTemplate; }
            set
            {
                if (_automaticMagicSpellTemplate != value)
                {
                    _automaticMagicSpellTemplate = value;
                    OnPropertyChanged("AutomaticMagicSpellTemplate");
                }
            }
        }
        [ProtoMember(2, AsReference = true)]
        public SpellTemplate InvokedMagicSpellTemplate
        {
            get { return _invokedMagicSpellTemplate; }
            set
            {
                if (_invokedMagicSpellTemplate != value)
                {
                    _invokedMagicSpellTemplate = value;
                    OnPropertyChanged("InvokedMagicSpellTemplate");
                }
            }
        }
        [ProtoMember(3)]
        public bool IsAutomatic
        {
            get { return _isAutomatic; }
            set
            {
                if (_isAutomatic != value)
                {
                    _isAutomatic = value;
                    OnPropertyChanged("IsAutomatic");
                }
            }
        }
        [ProtoMember(4)]
        public bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                if (_isVisible != value)
                {
                    _isVisible = value;
                    OnPropertyChanged("IsVisible");
                }
            }
        }
        [ProtoMember(5)]
        public bool IsInvoked
        {
            get { return _isInvoked; }
            set
            {
                if (_isInvoked != value)
                {
                    _isInvoked = value;
                    OnPropertyChanged("IsInvoked");
                }
            }
        }
        [ProtoMember(6)]
        public bool IsOneUse
        {
            get { return _isOneUse; }
            set
            {
                if (_isOneUse != value)
                {
                    _isOneUse = value;
                    OnPropertyChanged("IsOneUse");
                }
            }
        }

        public DoodadTemplate()
        {
            this.AutomaticMagicSpellTemplate = new SpellTemplate();
            this.InvokedMagicSpellTemplate = new SpellTemplate();
            this.IsUnique = false;
            this.IsOneUse = false;
        }
        public DoodadTemplate(DungeonObjectTemplate tmp) : base(tmp)
        {
            this.AutomaticMagicSpellTemplate = new SpellTemplate();
            this.InvokedMagicSpellTemplate = new SpellTemplate();
            this.IsUnique = false;
            this.IsOneUse = false;
        }
    }
}
