using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Rogue.NET.Common.View
{
    public partial class ProgressBar : UserControl
    {
        public static readonly DependencyProperty Value1Property;
        public static readonly DependencyProperty Value2Property;
        public static readonly DependencyProperty ValueMaxProperty;
        public static readonly DependencyProperty BarColor1Property;

        public Color BarColor1
        {
            get { return (Color)GetValue(ProgressBar.BarColor1Property); }
            set { SetValue(ProgressBar.BarColor1Property, value); }
        }
        public string TextFormat { get; set; }
        public string Header
        {
            get { return this.HeaderText.Text; }
            set { this.HeaderText.Text = value; }
        }
        public Brush ValueTextBrush
        {
            get { return this.ValueText.Foreground; }
            set { this.ValueText.Foreground = value; }
        }
        public double Value
        {
            get { return (double)GetValue(Value1Property); }
            set { SetValue(Value1Property, value); }
        }
        public double Value2
        {
            get { return (double)GetValue(Value2Property); }
            set { SetValue(Value2Property, value); }
        }
        public double ValueMax
        {
            get { return (double)GetValue(ValueMaxProperty); }
            set { SetValue(ValueMaxProperty, value); }
        }
        public bool TextOn
        {
            get { return this.ValueText.Visibility == Visibility.Visible; }
            set { this.ValueText.Visibility = value == true ? Visibility.Visible : Visibility.Hidden; }
        }
        public bool HeaderTextOn
        {
            get { return this.HeaderText.Visibility == Visibility.Visible; }
            set
            {
                if (value)
                {
                    this.HeaderColumn.Width = new GridLength(80);
                    this.HeaderText.Visibility = Visibility.Visible;
                }
                else
                {
                    this.HeaderColumn.Width = new GridLength(0);
                    this.HeaderText.Visibility = Visibility.Collapsed;
                }
            }
        }
        public bool Invert { get; set; }

        static ProgressBar()
        {
            PropertyMetadata m0 = new PropertyMetadata(1.0, new PropertyChangedCallback(OnValue1Changed));
            PropertyMetadata m1 = new PropertyMetadata(1.0, new PropertyChangedCallback(OnValue2Changed));
            PropertyMetadata m2 = new PropertyMetadata(1.0, new PropertyChangedCallback(OnValueMaxChanged));
            PropertyMetadata m3 = new PropertyMetadata(new PropertyChangedCallback(OnBarColor1Changed));

            ProgressBar.Value1Property = DependencyProperty.Register("Value", typeof(double), typeof(ProgressBar), m0);
            ProgressBar.Value2Property = DependencyProperty.Register("Value2", typeof(double), typeof(ProgressBar), m1);
            ProgressBar.ValueMaxProperty = DependencyProperty.Register("ValueMax", typeof(double), typeof(ProgressBar), m2);
            ProgressBar.BarColor1Property = DependencyProperty.Register("BarColor1", typeof(Color), typeof(ProgressBar), m3);
        }
        public ProgressBar()
        {
            InitializeComponent();

            this.TextFormat = "N0";
        }
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            if (this.ValueMax <= 0)
                return;

            SetBarWidths(this);
        }
        private static void OnValue1Changed(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ProgressBar b = o as ProgressBar;
            SetBarWidths(b);
        }
        private static void OnValue2Changed(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ProgressBar b = o as ProgressBar;
            SetBarWidths(b);
        }
        private static void OnValueMaxChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ProgressBar b = o as ProgressBar;
            SetBarWidths(b);
        }
        private static void SetBarWidths(ProgressBar b)
        {
            double v = b.Value >= 0 ? b.Value : 0;
            double v2 = b.Value2 >= 0 ? b.Value2 : 0;
            double vm = b.ValueMax > 0 ? b.ValueMax : double.MaxValue;
            double w = b.HeaderTextOn ? (b.RenderSize.Width - 80) * (v2 / vm) : b.RenderSize.Width * (v2 / vm);

            w = Math.Max(w, 0);
            w = Math.Min(w, b.MaxWidth);
            b.Bar1.Width = w;
            b.BarGloss.Width = w;
            b.BarBackground.Width = Math.Max(b.HeaderTextOn ? (b.RenderSize.Width - 80) : b.RenderSize.Width, 0);

            b.ValueText.Text = (b.Value2).ToString(b.TextFormat) + " / " + (b.ValueMax).ToString(b.TextFormat);

            if (v2 > v)
                b.HeaderText.Foreground = b.Invert ? Brushes.Red : Brushes.GreenYellow;
            else if (v2 < v)
                b.HeaderText.Foreground = b.Invert ? Brushes.GreenYellow : Brushes.Red;
            else
                b.HeaderText.Foreground = Brushes.White;

            b.InvalidateVisual();
        }
        private static void OnBarColor1Changed(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ProgressBar b = o as ProgressBar;
            b.Gradient1.Color = (Color)e.NewValue;
            b.Gradient2.Color = (Color)e.NewValue;
            b.Bar1.Fill = new SolidColorBrush((Color)e.NewValue);

            Color bkgdColor = (Color)e.NewValue;
            bkgdColor.A = 0x2f;

            b.BarBackground.Fill = new SolidColorBrush(bkgdColor);
        }
    }
}