using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Content;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Construction
{
    [Export]
    public partial class PlayerDesign : UserControl
    {
        public PlayerDesign()
        {
            InitializeComponent();
        }

        public void SetConfigurationParameters(ScenarioConfigurationContainerViewModel config)
        {
            this.ConsumablesLB.SourceLB.ItemsSource = config.ConsumableTemplates.Select(x => 
                new ProbabilityConsumableTemplateViewModel() { Name = x.Name, TheTemplate = x });
            this.EquipmentLB.SourceLB.ItemsSource = config.EquipmentTemplates.Select(x =>
                new ProbabilityEquipmentTemplateViewModel() { Name = x.Name, TheTemplate = x });
            this.SkillsLB.SourceLB.ItemsSource = config.SkillTemplates;

            this.ConsumablesLB.SourceLB.DisplayMemberPath = "Name";
            this.EquipmentLB.SourceLB.DisplayMemberPath = "Name";
            this.SkillsLB.SourceLB.DisplayMemberPath = "Name";

            var consumables = new ObservableCollection<ProbabilityConsumableTemplateViewModel>(config.PlayerTemplate.StartingConsumables);
            var equipment = new ObservableCollection<ProbabilityEquipmentTemplateViewModel>(config.PlayerTemplate.StartingEquipment);
            var skills = new ObservableCollection<SkillSetTemplateViewModel>(config.PlayerTemplate.Skills);

            consumables.CollectionChanged += (obj, e) =>
            {
                config.PlayerTemplate.StartingConsumables.Clear();
                foreach (var item in consumables)
                    config.PlayerTemplate.StartingConsumables.Add(item);
            };

            equipment.CollectionChanged += (obj, e) =>
            {
                config.PlayerTemplate.StartingEquipment.Clear();
                foreach (var item in equipment)
                    config.PlayerTemplate.StartingEquipment.Add(item);
            };

            skills.CollectionChanged += (obj, e) =>
            {
                config.PlayerTemplate.Skills.Clear();
                foreach (var skill in skills)
                    config.PlayerTemplate.Skills.Add(skill);
            };

            this.ConsumablesLB.DestinationLB.ItemsSource = consumables;
            this.EquipmentLB.DestinationLB.ItemsSource = equipment;
            this.SkillsLB.DestinationLB.ItemsSource = skills;
        }
    }
}
