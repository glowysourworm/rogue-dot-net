using Rogue.NET.Common.Extension;
using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.ScenarioEditor.Events;
using Rogue.NET.ScenarioEditor.Service.Interface;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Content;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Assets.SharedControl
{
    public partial class AttackAttributeListControl : UserControl
    {
        public static readonly DependencyProperty AttackAttributeCountLimitProperty =
            DependencyProperty.Register("AttackAttributeCountLimit", typeof(int), typeof(AttackAttributeListControl));

        public static readonly DependencyProperty ShowAttackProperty =
            DependencyProperty.Register("ShowAttack", typeof(bool), typeof(AttackAttributeListControl));

        public static readonly DependencyProperty ShowResistanceProperty =
            DependencyProperty.Register("ShowResistance", typeof(bool), typeof(AttackAttributeListControl));

        public static readonly DependencyProperty ShowWeaknessProperty =
            DependencyProperty.Register("ShowWeakness", typeof(bool), typeof(AttackAttributeListControl));

        public static readonly DependencyProperty ShowImmuneProperty =
            DependencyProperty.Register("ShowImmune", typeof(bool), typeof(AttackAttributeListControl));

        public int AttackAttributeCountLimit
        {
            get { return (int)GetValue(AttackAttributeCountLimitProperty); }
            set { SetValue(AttackAttributeCountLimitProperty, value); }
        }
        public bool ShowAttack
        {
            get { return (bool)GetValue(ShowAttackProperty); }
            set { SetValue(ShowAttackProperty, value); }
        }
        public bool ShowResistance
        {
            get { return (bool)GetValue(ShowResistanceProperty); }
            set { SetValue(ShowResistanceProperty, value); }
        }
        public bool ShowWeakness
        {
            get { return (bool)GetValue(ShowWeaknessProperty); }
            set { SetValue(ShowWeaknessProperty, value); }
        }
        public bool ShowImmune
        {
            get { return (bool)GetValue(ShowImmuneProperty); }
            set { SetValue(ShowImmuneProperty, value); }
        }

        public AttackAttributeListControl(
                IRogueEventAggregator eventAggregator,
                IScenarioCollectionProvider scenarioCollectionProvider)
        {
            InitializeComponent();

            eventAggregator.GetEvent<ScenarioLoadedEvent>().Subscribe(configuration =>
            {
                this.AttackAttributeCB.ItemsSource = scenarioCollectionProvider.AttackAttributes;
            });

            eventAggregator.GetEvent<ScenarioUpdateEvent>().Subscribe(provider =>
            {
                this.AttackAttributeCB.ItemsSource = provider.AttackAttributes;
            });

            this.Loaded += (sender, e) =>
            {
                this.AttackAttributeCB.ItemsSource = scenarioCollectionProvider.AttackAttributes;
            };
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = this.DataContext as IList<AttackAttributeTemplateViewModel>;
            var control = sender as Button;

            if (viewModel != null &&
                control != null)
            {
                var selectedItem = control.DataContext as AttackAttributeTemplateViewModel;
                if (selectedItem != null)
                    viewModel.Remove(selectedItem);

                // If limit met - then disable the combobox and add button
                if (viewModel.Count < this.AttackAttributeCountLimit)
                {
                    this.AttackAttributeCB.IsEnabled = true;
                    this.AddButton.IsEnabled = true;
                    this.LimitTB.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = this.DataContext as IList<AttackAttributeTemplateViewModel>;
            var selectedItem = this.AttackAttributeCB.SelectedItem as AttackAttributeTemplateViewModel;

            if (viewModel != null &&
                selectedItem != null &&
                viewModel.Count < this.AttackAttributeCountLimit &&         // Check Limit
               !viewModel.Any(x => x.Name == selectedItem.Name))            // Prevent Duplicates
                viewModel.Add(selectedItem.DeepClone());

            // If limit met - then disable the combobox and add button
            if (viewModel != null &&
                viewModel.Count >= this.AttackAttributeCountLimit)
            {
                this.AttackAttributeCB.IsEnabled = false;
                this.AddButton.IsEnabled = false;
                this.LimitTB.Visibility = Visibility.Visible;
            }
        }
    }
}
