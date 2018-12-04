using Prism.Events;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.ScenarioEditor.Events;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration;
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

        private void AddAlteredStateButton_Click(object sender, RoutedEventArgs e)
        {
            var config = this.DataContext as ScenarioConfigurationContainerViewModel;
            if (config == null || string.IsNullOrEmpty(this.AlteredStateTB.Text))
                return;

            if (config.AlteredCharacterStates.Any(x => x.Name == this.AlteredStateTB.Text))
                return;

            var name = this.AlteredStateTB.Text;
            var symbol = this.AlteredStateSymbolCB.Value;
            var baseType = this.AlteredStateEnumCB.EnumValue;

            _eventAggregator.GetEvent<AddAlteredCharacterStateEvent>().Publish(new AddAlteredCharacterStateEventArgs()
            {
                Icon = symbol,
                Name = name,
                BaseType = (CharacterStateType)baseType
            });

            this.AlteredStateTB.Text = "";
        }

        private void RemoveAlteredStateButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = this.AlteredStateLB.SelectedItem as AlteredCharacterStateTemplateViewModel;
            if (selectedItem != null)
            {
                _eventAggregator.GetEvent<RemoveAlteredCharacterStateEvent>().Publish(selectedItem);
            }
        }

        private void AlteredStateLB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
                this.AlteredStateStack.DataContext = e.AddedItems[0];
        }
    }
}
