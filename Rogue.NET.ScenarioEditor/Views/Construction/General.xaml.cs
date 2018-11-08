using Prism.Events;
using Rogue.NET.ScenarioEditor.Events;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Layout;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Construction
{
    [Export]
    public partial class General : UserControl
    {
        readonly IEventAggregator _eventAggregator;

        [ImportingConstructor]
        public General(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;

            InitializeComponent();
        }


        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var config = this.DataContext as ScenarioConfigurationContainerViewModel;
            if (config == null || string.IsNullOrEmpty(this.AttributeTB.Text))
                return;

            if (config.AttackAttributes.Any(x => x.Name == this.AttributeTB.Text))
                return;

            var name = this.AttributeTB.Text;
            var symbol = this.AttributeSymbolCB.Value;

            _eventAggregator.GetEvent<AddAttackAttributeEvent>().Publish(new AddAttackAttributeEventArgs()
            {
                Icon = symbol,
                Name = name
            });

            this.AttributeTB.Text = "";
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = this.AttackAttribLB.SelectedItem as DungeonObjectTemplateViewModel;
            if (selectedItem != null)
            {
                _eventAggregator.GetEvent<RemoveAttackAttributeEvent>().Publish(selectedItem);               
            }
        }

        private void AttackAttribLB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
                this.NewAttributeStack.DataContext = e.AddedItems[0];
        }
    }
}
