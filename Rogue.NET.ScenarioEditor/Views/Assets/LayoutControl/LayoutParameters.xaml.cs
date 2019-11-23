using Rogue.NET.Core.Converter.Model.ScenarioConfiguration;
using Rogue.NET.ScenarioEditor.Utility;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Layout;
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
                _viewModel = (e.NewValue as LayoutTemplateViewModel) ?? _viewModel;
            };
        }

        private void LayoutHeight_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (_viewModel == null)
                return;

            // Use converter to get value to set to the height
            _viewModel.Height = (int)_heightConverter.Convert(new object[]
            {
                (int)e.NewValue,
                _viewModel.Width,
                _viewModel.Type,
                _viewModel.SymmetryType,
                _viewModel.RegionWidthRange.Low,
                _viewModel.RegionWidthRange.High,
                _viewModel.RegionHeightRange.Low,
                _viewModel.RegionHeightRange.High,
                _viewModel.NumberRoomCols,
                _viewModel.NumberRoomRows,
                _viewModel.RectangularGridPadding,
                _viewModel.RandomRoomCount,
                _viewModel.RandomRoomSpread,
                _viewModel.MakeSymmetric
            }, null, null, null);
        }

        private void LayoutWidth_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (_viewModel == null)
                return;

            // Use converter to get value to set to the height
            _viewModel.Width = (int)_widthConverter.Convert(new object[]
            {
                _viewModel.Height,
                (int)e.NewValue,
                _viewModel.Type,
                _viewModel.SymmetryType,
                _viewModel.RegionWidthRange.Low,
                _viewModel.RegionWidthRange.High,
                _viewModel.RegionHeightRange.Low,
                _viewModel.RegionHeightRange.High,
                _viewModel.NumberRoomCols,
                _viewModel.NumberRoomRows,
                _viewModel.RectangularGridPadding,
                _viewModel.RandomRoomCount,
                _viewModel.RandomRoomSpread,
                _viewModel.MakeSymmetric
            }, null, null, null);
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
    }
}
