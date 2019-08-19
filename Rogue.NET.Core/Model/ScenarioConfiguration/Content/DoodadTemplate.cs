using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Doodad;
using System;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Content
{
    [Serializable]
    public class DoodadTemplate : DungeonObjectTemplate
    {
        private DoodadAlterationTemplate _automaticAlteration;
        private DoodadAlterationTemplate _invokedAlteration;
        private bool _isAutomatic;
        private bool _isVisible;
        private bool _isInvoked;
        private bool _isOneUse;
        private bool _hasCharacterClassRequirement;
        private CharacterClassTemplate _characterClass;

        public DoodadAlterationTemplate AutomaticAlteration
        {
            get { return _automaticAlteration; }
            set
            {
                if (_automaticAlteration != value)
                {
                    _automaticAlteration = value;
                    OnPropertyChanged("AutomaticAlteration");
                }
            }
        }
        public DoodadAlterationTemplate InvokedAlteration
        {
            get { return _invokedAlteration; }
            set
            {
                if (_invokedAlteration != value)
                {
                    _invokedAlteration = value;
                    OnPropertyChanged("InvokedAlteration");
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
        public bool HasCharacterClassRequirement
        {
            get { return _hasCharacterClassRequirement; }
            set
            {
                if (_hasCharacterClassRequirement != value)
                {
                    _hasCharacterClassRequirement = value;
                    OnPropertyChanged("HasCharacterClassRequirement");
                }
            }
        }
        public CharacterClassTemplate CharacterClass
        {
            get { return _characterClass; }
            set
            {
                if (_characterClass != value)
                {
                    _characterClass = value;
                    OnPropertyChanged("CharacterClass");
                }
            }
        }

        public DoodadTemplate()
        {
            this.AutomaticAlteration = new DoodadAlterationTemplate();
            this.InvokedAlteration = new DoodadAlterationTemplate();
            this.CharacterClass = new CharacterClassTemplate();
            this.IsUnique = false;
            this.IsOneUse = false;
        }
    }
}
