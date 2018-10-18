using Rogue.NET.Model;
using System.Windows;
using System.Windows.Controls;

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
