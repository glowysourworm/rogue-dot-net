using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    public partial class Slider : UserControl
    {
        public static readonly DependencyProperty SliderValueProperty =
            DependencyProperty.Register("SliderValue", typeof(double), typeof(Slider), new PropertyMetadata(OnSliderValueChanged));

        public string Title
        {
            get { return ""; }
            set { }
        }
        public bool TextVisible
        {
            get { return false; }
            set 
            {
                //this.TitleColumn.Width = (value) ? new GridLength(300): new GridLength(0);
                this.ValueColumn.Width = (value) ? new GridLength(80) : new GridLength(0);
            }
        }
        public Brush TitleForeground
        {
            get { /*return this.TitleTextBlock.Foreground;*/ return Brushes.White; }
            set { /*this.TitleTextBlock.Foreground = value;*/ }
        }
        public double SliderValue
        {
            get { return (double)this.TheSlider.GetValue(Slider.SliderValueProperty); }
            set { this.TheSlider.SetValue(Slider.SliderValueProperty, value); }
        }
        public double SliderMin
        {
            get { return this.TheSlider.Minimum; }
            set { this.TheSlider.Minimum = value; }
        }
        public double SliderMax
        {
            get { return this.TheSlider.Maximum; }
            set { this.TheSlider.Maximum = value; }
        }
        public double SliderSmallChange
        {
            get { return this.TheSlider.SmallChange; }
            set { this.TheSlider.SmallChange = value; }
        }
        public double SliderLargeChange
        {
            get { return this.TheSlider.LargeChange; }
            set { this.TheSlider.LargeChange = value; }
        }
        public string TextFormat { get; set; }
        public Slider()
        {
            InitializeComponent();
            this.TextVisible = true;
            this.TextFormat = "F";
            this.TheSlider.ValueChanged += new RoutedPropertyChangedEventHandler<double>(TheSlider_ValueChanged);
            this.DataContextChanged += new DependencyPropertyChangedEventHandler(Rogue2Slider_DataContextChanged);
        }

        private void Rogue2Slider_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            BindingExpression b = GetBindingExpression(SliderValueProperty);
            if (b != null)
            {
                b.UpdateTarget();
            }
        }
        private void TheSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SetValue(SliderValueProperty, e.NewValue);
            this.SliderTextBlock.Text = e.NewValue.ToString(this.TextFormat);
        }
        private static void OnSliderValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            //Need to update slider..
            //SliderValue is a separate double from the Slider.ValueProperty dependency property.
            //So, when the target "SliderValue" updates, have to force an update of the slider.
            //Moving the slider after this will fire the "ValueChanged" event, and then I can update
            //the source by using SetValue(SliderValueProperty)
            Slider s = (Slider)o;
            s.TheSlider.Value = (double)e.NewValue;
        }
    }
}
