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

            this.AttributeSymbolButton.DataContext = new SymbolDetailsTemplateViewModel();
        }

        #region Combat Attribute
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var config = this.DataContext as ScenarioConfigurationContainerViewModel;
            if (config == null || string.IsNullOrEmpty(this.AttributeTB.Text))
                return;

            if (config.AttackAttributes.Any(x => x.Name == this.AttributeTB.Text))
                return;

            _eventAggregator.GetEvent<AddAttackAttributeEvent>().Publish(new AttackAttributeTemplateViewModel()
            {
                Name = this.AttributeTB.Text,
                SymbolDetails = this.AttributeSymbolButton.DataContext as SymbolDetailsTemplateViewModel
            });

            this.AttributeTB.Text = "";
            this.AttributeSymbolButton.DataContext = new SymbolDetailsTemplateViewModel();
        }
        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = this.AttribLB.SelectedItem as AttackAttributeTemplateViewModel;
            if (selectedItem != null)
            {
                _eventAggregator.GetEvent<RemoveAttackAttributeEvent>().Publish(selectedItem);               
            }
        }
        private void AttributeSymbolButton_Click(object sender, RoutedEventArgs e)
        {
            var symbolDetails = this.AttributeSymbolButton.DataContext as SymbolDetailsTemplateViewModel;
            if (symbolDetails != null)
            {
                var view = new SymbolEditor();
                view.DataContext = symbolDetails;
                view.WindowMode = true;
                view.Width = 600;

                DialogWindowFactory.Show(view, "Rogue Symbol Editor");
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
