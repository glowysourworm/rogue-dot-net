using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.ScenarioEditor.Service.Interface;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Content;
using Rogue.NET.ScenarioEditor.Views.Assets.SharedControl;
using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Assets
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export]
    public partial class Equipment : UserControl
    {
        readonly IRogueEventAggregator _eventAggregator;
        readonly IScenarioCollectionProvider _scenarioCollectionProvider;

        [ImportingConstructor]
        public Equipment(IRogueEventAggregator eventAggregator, IScenarioCollectionProvider scenarioCollectionProvider)
        {
            _eventAggregator = eventAggregator;
            _scenarioCollectionProvider = scenarioCollectionProvider;

            InitializeComponent();

            // Set symbol tab to be the default to show for the consumable
            this.Loaded += (sender, e) =>
            {
                this.DefaultTab.IsSelected = true;
            };

            this.DataContextChanged += (sender, e) =>
            {
                var oldViewModel = e.OldValue as EquipmentTemplateViewModel;
                var newViewModel = e.NewValue as EquipmentTemplateViewModel;

                if (oldViewModel != null)
                    oldViewModel.PropertyChanged -= OnEquipmentPropertyChanged;

                if (newViewModel != null)
                {
                    newViewModel.PropertyChanged += OnEquipmentPropertyChanged;

                    // Show Attack Attributes
                    if (IsWeaponType(newViewModel.Type) ||
                        IsArmorType(newViewModel.Type))
                    {
                        this.AttackAttributesTab.Visibility = System.Windows.Visibility.Visible;

                        ConstructAttackAttributesControl(eventAggregator, scenarioCollectionProvider, newViewModel);
                    }

                    else
                        this.AttackAttributesTab.Visibility = System.Windows.Visibility.Collapsed;
                }
            };
        }

        private void ConstructAttackAttributesControl(
                        IRogueEventAggregator eventAggregator, 
                        IScenarioCollectionProvider scenarioCollectionProvider,
                        EquipmentTemplateViewModel viewModel)
        {
            var view = new AttackAttributeListControl(eventAggregator, scenarioCollectionProvider);

            view.DataContext = viewModel.AttackAttributes;

            view.AttackAttributeCountLimit = 1;
            view.ShowAttack = IsWeaponType(viewModel.Type);
            view.ShowResistance = IsArmorType(viewModel.Type);
            view.ShowWeakness = IsArmorType(viewModel.Type);
            view.ShowImmune = IsArmorType(viewModel.Type);

            this.AttackAttributesTab.Content = view;
        }

        private bool IsWeaponType(EquipmentType type)
        {
            switch (type)
            {
                default:
                case EquipmentType.None:
                case EquipmentType.Armor:
                case EquipmentType.Shoulder:
                case EquipmentType.Belt:
                case EquipmentType.Helmet:
                case EquipmentType.Ring:
                case EquipmentType.Amulet:
                case EquipmentType.Boots:
                case EquipmentType.Gauntlets:
                case EquipmentType.Shield:
                case EquipmentType.Orb:
                    return false;
                case EquipmentType.OneHandedMeleeWeapon:
                case EquipmentType.TwoHandedMeleeWeapon:
                case EquipmentType.RangeWeapon:
                    return true;
            }
        }

        private bool IsArmorType(EquipmentType type)
        {
            switch (type)
            {
                case EquipmentType.Armor:
                case EquipmentType.Shoulder:
                case EquipmentType.Belt:
                case EquipmentType.Helmet:
                case EquipmentType.Boots:
                case EquipmentType.Gauntlets:
                case EquipmentType.Shield:
                    return true;
                default:
                case EquipmentType.None:
                case EquipmentType.Orb:
                case EquipmentType.Ring:
                case EquipmentType.Amulet:
                case EquipmentType.OneHandedMeleeWeapon:
                case EquipmentType.TwoHandedMeleeWeapon:
                case EquipmentType.RangeWeapon:
                    return false;
            }
        }

        private void OnEquipmentPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var viewModel = this.DataContext as EquipmentTemplateViewModel;

            if (e.PropertyName == "Type" &&
                viewModel != null)
            {
                // Since the type changed - reset the attack attributes to prevent saved attributes that can't
                // be altered
                foreach (var attackAttribute in viewModel.AttackAttributes)
                {
                    attackAttribute.Attack.Low = 0;
                    attackAttribute.Attack.High = 0;
                    attackAttribute.Resistance.Low = 0;
                    attackAttribute.Resistance.High = 0;
                    attackAttribute.Weakness.Low = 0;
                    attackAttribute.Weakness.High = 0;
                    attackAttribute.Immune = false;
                }

                ConstructAttackAttributesControl(_eventAggregator, _scenarioCollectionProvider, viewModel);
            }
        }
    }
}
