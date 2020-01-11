using Microsoft.Practices.ServiceLocation;

using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Processing.Service.Interface;
using Rogue.NET.ScenarioEditor.Events;
using Rogue.NET.ScenarioEditor.Service.Interface;
using Rogue.NET.ScenarioEditor.Utility;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;

using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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

        readonly IScenarioResourceService _scenarioResourceService;

        public SymbolEditor()
        {
            var eventAggregator = ServiceLocator.Current.GetInstance<IRogueEventAggregator>();
            var scenarioCollectionProvider = ServiceLocator.Current.GetInstance<IScenarioCollectionProvider>();
            _scenarioResourceService = ServiceLocator.Current.GetInstance<IScenarioResourceService>();

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

            this.DataContextChanged += (sender, e) =>
            {
                var oldViewModel = e.NewValue as SymbolDetailsTemplateViewModel;
                var newViewModel = e.NewValue as SymbolDetailsTemplateViewModel;

                if (oldViewModel != null)
                    oldViewModel.PropertyChanged -= OnSymbolPropertyChanged;

                if (newViewModel != null)
                    newViewModel.PropertyChanged += OnSymbolPropertyChanged;
            };
        }

        private void OnSymbolPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var viewModel = this.DataContext as SymbolDetailsTemplateViewModel;

            if (e.PropertyName == "SymbolType" &&
                viewModel != null)
            {
                switch (viewModel.SymbolType)
                {
                    case SymbolType.Smiley:
                        break;
                    case SymbolType.Character:
                        {
                            // Reset symbol path to a known resource for this type
                            var category = _scenarioResourceService.GetCharacterCategories()
                                                                   .First();

                            // TODO: REMOVE THIS - ENSURE DATA VALID
                            viewModel.SymbolClampColor = viewModel.SymbolClampColor ?? Colors.White.ToString();

                            viewModel.SymbolPath = _scenarioResourceService.GetCharacterResourceNames(category)
                                                                           .First();

                            viewModel.SymbolEffectType = CharacterSymbolEffectType.ColorClamp;
                        }
                        break;
                    case SymbolType.Symbol:
                    case SymbolType.Game:
                    case SymbolType.OrientedSymbol:
                    case SymbolType.Terrain:
                        {
                            // Reset symbol path to a known resource for this type
                            viewModel.SymbolPath = _scenarioResourceService.GetResourceNames(viewModel.SymbolType)
                                                                           .First();

                            if (viewModel.SymbolType == SymbolType.Terrain ||
                                viewModel.SymbolType == SymbolType.Smiley)
                                viewModel.SymbolSize = CharacterSymbolSize.Large;

                            if (viewModel.SymbolType == SymbolType.Game)
                                viewModel.SymbolEffectType = CharacterSymbolEffectType.None;
                        }
                        break;
                    default:
                        throw new Exception("Unhandled symbol type SymbolEditor");
                }
            }
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
