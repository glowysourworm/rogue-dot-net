using Prism.Events;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.ScenarioEditor.Events;
using Rogue.NET.ScenarioEditor.Utility;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Content;
using Rogue.NET.ScenarioEditor.Views.Controls;
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

        #region Combat Attribute
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var config = this.DataContext as ScenarioConfigurationContainerViewModel;
            if (config == null || string.IsNullOrEmpty(this.CombatAttributeTB.Text))
                return;

            if (config.AttackAttributes.Any(x => x.Name == this.CombatAttributeTB.Text))
                return;

            var name = this.CombatAttributeTB.Text;

            _eventAggregator.GetEvent<AddCombatAttributeEvent>().Publish(new CombatAttributeTemplateViewModel()
            {
                Name = name
            });

            this.CombatAttributeTB.Text = "";
        }
        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = this.CombatAttribLB.SelectedItem as CombatAttributeTemplateViewModel;
            if (selectedItem != null)
            {
                _eventAggregator.GetEvent<RemoveCombatAttributeEvent>().Publish(selectedItem);               
            }
        }
        private void AttributeSymbolButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = this.CombatAttribLB.SelectedItem as CombatAttributeTemplateViewModel;
            if (selectedItem != null)
            {
                var view = new SymbolEditor();
                view.DataContext = selectedItem.SymbolDetails;
                view.WindowMode = true;
                view.Width = 600;

                DialogWindowFactory.Show(view, "Rogue Symbol Editor");

                // Update Combat Attributes
                _eventAggregator.GetEvent<UpdateCombatAttributeEvent>().Publish(selectedItem);
            }
        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Update Combat Attributes
            var selectedItem = this.CombatAttribLB.SelectedItem as CombatAttributeTemplateViewModel;
            if (selectedItem != null)
            {
                _eventAggregator.GetEvent<UpdateCombatAttributeEvent>().Publish(selectedItem);
            }
        }
        #endregion

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
