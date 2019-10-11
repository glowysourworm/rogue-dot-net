using Microsoft.Practices.ServiceLocation;
using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.ScenarioEditor.Events;
using Rogue.NET.ScenarioEditor.Service.Interface;
using Rogue.NET.ScenarioEditor.Utility;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using System;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Controls.Symbol
{
    public partial class SymbolEditor : UserControl
    {
        public static readonly DependencyProperty IsOrientedSymbolProperty =
            DependencyProperty.Register("IsOrientedSymbol", typeof(bool), typeof(SymbolEditor), new PropertyMetadata(false));

        public static readonly DependencyProperty AllowSymbolRandomizationProperty =
            DependencyProperty.Register("AllowSymbolRandomization", typeof(bool), typeof(SymbolEditor), new PropertyMetadata(false));

        public bool WindowMode
        {
            get { return this.ButtonGrid.Visibility == Visibility.Visible; }
            set
            {
                if (value)
                    this.ButtonGrid.Visibility = Visibility.Visible;
                else
                    this.ButtonGrid.Visibility = Visibility.Collapsed; 
            }
        }

        public bool IsOrientedSymbol
        {
            get { return (bool)GetValue(IsOrientedSymbolProperty); }
            set { SetValue(IsOrientedSymbolProperty, value); }
        }

        public bool AllowSymbolRandomization
        {
            get { return (bool)GetValue(AllowSymbolRandomizationProperty); }
            set { SetValue(AllowSymbolRandomizationProperty, value); }
        }

        public SymbolEditor()
        {
            var eventAggregator = ServiceLocator.Current.GetInstance<IRogueEventAggregator>();
            var scenarioCollectionProvider = ServiceLocator.Current.GetInstance<IScenarioCollectionProvider>();

            InitializeComponent();

            eventAggregator.GetEvent<ScenarioLoadedEvent>().Subscribe(configuration =>
            {
                this.SymbolPoolCategoryCB.ItemsSource = scenarioCollectionProvider.SymbolPool;
            });

            eventAggregator.GetEvent<ScenarioUpdateEvent>().Subscribe(collectionProvider =>
            {
                this.SymbolPoolCategoryCB.ItemsSource = collectionProvider.SymbolPool;
            });

            this.SymbolPoolCategoryCB.ItemsSource = scenarioCollectionProvider.SymbolPool;
        }

        private void CharacterSymbolButton_Click(object sender, RoutedEventArgs e)
        {
            var view = new CharacterMap();
            view.DataContext = this.DataContext;

            // Can be shown as a dialog
            DialogWindowFactory.Show(view, "Rogue UTF-8 Character Map");
        }
    }
}
