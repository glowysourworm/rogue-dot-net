﻿using Rogue.NET.Core.Model.Scenario.Alteration.Common;
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

            if (cost.Hunger != 0)
                this.AlterationCostAttributes.Add(new AlterationAttributeViewModel("Hunger", cost.Hunger.ToString("F1")));

            if (cost.Experience != 0)
                this.AlterationCostAttributes.Add(new AlterationAttributeViewModel("Experience", cost.Experience.ToString("F1")));

            if (cost.Health != 0)
                this.AlterationCostAttributes.Add(new AlterationAttributeViewModel("Health", cost.Health.ToString("F1")));

            if (cost.Stamina != 0)
                this.AlterationCostAttributes.Add(new AlterationAttributeViewModel("Stamina", cost.Stamina.ToString("F1")));
        }

        public AlterationCostViewModel(AlterationCostTemplate template)
        {
            this.AlterationCostAttributes = new ObservableCollection<AlterationAttributeViewModel>();

            if (template.Hunger != 0)
                this.AlterationCostAttributes.Add(new AlterationAttributeViewModel("Hunger", template.Hunger.ToString("F1")));

            if (template.Experience != 0)
                this.AlterationCostAttributes.Add(new AlterationAttributeViewModel("Experience", template.Experience.ToString("F1")));

            if (template.Health != 0)
                this.AlterationCostAttributes.Add(new AlterationAttributeViewModel("Health", template.Health.ToString("F1")));

            if (template.Stamina != 0)
                this.AlterationCostAttributes.Add(new AlterationAttributeViewModel("Stamina", template.Stamina.ToString("F1")));
        }
    }
}
