using Rogue.NET.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Rogue.NET.ScenarioEditor.Views.Construction
{
    public partial class General : UserControl
    {
        ScenarioConfiguration _config;

        public General()
        {
            InitializeComponent();
        }

        public void SetConfigurationParameters(ScenarioConfiguration config)
        {
            _config = config;

            this.AttackAttribLB.ItemsSource = config.AttackAttributes;

            this.ConsumablesLB.SourceLB.ItemsSource = config.ConsumableTemplates.
                Select(x => new ProbabilityConsumableTemplate() { Name = x.Name, TheTemplate = x });
            this.ConsumablesLB.SourceLB.DisplayMemberPath = "TheTemplate.Name";

            this.EquipmentLB.SourceLB.ItemsSource = config.EquipmentTemplates.
                Select(x => new ProbabilityConsumableTemplate() { Name = x.Name, TheTemplate = x });
            this.EquipmentLB.SourceLB.DisplayMemberPath = "TheTemplate.Name";

            var consumables = new ObservableCollection<ProbabilityConsumableTemplate>(config.DungeonTemplate.ShopConsumables);
            var equipment = new ObservableCollection<ProbabilityEquipmentTemplate>(config.DungeonTemplate.ShopEquipment);

            consumables.CollectionChanged += (obj, e) =>
            {
                _config.DungeonTemplate.ShopConsumables.Clear();
                foreach (var item in consumables)
                    _config.DungeonTemplate.ShopConsumables.Add(item);
            };

            equipment.CollectionChanged += (obj, e) =>
            {
                _config.DungeonTemplate.ShopEquipment.Clear();
                foreach (var item in equipment)
                    _config.DungeonTemplate.ShopEquipment.Add(item);
            };

            this.ConsumablesLB.DestinationLB.ItemsSource = consumables;
            this.EquipmentLB.DestinationLB.ItemsSource = equipment;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var name = this.AttributeTB.Text;
            var symbol = this.AttributeSymbolCB.Value;
            _config.AttackAttributes.Add(new DungeonObjectTemplate()
            {
                Name = name,
                SymbolDetails = new SymbolDetailsTemplate()
                {
                    Type = Common.SymbolTypes.Image,
                    Icon = symbol
                }
            });

            this.AttackAttribLB.ClearValue(ListBox.ItemsSourceProperty);
            this.AttackAttribLB.ItemsSource = _config.AttackAttributes;
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = this.AttackAttribLB.SelectedItem as DungeonObjectTemplate;
            if (selectedItem != null)
            {
                _config.AttackAttributes.Remove(selectedItem);
                this.AttackAttribLB.ClearValue(ListBox.ItemsSourceProperty);
                this.AttackAttribLB.ItemsSource = _config.AttackAttributes;
            }
        }

        private void AttackAttribLB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
                this.NewAttributeStack.DataContext = e.AddedItems[0];
        }
    }
}
