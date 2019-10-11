using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.ScenarioEditor.Events.Asset;
using Rogue.NET.ScenarioEditor.Events.Browser;
using Rogue.NET.ScenarioEditor.Utility;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration;
using Rogue.NET.ScenarioEditor.Views.Controls.Symbol;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
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

namespace Rogue.NET.ScenarioEditor.Views.Design.GeneralDesign
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export]
    public partial class AlteredCharacterStates : UserControl
    {
        [ImportingConstructor]
        public AlteredCharacterStates(IRogueEventAggregator eventAggregator)
        {
            InitializeComponent();

            this.AddAlteredStateButton.Click += (sender, e) =>
            {
                var alteredStates = this.DataContext as IList<AlteredCharacterStateTemplateViewModel>;

                if (alteredStates == null || string.IsNullOrEmpty(this.AlteredStateTB.Text))
                    return;

                if (alteredStates.Any(x => x.Name == this.AlteredStateTB.Text))
                    return;

                eventAggregator.GetEvent<AddGeneralAssetEvent>().Publish(new AlteredCharacterStateTemplateViewModel()
                {
                    Name = this.AlteredStateTB.Text,
                    BaseType = (CharacterStateType)this.AlteredStateEnumCB.EnumValue
                });

                this.AlteredStateTB.Text = "";
            };

            this.RemoveAlteredStateButton.Click += (sender, e) =>
            {
                var selectedItem = this.AlteredStateLB.SelectedItem as AlteredCharacterStateTemplateViewModel;

                if (selectedItem != null)
                {
                    eventAggregator.GetEvent<RemoveGeneralAssetEvent>().Publish(selectedItem);
                }
            };
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
    }
}
