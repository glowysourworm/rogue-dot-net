using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.ScenarioEditor.Events;
using Rogue.NET.ScenarioEditor.Events.Browser;
using Rogue.NET.ScenarioEditor.Utility;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Content;
using Rogue.NET.ScenarioEditor.Views.Controls.Symbol;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Design
{
    [Export]
    public partial class General : UserControl
    {
        readonly IRogueEventAggregator _eventAggregator;

        [ImportingConstructor]
        public General(IRogueEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;

            InitializeComponent();
        }

        #region Attack Attribute
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var config = this.DataContext as ScenarioConfigurationContainerViewModel;
            if (config == null || string.IsNullOrEmpty(this.AttributeTB.Text))
                return;

            if (config.AttackAttributes.Any(x => x.Name == this.AttributeTB.Text))
                return;

            _eventAggregator.GetEvent<AddGeneralAssetEvent>().Publish(new AttackAttributeTemplateViewModel()
            {
                Name = this.AttributeTB.Text
            });

            this.AttributeTB.Text = "";
        }
        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = this.AttribLB.SelectedItem as AttackAttributeTemplateViewModel;
            if (selectedItem != null)
            {
                _eventAggregator.GetEvent<RemoveGeneralAssetEvent>().Publish(selectedItem);               
            }
        }
        #endregion

        #region Altered State
        private void AddAlteredStateButton_Click(object sender, RoutedEventArgs e)
        {
            var config = this.DataContext as ScenarioConfigurationContainerViewModel;
            if (config == null || string.IsNullOrEmpty(this.AlteredStateTB.Text))
                return;

            if (config.AlteredCharacterStates.Any(x => x.Name == this.AlteredStateTB.Text))
                return;

            _eventAggregator.GetEvent<AddGeneralAssetEvent>().Publish(new AlteredCharacterStateTemplateViewModel()
            {                
                Name = this.AlteredStateTB.Text,
                BaseType = (CharacterStateType)this.AlteredStateEnumCB.EnumValue
            });

            this.AlteredStateTB.Text = "";
        }

        private void RemoveAlteredStateButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = this.AlteredStateLB.SelectedItem as AlteredCharacterStateTemplateViewModel;
            if (selectedItem != null)
            {
                _eventAggregator.GetEvent<RemoveGeneralAssetEvent>().Publish(selectedItem);
            }
        }
        private void AlteredStateSymbolButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;

            if (button != null)
            {
                var viewModel = button.DataContext as AlteredCharacterStateTemplateViewModel;
                if (viewModel != null)
                {
                    var view = new SymbolEditor();
                    view.DataContext = viewModel.SymbolDetails;

                    view.Width = 800;
                    view.Height = 600;

                    DialogWindowFactory.Show(view, "Rogue Symbol Editor");
                }
            }
        }
        #endregion
    }
}
