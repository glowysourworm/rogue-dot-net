using Rogue.NET.Core.Model.Enums;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Content;
using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Assets
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export]
    public partial class Consumable : UserControl
    {
        [ImportingConstructor]
        public Consumable()
        {
            InitializeComponent();

            // Set symbol tab to be the default to show for the consumable
            this.Loaded += (sender, e) =>
            {
                this.DefaultTab.IsSelected = true;
            };

            this.DataContextChanged += (sender, e) =>
            {
                var newViewModel = e.NewValue as ConsumableTemplateViewModel;
                var oldViewModel = e.OldValue as ConsumableTemplateViewModel;

                if (oldViewModel != null)
                    oldViewModel.PropertyChanged -= OnConsumablePropertyChanged;

                if (newViewModel != null)
                    newViewModel.PropertyChanged += OnConsumablePropertyChanged;
            };
        }

        private void OnConsumablePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var viewModel = sender as ConsumableTemplateViewModel;
            if (viewModel != null &&
                e.PropertyName == "SubType")
            {
                // NOTE*** This data-keeping could be centralized to a service. Also, 
                //         considered making a separate asset collection for ammo; but
                //         didn't want to add to the clutter. If ammo "grows up" any more
                //         then we'll have to consider doing something else.
                //
                // For ammunition - must reset properties to prevent dangling parameters
                if (viewModel.SubType == ConsumableSubType.Ammo)
                {
                    viewModel.HasAlteration = false;
                    viewModel.HasCharacterClassRequirement = false;
                    viewModel.HasLearnedSkill = false;
                    viewModel.HasProjectileAlteration = false;
                    viewModel.Type = ConsumableType.OneUse;
                    viewModel.SymbolDetails.SymbolType = SymbolType.OrientedSymbol;
                }
            }
        }
    }
}
