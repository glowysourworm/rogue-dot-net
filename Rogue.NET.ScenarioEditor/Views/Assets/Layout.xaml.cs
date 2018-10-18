using Rogue.NET.Common;
using Rogue.NET.Common.Views;
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

namespace Rogue.NET.ScenarioEditor.Views.Assets
{
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
