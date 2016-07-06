

using Rogue.NET.Model;
using Rogue.NET.ScenarioEditor.Views.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace Rogue.NET.ScenarioEditor.Views.Assets
{
    /// <summary>
    /// Interaction logic for SkillSet.xaml
    /// </summary>
    public partial class SkillSet : UserControl
    {
        public SkillSet()
        {
            InitializeComponent();

            this.DataContextChanged += SkillSet_DataContextChanged;
        }

        private void SkillSet_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var skillSet = e.NewValue as SkillSetTemplate;
            if (skillSet != null)
            {
                var collection = new ObservableCollection<SpellTemplate>(skillSet.Spells);
                collection.CollectionChanged += (obj, ev) =>
                {
                    skillSet.Spells.Clear();
                    foreach (var spell in collection)
                        skillSet.Spells.Add(spell);
                };
                this.SkillSetBuilder.DestinationLB.ItemsSource = collection;
            }
        }

        /// <summary>
        /// Use to set skill collections
        /// </summary>
        public void SetConfigurationData(ScenarioConfiguration configuration)
        {
            this.SkillSetBuilder.SourceLB.ItemsSource = new ObservableCollection<SpellTemplate>(configuration.MagicSpells);
            this.SkillSetBuilder.SourceLB.DisplayMemberPath = "Name";
        }

        private void CreateSymbol_Click(object sender, RoutedEventArgs e)
        {
            var window = new Window();
            var model = this.DataContext as SkillSetTemplate;
            var copy = (SymbolDetailsTemplate)ResourceManager.CreateDeepCopy(model.SymbolDetails);

            window.Content = new SymbolEditor();
            var ctrl = window.Content as SymbolEditor;
            ctrl.Width = 600;
            ctrl.DataContext = copy;
            ctrl.WindowMode = true;
            window.SizeToContent = SizeToContent.WidthAndHeight;
            window.ResizeMode = ResizeMode.NoResize;

            if ((bool)window.ShowDialog())
                model.SymbolDetails = copy;
        }
    }
}
