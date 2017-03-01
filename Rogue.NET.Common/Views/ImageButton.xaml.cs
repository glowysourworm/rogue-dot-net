using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace Rogue.NET.Common.Views
{
    public partial class ImageButton : UserControl
    {
        public static readonly RoutedEvent ClickEvent = EventManager.RegisterRoutedEvent("Click", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ImageButton));

        public event RoutedEventHandler Click 
        {
            add { AddHandler(ClickEvent, value); }
            remove { RemoveHandler(ClickEvent, value); }
        }

        public ImageSource Source
        {
            get { return this.TheButton.Tag as ImageSource; }
            set 
            { 
                this.TheButton.Tag = value;
                InvalidateVisual();
            }
        }

        public Thickness ImagePadding
        {
            get { return this.TheButton.Padding; }
            set { this.TheButton.Padding = value; }
        }

        public ImageButton()
        {
            InitializeComponent();

            this.TheButton.Click += TheButton_Click;
        }

        private void TheButton_Click(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(ClickEvent, this));
        }
    }
}
