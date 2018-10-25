﻿using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration;
using System;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Content
{
    [Serializable]
    public class DoodadTemplate : DungeonObjectTemplate
    {
        private SpellTemplate _automaticMagicSpellTemplate;
        private SpellTemplate _invokedMagicSpellTemplate;
        private bool _isAutomatic;
        private bool _isVisible;
        private bool _isInvoked;
        private bool _isOneUse;

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