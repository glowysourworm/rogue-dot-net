using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration;
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

        public double HighLimit
        {
            get { return (int)GetValue(HighLimitProperty); }
            set { SetValue(HighLimitProperty, value); }
        }
        public double LowLimit
        {
            get { return (int)GetValue(LowLimitProperty); }
            set { SetValue(LowLimitProperty, value); }
        }

        [ImportingConstructor]
        public Range()
        {
            InitializeComponent();

            this.DataContextChanged += Range_DataContextChanged;
        }

        private void Range_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is RangeViewModel<int>)
            {
                this.LowUD.Increment = 1;
                this.HighUD.Increment = 1;

                this.LowUD.FormatString = "N0";
                this.HighUD.FormatString = "N0";
            }
            else
            {
                this.LowUD.Increment = 0.1;
                this.HighUD.Increment = 0.1;

                this.LowUD.FormatString = "F1";
                this.HighUD.FormatString = "F1";
            }
        }
    }
}
