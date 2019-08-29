using Rogue.NET.Common.Extension;
using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.ScenarioEditor.Events;
using Rogue.NET.ScenarioEditor.Utility;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Animation;
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
        readonly IRogueEventAggregator _eventAggregator;

        [ImportingConstructor]
        public General(IRogueEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;

            InitializeComponent();

            this.AttributeSymbolButton.DataContext = new SymbolDetailsTemplateViewModel();
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
                _eventAggregator.GetEvent<RemoveGeneralAssetEvent>().Publish(selectedItem);               
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
                SymbolDetails = new SymbolDetailsTemplateViewModel()
                {
                    Icon = this.AlteredStateSymbolCB.Value,
                    Type = SymbolTypes.Image
                },
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

        private void AlteredStateLB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
                this.AlteredStateStack.DataContext = e.AddedItems[0];
        }
        #endregion

        #region Character Classes
        private void AddCharacterClassButton_Click(object sender, RoutedEventArgs e)
        {
            var config = this.DataContext as ScenarioConfigurationContainerViewModel;
            if (config == null || string.IsNullOrEmpty(this.CharacterClassTB.Text))
                return;

            // TODO:CHARACTERCLASS
            //if (config.CharacterClasses.Any(x => x.Name == this.CharacterClassTB.Text))
            //    return;

            //_eventAggregator.GetEvent<AddGeneralAssetEvent>().Publish(new CharacterClassTemplateViewModel()
            //{
            //    Name = this.CharacterClassTB.Text,
            //    BonusAttackAttributes = config.AttackAttributes.DeepClone(),
            //    SymbolDetails = new SymbolDetailsTemplateViewModel()
            //    {
            //        Type = SymbolTypes.DisplayImage,
            //        DisplayIcon = this.CharacterClassSymbolCB.Value
            //    }
            //});

            this.CharacterClassTB.Text = "";
        }

        private void RemoveCharacterClassButton_Click(object sender, RoutedEventArgs e)
        {
            //var selectedItem = this.CharacterClassLB.SelectedItem as CharacterClassTemplateViewModel;
            //if (selectedItem != null)
            //{
            //    _eventAggregator.GetEvent<RemoveGeneralAssetEvent>().Publish(selectedItem);
            //}
        }
        #endregion
    }
}
