using Rogue.NET.Core.Converter.Model.ScenarioConfiguration;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.ScenarioEditor.Utility;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Layout;
using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Assets.LayoutControl
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export]
    public partial class LayoutParameters : UserControl
    {
        LayoutTemplateViewModel _viewModel;

        // BINDING ISSUE:  Multibinding used to set value for height / width of grid based on
        //                 other parameters; but ALSO to set actual Height and Width variables.
        //      
        //                 WPF binding doesn't seem to allow for this - so if I find something
        //                 in the future I'll fix it. Otherwise - have to set it manually. Must
        //                 prevent manual setting for all modes except Maze and Cellular Automata
        //                 where manual setting is required.
        LayoutWidthConverter _widthConverter;
        LayoutHeightConverter _heightConverter;

        public LayoutParameters()
        {
            InitializeComponent();

            _widthConverter = new LayoutWidthConverter();
            _heightConverter = new LayoutHeightConverter();

            this.DataContextChanged += (sender, e) =>
            {
                _viewModel = (e.NewValue as LayoutTemplateViewModel);

                var newViewModel = e.NewValue as LayoutTemplateViewModel;
                var oldViewModel = e.OldValue as LayoutTemplateViewModel;

                if (oldViewModel != null)
                    oldViewModel.PropertyChanged -= OnLayoutPropertyChanged;

                if (newViewModel != null)
                    newViewModel.PropertyChanged += OnLayoutPropertyChanged;
            };
        }

        private void OnLayoutPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // TODO: Filter these in the UI
            if (e.PropertyName == "Type" ||
                e.PropertyName == "ConnectionType")
            {
                if (_viewModel.Type == LayoutType.CellularAutomataMazeMap ||
                    _viewModel.Type == LayoutType.ElevationMazeMap ||
                    _viewModel.Type == LayoutType.MazeMap &&
                    _viewModel.ConnectionType == LayoutConnectionType.Maze)
                    _viewModel.ConnectionType = LayoutConnectionType.Corridor;
            }
        }

        private void EditWallSymbolButton_Click(object sender, RoutedEventArgs e)
        {
            DialogWindowFactory.ShowSymbolEditor((this.DataContext as LayoutTemplateViewModel).WallSymbol);
        }

        private void EditDoorSymbolButton_Click(object sender, RoutedEventArgs e)
        {
            DialogWindowFactory.ShowSymbolEditor((this.DataContext as LayoutTemplateViewModel).DoorSymbol);
        }

        private void EditCellSymbolButton_Click(object sender, RoutedEventArgs e)
        {
            DialogWindowFactory.ShowSymbolEditor((this.DataContext as LayoutTemplateViewModel).CellSymbol);
        }

        private void EditWallLightSymbolButton_Click(object sender, RoutedEventArgs e)
        {
            DialogWindowFactory.ShowSymbolEditor((this.DataContext as LayoutTemplateViewModel).WallLightSymbol);
        }
    }
}
