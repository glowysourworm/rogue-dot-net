using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.ScenarioEditor.Events.Asset;
using Rogue.NET.ScenarioEditor.Events.Browser;
using Rogue.NET.ScenarioEditor.Utility;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Content;
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
    public partial class AttackAttributes : UserControl
    {
        [ImportingConstructor]
        public AttackAttributes(IRogueEventAggregator eventAggregator)
        {
            InitializeComponent();

            this.AddAttributeButton.Click += (sender, e) =>
            {
                var attackAttributes = this.DataContext as IList<AttackAttributeTemplateViewModel>;

                if (attackAttributes == null || string.IsNullOrEmpty(this.AttributeTB.Text))
                    return;

                if (attackAttributes.Any(x => x.Name == this.AttributeTB.Text))
                    return;

                eventAggregator.GetEvent<AddGeneralAssetEvent>().Publish(new AttackAttributeTemplateViewModel()
                {
                    Name = this.AttributeTB.Text
                });

                this.AttributeTB.Text = "";
            };

            this.RemoveAttributeButton.Click += (sender, e) =>
            {
                var selectedItem = this.AttribLB.SelectedItem as AttackAttributeTemplateViewModel;

                if (selectedItem != null)
                {
                    eventAggregator.GetEvent<RemoveGeneralAssetEvent>().Publish(selectedItem);
                }
            };
        }

        private void AttackAttributeStateSymbolButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;

            if (button != null)
            {
                var viewModel = button.DataContext as AttackAttributeTemplateViewModel;
                if (viewModel != null)
                {
                    DialogWindowFactory.ShowSymbolEditor(viewModel.SymbolDetails);
                }
            }
        }
    }
}
