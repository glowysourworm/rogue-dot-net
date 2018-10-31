using Rogue.NET.Common.View;
using Rogue.NET.Core.Model.Enums;
using System;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Assets
{
    [Export]
    public partial class Layout : UserControl
    {
        public Layout()
        {
            InitializeComponent();
        }

        private void EnumComboBox_EnumValueChanged(object sender, RoutedEventArgs e)
        {
            var enumCB = sender as EnumComboBox;
            var val = Enum.Parse(enumCB.EnumType, enumCB.EnumValue.ToString());
            switch ((LayoutType)val)
            {
                case LayoutType.BigRoom:
                case LayoutType.Normal:
                    {
                        this.HiddenDoorGB.Visibility = Visibility.Visible;
                        this.MazeGB.Visibility = Visibility.Collapsed;
                    }
                    break;
                case LayoutType.Maze:
                    {
                        this.HiddenDoorGB.Visibility = Visibility.Collapsed;
                        this.MazeGB.Visibility = Visibility.Visible;
                    }
                    break;
                case LayoutType.Teleport:
                case LayoutType.TeleportRandom:
                case LayoutType.Hall:
                    {
                        this.HiddenDoorGB.Visibility = Visibility.Collapsed;
                        this.MazeGB.Visibility = Visibility.Collapsed;
                    }
                    break;
            }
        }
    }
}
