using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration;
using System.Collections.ObjectModel;

namespace Rogue.NET.Scenario.Content.ViewModel.Content.Alteration.Common
{
    public class AlterationCostViewModel : RogueBaseViewModel
    {
        public ObservableCollection<AlterationAttributeViewModel> AlterationCostAttributes { get; set; }

        public AlterationCostViewModel(AlterationCost cost)
        {
            this.AlterationCostAttributes = new ObservableCollection<AlterationAttributeViewModel>();

            if (cost.Strength != 0)
                this.AlterationCostAttributes.Add(new AlterationAttributeViewModel("Strength", cost.Strength.ToString("F1")));

            if (cost.Agility != 0)
                this.AlterationCostAttributes.Add(new AlterationAttributeViewModel("Agility", cost.Agility.ToString("F1")));

            if (cost.Intelligence != 0)
                this.AlterationCostAttributes.Add(new AlterationAttributeViewModel("Intelligence", cost.Intelligence.ToString("F1")));

            if (cost.Speed != 0)
                this.AlterationCostAttributes.Add(new AlterationAttributeViewModel("Speed", cost.Speed.ToString("F1")));

            if (cost.LightRadius != 0)
                this.AlterationCostAttributes.Add(new AlterationAttributeViewModel("Light Radius", cost.LightRadius.ToString("N0")));

            if (cost.FoodUsagePerTurn != 0)
                this.AlterationCostAttributes.Add(new AlterationAttributeViewModel("Food Usage (per turn)", cost.FoodUsagePerTurn.ToString("F1")));

            if (cost.Hunger != 0)
                this.AlterationCostAttributes.Add(new AlterationAttributeViewModel("Hunger", cost.Hunger.ToString("F1")));

            if (cost.Experience != 0)
                this.AlterationCostAttributes.Add(new AlterationAttributeViewModel("Experience", cost.Experience.ToString("F1")));

            if (cost.Hp != 0)
                this.AlterationCostAttributes.Add(new AlterationAttributeViewModel("Hp", cost.Hp.ToString("F1")));

            if (cost.Mp != 0)
                this.AlterationCostAttributes.Add(new AlterationAttributeViewModel("Mp", cost.Mp.ToString("F1")));
        }

        public AlterationCostViewModel(AlterationCostTemplate template)
        {
            this.AlterationCostAttributes = new ObservableCollection<AlterationAttributeViewModel>();

            if (template.Strength != 0)
                this.AlterationCostAttributes.Add(new AlterationAttributeViewModel("Strength", template.Strength.ToString("F1")));

            if (template.Agility != 0)
                this.AlterationCostAttributes.Add(new AlterationAttributeViewModel("Agility", template.Agility.ToString("F1")));

            if (template.Intelligence != 0)
                this.AlterationCostAttributes.Add(new AlterationAttributeViewModel("Intelligence", template.Intelligence.ToString("F1")));

            if (template.Speed != 0)
                this.AlterationCostAttributes.Add(new AlterationAttributeViewModel("Speed", template.Speed.ToString("F1")));

            if (template.AuraRadius != 0)
                this.AlterationCostAttributes.Add(new AlterationAttributeViewModel("Light Radius", template.AuraRadius.ToString("N0")));

            if (template.FoodUsagePerTurn != 0)
                this.AlterationCostAttributes.Add(new AlterationAttributeViewModel("Food Usage (per turn)", template.FoodUsagePerTurn.ToString("F1")));

            if (template.Hunger != 0)
                this.AlterationCostAttributes.Add(new AlterationAttributeViewModel("Hunger", template.Hunger.ToString("F1")));

            if (template.Experience != 0)
                this.AlterationCostAttributes.Add(new AlterationAttributeViewModel("Experience", template.Experience.ToString("F1")));

            if (template.Hp != 0)
                this.AlterationCostAttributes.Add(new AlterationAttributeViewModel("Hp", template.Hp.ToString("F1")));

            if (template.Mp != 0)
                this.AlterationCostAttributes.Add(new AlterationAttributeViewModel("Mp", template.Mp.ToString("F1")));
        }
    }
}
