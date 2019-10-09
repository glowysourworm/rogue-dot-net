using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration;
using System;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;


namespace Rogue.NET.ScenarioEditor.Views.Controls
{
    [Export]
    public partial class Range : UserControl
    {
        public static readonly DependencyProperty HighLimitProperty =
            DependencyProperty.Register("HighLimit", typeof(double), typeof(Range));

        public static readonly DependencyProperty LowLimitProperty =
            DependencyProperty.Register("LowLimit", typeof(double), typeof(Range));

        public static readonly DependencyProperty IncrementProperty =
            DependencyProperty.Register("Increment", typeof(double), typeof(Range), new PropertyMetadata(0.01));

        public static readonly DependencyProperty StringFormatProperty =
            DependencyProperty.Register("StringFormat", typeof(string), typeof(Range), new PropertyMetadata("F2"));

        public double HighLimit
        {
            get { return (double)GetValue(HighLimitProperty); }
            set { SetValue(HighLimitProperty, value); }
        }
        public double LowLimit
        {
            get { return (double)GetValue(LowLimitProperty); }
            set { SetValue(LowLimitProperty, value); }
        }
        public double Increment
        {
            get { return (double)GetValue(IncrementProperty); }
            set { SetValue(IncrementProperty, value); }
        }
        public string StringFormat
        {
            get { return (string)GetValue(StringFormatProperty); }
            set { SetValue(StringFormatProperty, value); }
        }

        [ImportingConstructor]
        public Range()
        {
            InitializeComponent();
        }
    }
}
