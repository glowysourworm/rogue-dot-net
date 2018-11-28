using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Rogue.NET.Common.View
{
    [Export]
    public partial class DisappearingButton : UserControl
    {
        public static readonly RoutedEvent ClickEvent = EventManager.RegisterRoutedEvent("Click", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(DisappearingButton));

       
        public event RoutedEventHandler Click 
        {
            add { AddHandler(ClickEvent, value); }
            remove { RemoveHandler(ClickEvent, value); }
        }

        public double Threshold { get; set; }

        bool _disabled = false;
        public bool Disabled 
        {
            get { return _disabled; }
            set
            {
                _disabled = value;
                if (value)
                    this.Visibility = Visibility.Visible;
                else
                    this.Visibility = Visibility.Hidden;
            }
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

        public int ImagePadding { get; set; }

        [ImportingConstructor]
        public DisappearingButton()
        {
            InitializeComponent();

            this.DataContext = this;

            this.Loaded += DisappearingButton_Loaded;
        }

        void DisappearingButton_Loaded(object sender, RoutedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(this))
                return;

            Application.Current.MainWindow.MouseMove += (obj, ev) =>
            {
                if (this.Disabled)
                    return;

                var mousePosition = ev.GetPosition(this);

                var distance = Point.Subtract(mousePosition, new Point(0,0)).Length;
                if (distance < this.Threshold)
                {
                    var ratio = (distance / this.Threshold);
                    this.Visibility = Visibility.Visible;
                    this.Opacity = ratio < 0.8 ? 1 : 1 - ratio;
                }

                else
                    this.Visibility = Visibility.Hidden;
            };
        }
        private void TheButton_Click(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(ClickEvent, this));
        }
    }
}
