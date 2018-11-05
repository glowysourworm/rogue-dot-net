using Rogue.NET.Core.Model.Enums;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Construction
{
    [Export]
    public partial class General : UserControl
    {
        ScenarioConfigurationContainerViewModel _config;

        public General()
        {
            InitializeComponent();
        }

        public void SetConfigurationParameters(ScenarioConfigurationContainerViewModel config)
        {
            _config = config;

            this.AttackAttribLB.ItemsSource = config.AttackAttributes;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var name = this.AttributeTB.Text;
            // TODO
            //var symbol = this.AttributeSymbolCB.Value;
            _config.AttackAttributes.Add(new DungeonObjectTemplateViewModel()
            {
                Name = name,
                SymbolDetails = new SymbolDetailsTemplateViewModel()
                {
                    Type = SymbolTypes.Image,
                    //Icon = symbol  TODO
                }
            });

            this.AttackAttribLB.ClearValue(ListBox.ItemsSourceProperty);
            this.AttackAttribLB.ItemsSource = _config.AttackAttributes;
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = this.AttackAttribLB.SelectedItem as DungeonObjectTemplateViewModel;
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
