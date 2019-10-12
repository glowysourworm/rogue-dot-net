using Rogue.NET.Common.Extension;
using Rogue.NET.ScenarioEditor.Utility;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration;
using Rogue.NET.ScenarioEditor.Views.Controls.Symbol;
using System;
using System.Collections.Generic;
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
    /// <summary>
    /// Interaction logic for AlterationCategories.xaml
    /// </summary>
    public partial class AlterationCategories : UserControl
    {
        public AlterationCategories()
        {
            InitializeComponent();

            this.AddAlterationCategoryButton.Click += (sender, e) =>
            {
                var viewModel = this.DataContext as IList<AlterationCategoryTemplateViewModel>;
                var categoryName = this.AlterationCategoryTB.Text?.Trim() ?? null;

                if (viewModel != null &&
                    !string.IsNullOrWhiteSpace(categoryName) &&
                    viewModel.None(x => x.Name == categoryName))
                    viewModel.Add(new AlterationCategoryTemplateViewModel()
                    {
                        Name = categoryName
                    });
            };

            this.RemoveAlterationCategoryButton.Click += (sender, e) =>
            {
                var viewModel = this.DataContext as IList<AlterationCategoryTemplateViewModel>;
                var category = this.AlterationCategoryLB.SelectedItem as AlterationCategoryTemplateViewModel;

                if (viewModel != null &&
                    category != null)
                    viewModel.Remove(category);
            };
        }

        private void EditSymbolButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;

            if (button != null)
            {
                var viewModel = button.DataContext as AlterationCategoryTemplateViewModel;
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
