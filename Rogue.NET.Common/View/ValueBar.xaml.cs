using System;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Rogue.NET.Common.View
{
    [Export]
    public partial class ValueBar : UserControl
    {
        #region Dependency Properties
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double), typeof(ValueBar), 
                new PropertyMetadata(0D, new PropertyChangedCallback(OnValueChanged)));

        public static readonly DependencyProperty ValueLowProperty =
            DependencyProperty.Register("ValueLow", typeof(double), typeof(ValueBar),
                new PropertyMetadata(0D, new PropertyChangedCallback(OnValueChanged)));

        public static readonly DependencyProperty ValueHighProperty =
            DependencyProperty.Register("ValueHigh", typeof(double), typeof(ValueBar),
                new PropertyMetadata(0D, new PropertyChangedCallback(OnValueChanged)));

        public static readonly DependencyProperty ValueForegroundProperty =
            DependencyProperty.Register("ValueForeground", typeof(Brush), typeof(ValueBar),
                new PropertyMetadata(Brushes.Red, new PropertyChangedCallback(OnColorChanged)));

        public static readonly DependencyProperty ValueBackgroundProperty =
            DependencyProperty.Register("ValueBackground", typeof(Brush), typeof(ValueBar), 
                new PropertyMetadata(Brushes.DarkRed, new PropertyChangedCallback(OnColorChanged)));

        public static readonly DependencyProperty ValueBorderProperty =
            DependencyProperty.Register("ValueBorder", typeof(Brush), typeof(ValueBar), 
                new PropertyMetadata(Brushes.OrangeRed, new PropertyChangedCallback(OnColorChanged)));
        #endregion

        #region Properties
        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
        public double ValueLow
        {
            get { return (double)GetValue(ValueLowProperty); }
            set { SetValue(ValueLowProperty, value); }
        }
        public double ValueHigh
        {
            get { return (double)GetValue(ValueHighProperty); }
            set { SetValue(ValueHighProperty, value); }
        }
        public Brush ValueForeground
        {
            get { return (Brush)GetValue(ValueForegroundProperty); }
            set { SetValue(ValueForegroundProperty, value); }
        }
        public Brush ValueBackground
        {
            get { return (Brush)GetValue(ValueBackgroundProperty); }
            set { SetValue(ValueBackgroundProperty, value); }
        }
        public Brush ValueBorder
        {
            get { return (Brush)GetValue(ValueBorderProperty); }
            set { SetValue(ValueBorderProperty, value); }
        }
        #endregion

        public ValueBar()
        {
            InitializeComponent();
        }

        protected void ReInitialize()
        {
            // Validate the limits
            if (this.ValueLow > this.ValueHigh)
                return;

            if (this.ValueLow == this.ValueHigh)
                return;

            if (this.ValueLow < 0 && this.ValueHigh < 0)
                return;

            if (this.Value < this.ValueLow)
                return;

            if (this.Value > this.ValueHigh)
                return;

            if (this.ValueLow == double.NaN || this.ValueHigh == double.NaN || this.Value == double.NaN)
                return;

            var valueTotal = this.ValueHigh - this.ValueLow;

            // Split Bar
            if (this.ValueLow < 0 && this.ValueHigh > 0)
            {
                // Negative Value
                if (this.Value < 0)
                {
                    // Calculate position of negative oriented bar
                    var totalNegativeWidth = Math.Abs(this.ValueLow / valueTotal) * this.RenderSize.Width;
                    var negativePosition = (this.Value / this.ValueLow) * totalNegativeWidth;
                    var negativeWidth = totalNegativeWidth - negativePosition;

                    // Position
                    this.ValueRectangle.Margin = new Thickness(negativePosition, 0, 0, 0);

                    // Middle - Position = Negative Bar Width
                    this.ValueRectangle.Width = negativeWidth;
                }
                // Positive Value
                else
                {
                    // Calculate position of positive oriented bar
                    var totalPositiveWidth = Math.Abs(this.ValueHigh / valueTotal) * this.RenderSize.Width;
                    var positiveWidth = (this.Value / this.ValueHigh) * totalPositiveWidth;
                    var zeroPosition = Math.Abs(this.ValueLow / valueTotal) * this.RenderSize.Width;

                    // Position
                    this.ValueRectangle.Margin = new Thickness(zeroPosition, 0, 0, 0);

                    // Width
                    this.ValueRectangle.Width = positiveWidth;
                }
            }

            // Completely Positive
            else
            {
                // Calculate position of positive oriented bar
                var positiveWidth = ((this.Value - this.ValueLow) / valueTotal) * this.RenderSize.Width;

                // *** TREAT ValueLow as Zero Position
                this.ValueRectangle.Margin = new Thickness(0);

                // Width
                this.ValueRectangle.Width = positiveWidth;
            }
        }

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ValueBar).ReInitialize();
        }
        private static void OnModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ValueBar).ReInitialize();
        }
        private static void OnColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var valueBar = d as ValueBar;

            valueBar.ValueBackgroundRectangle.Fill = valueBar.ValueBackground;
            valueBar.ValueRectangle.Fill = valueBar.ValueForeground;
            valueBar.ValueRectangle.Stroke = valueBar.ValueBorder;
        }
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);

            ReInitialize();
        }
    }
}
