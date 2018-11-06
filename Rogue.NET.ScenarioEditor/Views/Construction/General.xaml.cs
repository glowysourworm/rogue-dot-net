using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Construction
{
    [Export]
    public partial class General : UserControl
    {
        public General()
        {
            InitializeComponent();

            this.DataContextChanged += General_DataContextChanged;
        }

        private void General_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var config = e.NewValue as ScenarioConfigurationContainerViewModel;
            if (config == null)
                return;

            this.DataContext = config.DungeonTemplate;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO
            //var name = this.AttributeTB.Text;
            //// TODO
            ////var symbol = this.AttributeSymbolCB.Value;
            //_config.AttackAttributes.Add(new DungeonObjectTemplateViewModel()
            //{
            //    Name = name,
            //    SymbolDetails = new SymbolDetailsTemplateViewModel()
            //    {
            //        Type = SymbolTypes.Image,
            //        //Icon = symbol  TODO
            //    }
            //});

            //this.AttackAttribLB.ClearValue(ListBox.ItemsSourceProperty);
            //this.AttackAttribLB.ItemsSource = _config.AttackAttributes;
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO
            //var selectedItem = this.AttackAttribLB.SelectedItem as DungeonObjectTemplateViewModel;
            //if (selectedItem != null)
            //{
            //    _config.AttackAttributes.Remove(selectedItem);
            //    this.AttackAttribLB.ClearValue(ListBox.ItemsSourceProperty);
            //    this.AttackAttribLB.ItemsSource = _config.AttackAttributes;
            //}
        }

        private void AttackAttribLB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
                this.NewAttributeStack.DataContext = e.AddedItems[0];
        }
    }
}
