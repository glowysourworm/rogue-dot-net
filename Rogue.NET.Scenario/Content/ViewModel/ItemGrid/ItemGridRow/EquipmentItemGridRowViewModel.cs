using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario;
using Rogue.NET.Core.Model.Scenario.Content.Item;
using Rogue.NET.Core.Model.Scenario.Content.Item.Extension;
using Rogue.NET.Core.Processing.Model.Static;
using Rogue.NET.Scenario.Content.ViewModel.Content.Alteration.Common;

namespace Rogue.NET.Scenario.Content.ViewModel.ItemGrid.ItemGridRow
{
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export]
    public class EquipmentItemGridRowViewModel : ItemGridRowViewModel<Equipment>
    {
        #region Backing Fields
        int _requiredLevel;
        EquipmentType _type;
        CharacterBaseAttribute _combatType;
        int _class;
        double _weight;
        double _quality;
        double _combatValue;
        bool _isEquipped;
        bool _isCursed;
        bool _isCurseIdentified;
        bool _hasCharacterClassRequirement;
        string _characterClass;
        #endregion

        public int RequiredLevel
        {
            get { return _requiredLevel; }
            set { this.RaiseAndSetIfChanged(ref _requiredLevel, value); }
        }
        public EquipmentType Type
        {
            get { return _type; }
            set { this.RaiseAndSetIfChanged(ref _type, value); }
        }
        public CharacterBaseAttribute CombatType
        {
            get { return _combatType; }
            set { this.RaiseAndSetIfChanged(ref _combatType, value); }
        }
        public int Class
        {
            get { return _class; }
            set { this.RaiseAndSetIfChanged(ref _class, value); }
        }
        public double Weight
        {
            get { return _weight; }
            set { this.RaiseAndSetIfChanged(ref _weight, value); }
        }
        public double Quality
        {
            get { return _quality; }
            set { this.RaiseAndSetIfChanged(ref _quality, value); }
        }
        public double CombatValue
        {
            get { return _combatValue; }
            set { this.RaiseAndSetIfChanged(ref _combatValue, value); }
        }
        public bool IsEquipped
        {
            get { return _isEquipped; }
            set { this.RaiseAndSetIfChanged(ref _isEquipped, value); }
        }
        public bool IsCursed
        {
            get { return _isCursed; }
            set { this.RaiseAndSetIfChanged(ref _isCursed, value); }
        }
        public bool IsCurseIdentified
        {
            get { return _isCurseIdentified; }
            set { this.RaiseAndSetIfChanged(ref _isCurseIdentified, value); }
        }
        public bool HasCharacterClassRequirement
        {
            get { return _hasCharacterClassRequirement; }
            set { this.RaiseAndSetIfChanged(ref _hasCharacterClassRequirement, value); }
        }
        public string CharacterClass
        {
            get { return _characterClass; }
            set { this.RaiseAndSetIfChanged(ref _characterClass, value); }
        }

        public ObservableCollection<AttackAttributeViewModel> AttackAttributes { get; set; }

        public EquipmentItemGridRowViewModel(Equipment equipment, ScenarioMetaData metaData, string displayName, bool isEnabled) : base(equipment, metaData, displayName, isEnabled)
        {
            this.AttackAttributes = new ObservableCollection<AttackAttributeViewModel>();

            Update(equipment, metaData, displayName, isEnabled);
        }

        public override void Update(Equipment equipment, ScenarioMetaData metaData, string displayName, bool isEnabled)
        {
            base.Update(equipment, metaData, displayName, isEnabled);

            this.IsEquipped = equipment.IsEquipped;
            this.IsCursed = equipment.IsCursed;
            this.IsCurseIdentified = metaData.IsCurseIdentified;

            this.HasCharacterClassRequirement = equipment.HasCharacterClassRequirement;
            this.RequiredLevel = equipment.LevelRequired;
            this.Type = equipment.Type;
            this.CombatType = equipment.CombatType;
            this.Class = equipment.Class;
            this.Weight = equipment.Weight;
            this.Quality = equipment.Quality;
            this.CombatValue = equipment.IsWeaponType() ? EquipmentCalculator.GetAttackValue(equipment.Type, equipment.Class, equipment.Quality)
                              : equipment.IsArmorType() ? EquipmentCalculator.GetDefenseValue(equipment.Type, equipment.Class, equipment.Quality)
                              : 0D;
            this.CharacterClass = equipment.CharacterClass;

            // Update Attack Attribute ccollection
            this.AttackAttributes.SynchronizeFrom(
                equipment.AttackAttributes,
                (x, y) => x.RogueName == y.RogueName,
                attackAttribute => new AttackAttributeViewModel(attackAttribute),
                (model, viewModel) =>
                {
                    viewModel.Attack = model.Attack;
                    viewModel.Resistance = model.Resistance;
                    viewModel.Weakness = model.Weakness;
                    viewModel.Immune = model.Immune;
                });
        }
    }
}
