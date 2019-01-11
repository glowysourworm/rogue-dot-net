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
            DependencyProperty.Register("ValueForeground", typeof(Brush), typeof(ValueBar), new PropertyMetadata(Brushes.Red));

        public static readonly DependencyProperty ValueBackgroundProperty =
            DependencyProperty.Register("ValueBackground", typeof(Brush), typeof(ValueBar), new PropertyMetadata(Brushes.DarkRed));

        public static readonly DependencyProperty ValueBorderProperty =
            DependencyProperty.Register("ValueBorder", typeof(Brush), typeof(ValueBar), new PropertyMetadata(Brushes.OrangeRed));
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

            this.DataContext = this;
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

            var valueTotal = this.ValueHigh - this.ValueLow;

            // Split Bar
            if (this.ValueLow < 0 && this.ValueHigh > 0)
            {
                // Negative Value
                if (this.Value <= 0)
                {
                    // Calculate position of negative oriented bar
                    var totalNegativeWidth = Math.Abs(this.ValueLow / valueTotal) * this.Width;
                    var negativePosition = (this.Value / this.ValueLow) * totalNegativeWidth;
                    var negativeWidth = totalNegativeWidth - negativePosition;

                    // Position
                    Canvas.SetLeft(this.ValueRectangle, negativePosition);

                    // Middle - Position = Negative Bar Width
                    this.ValueRectangle.Width = negativeWidth;
                }
                // Positive Value
                else
                {
                    // Calculate position of positive oriented bar
                    var totalPositiveWidth = Math.Abs(this.ValueHigh / valueTotal) * this.Width;
                    var positiveWidth = (this.Value / this.ValueHigh) * totalPositiveWidth;
                    var zeroPosition = Math.Abs(this.ValueLow / valueTotal) * this.Width;

                    // Position
                    Canvas.SetLeft(this.ValueRectangle, zeroPosition);

                    // Width
                    this.ValueRectangle.Width = positiveWidth;
                }
            }

            // Completely Positive
            else
            {
                // Calculate position of positive oriented bar
                var positiveWidth = ((this.Value - this.ValueLow) / valueTotal) * this.Width;

                // *** TREAT ValueLow as Zero Position
                Canvas.SetLeft(this.ValueRectangle, 0);

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
    }
}
