using Rogue.NET.Core.Converter.Model.ScenarioConfiguration;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Layout;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Assets
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export]
    public partial class Layout : UserControl
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

        [ImportingConstructor]
        public Layout()
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
                _viewModel.Type,
                _viewModel.RoomPlacementType,
                _viewModel.RoomHeightLimit,
                _viewModel.RectangularGridPadding,
                _viewModel.NumberRoomRows,
                _viewModel.RandomRoomCount,
                _viewModel.RandomRoomSpread,
                (int)e.NewValue
            }, null, null, null);
        }

        private void LayoutWidth_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (_viewModel == null)
                return;

            // Use converter to get value to set to the height
            _viewModel.Width = (int)_widthConverter.Convert(new object[]
            {
                _viewModel.Type,
                _viewModel.RoomPlacementType,
                _viewModel.RoomWidthLimit,
                _viewModel.RectangularGridPadding,
                _viewModel.NumberRoomCols,
                _viewModel.RandomRoomCount,
                _viewModel.RandomRoomSpread,
                (int)e.NewValue
            }, null, null, null);
        }
    }
}
